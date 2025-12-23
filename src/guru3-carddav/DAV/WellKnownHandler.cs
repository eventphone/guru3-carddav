using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NWebDav.Server;
using NWebDav.Server.Handlers;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class WellKnownHandler : IRequestHandler
    {
        private readonly IStore _store;
        private readonly PropFindHandler _inner;

        public WellKnownHandler(IStore store, PropFindHandler inner)
        {
            _store = store;
            _inner = inner;
        }

        public Task<bool> HandleRequestAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/.well-known/carddav")
            {
                var response = httpContext.Response;
                response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
                response.Headers.Append(nameof(HttpResponseHeader.Location), "/");
                return Task.FromResult(true);
            }
            return _inner.HandleRequestAsync(httpContext);
        }
    }
}