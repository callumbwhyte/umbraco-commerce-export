using System.IO;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Contrib.Export.Notifications
{
    public class ExportRenderingNotification : ExportTemplateNotificationEventBase<ExportTemplateReadOnly>
    {
        public ExportRenderingNotification(ExportTemplateReadOnly exportTemplate)
            : base(exportTemplate)
        {

        }

        public string? Content { get; init; }

        public Stream? Stream { get; private set; }

        public void SetStream(Stream stream)
        {
            Stream = stream;
        }
    }
}