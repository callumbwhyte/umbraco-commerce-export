using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;

namespace Umbraco.Commerce.Contrib.Export.Routing
{
    /// <summary>
    /// Ensure uc-export route does not get matchd, and uc-contrib-export is used instead
    /// </summary>
    internal class ExportRouteMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        public override int Order => int.MinValue;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            return endpoints.Any(IsEndpoint);
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                var endpoint = candidates[i].Endpoint;

                if (IsEndpoint(endpoint) == true)
                {
                    candidates.SetValidity(i, false);
                }
            }

            return Task.CompletedTask;
        }

        private bool IsEndpoint(Endpoint endpoint)
        {
            return endpoint?.Metadata?.GetMetadata<RouteNameMetadata>()?.RouteName == "uc-export";
        }
    }
}