using System.IO;
using System.Text;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Contrib.Export.Notifications;

namespace Umbraco.Commerce.Contrib.Export.Handlers
{
    /// <summary>
    /// Default rendering implementation, exports a UTF-8 stream to maintain original <see cref="UmbracoCommerceExportController"/> behaviour
    /// </summary>
    public class DefaultExportRenderingHandler : NotificationEventHandlerBase<ExportRenderingNotification>
    {
        public override void Handle(ExportRenderingNotification evt)
        {
            if (string.IsNullOrWhiteSpace(evt.Content) == true)
            {
                return;
            }

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(evt.Content));

            evt.SetStream(stream);
        }
    }
}