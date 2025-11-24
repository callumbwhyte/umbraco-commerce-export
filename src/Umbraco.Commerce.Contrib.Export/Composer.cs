using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Commerce.Cms;
using Umbraco.Commerce.Contrib.Export.Composing;
using Umbraco.Commerce.Contrib.Export.Handlers;
using Umbraco.Commerce.Contrib.Export.Notifications;
using Umbraco.Commerce.Contrib.Export.Routing;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Contrib.Export
{
    [ComposeAfter(typeof(UmbracoCommerceComposer))]
    public class Composer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<MatcherPolicy, ExportRouteMatcherPolicy>();

            builder.Services.ConfigureOptions<ConfigureUmbracoPipelineOptions>();

            builder.WithNotificationEvent<ExportRenderingNotification>().RegisterHandler<DefaultExportRenderingHandler>();
        }
    }
}