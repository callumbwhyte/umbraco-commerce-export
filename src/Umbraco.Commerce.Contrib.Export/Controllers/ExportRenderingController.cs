using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Commerce.Cms.Web.Controllers;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Contrib.Export.Notifications;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.Templating;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Contrib.Export.Controllers
{
    /// <summary>
    /// Duplicate <see cref="UmbracoCommerceExportController"/> to dispatch notifications during rendering
    /// </summary>
    [IsBackOffice]
    [DisableBrowserCache]
    [Authorize(Policy = "SectionAccessCommerce")]
    public class ExportRenderingController : UmbracoController
    {
        private readonly UmbracoCommerceContext _context;
        private readonly ITemplateEngine _templateEngine;

        public ExportRenderingController(UmbracoCommerceContext context, ITemplateEngine templateEngine)
        {
            _context = context;
            _templateEngine = templateEngine;
        }

        [HttpPost]
        public IActionResult Export(ExportRequest request)
        {
            var entities = _context.Services.EntityService.GetEntities(request.EntityType, request.EntityIds);

            if (entities?.Any() != true)
            {
                return BadRequest();
            }

            var files = new List<ExportFile>();

            var templates = _context.Services.ExportTemplateService.GetExportTemplates(request.TemplateIds);

            foreach (var template in templates)
            {
                var viewPath = _context.Services.TranslationService.TranslateValue(template.TemplateView, request.LanguageIsoCode, out var usedLanguageIsoCode);

                if (template.ExportStrategy == ExportStrategy.SingleFile)
                {
                    var file = Render(template, viewPath?.ToString(), entities, usedLanguageIsoCode);

                    if (file != null)
                    {
                        files.Add(file);
                    }
                }
                else if (template.ExportStrategy == ExportStrategy.MultiFile)
                {
                    foreach (var entity in entities)
                    {
                        var file = Render(template, viewPath?.ToString(), entity, usedLanguageIsoCode);

                        if (file != null)
                        {
                            files.Add(file);
                        }
                    }
                }
            }

            if (files.Count == 1)
            {
                return File(files[0].Content!, files[0].MimeType!, files[0].FileName);
            }

            var archiveStream = new MemoryStream();

            using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var archiveEntry = archive.CreateEntry(file.FileName!, CompressionLevel.Fastest);

                    using var entryStream = archiveEntry.Open();

                    file.Content?.CopyTo(entryStream);
                }
            }

            archiveStream.Seek(0, SeekOrigin.Begin);

            return File(archiveStream, "application/zip", $"export_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}.zip");
        }

        private ExportFile? Render(ExportTemplateReadOnly template, string viewPath, object model, string languageIsoCode)
        {
            var content = _templateEngine.RenderTemplateView(viewPath, model, languageIsoCode);

            if (string.IsNullOrWhiteSpace(content) == true)
            {
                return null;
            }

            var notification = new ExportRenderingNotification(template)
            {
                Content = content
            };

            EventBus.Dispatch(notification);

            if (notification.Stream == null)
            {
                return null;
            }

            string? ToKebabCase(string value)
            {
                var method = typeof(StringExtensions).GetMethod("ToSnakeCase", BindingFlags.Static | BindingFlags.NonPublic);

                return method?.Invoke(null, [value])?.ToString()?.Replace("_", "-");
            }

            var fileName = $"{ToKebabCase(template.Alias)}_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}";

            if (model is EntityBase entity)
            {
                fileName += $"_{entity.Id}";
            }

            fileName += $".{template.FileExtension}";

            return new()
            {
                FileName = fileName,
                MimeType = template.FileMimeType,
                Content = notification.Stream
            };
        }

        public class ExportRequest
        {
            public string? EntityType { get; set; }

            public string? LanguageIsoCode { get; set; }

            public Guid[] TemplateIds { get; set; } = [];

            public Guid[] EntityIds { get; set; } = [];
        }

        public class ExportFile
        {
            public string? FileName { get; set; }

            public string? MimeType { get; set; }

            public Stream? Content { get; set; }
        }
    }
}