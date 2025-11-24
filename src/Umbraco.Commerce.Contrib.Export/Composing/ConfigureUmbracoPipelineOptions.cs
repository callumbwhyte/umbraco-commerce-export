using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Commerce.Contrib.Export.Controllers;
using Umbraco.Commerce.Core.Configuration;
using Umbraco.Extensions;

namespace Umbraco.Commerce.Contrib.Export.Composing
{
    /// <summary>
    /// Register a route with the same pattern as uc-export mapped to <see cref="ExportRenderingController"/> instead
    /// </summary>
    internal class ConfigureUmbracoPipelineOptions : IConfigureOptions<UmbracoPipelineOptions>
    {
        private readonly IUmbracoCommerceSettings _settings;

        public ConfigureUmbracoPipelineOptions(IUmbracoCommerceSettings settings)
        {
            _settings = settings;
        }

        public void Configure(UmbracoPipelineOptions options)
        {
            options.AddFilter(new UmbracoPipelineFilter("Umbraco.Commerce.Contrib.Export")
            {
                Endpoints = app =>
                {
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllerRoute("uc-contrib-export", _settings.BackOfficeMvcArea + "/backoffice/commerce/export/{action}", new
                        {
                            controller = nameof(ExportRenderingController).TrimEnd("Controller"),
                            action = nameof(ExportRenderingController.Export)
                        });
                    });
                }
            });
        }
    }
}