using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class WellKnownHandler : IRequestHandler
    {
        private readonly string _root;

        public WellKnownHandler(string root)
        {
            _root = root;
        }

        public Task<bool> HandleRequestAsync(IHttpContext httpContext, IStore store, CancellationToken cancellationToken)
        {
            var response = httpContext.Response;
            response.Status = (int) HttpStatusCode.TemporaryRedirect;
            response.SetHeaderValue(nameof(HttpResponseHeader.Location), _root);
            return Task.FromResult(true);
        }
    }
}