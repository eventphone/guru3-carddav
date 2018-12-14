using System.Threading;
using System.Threading.Tasks;
using NWebDav.Server;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class CarddavOptionsHandler : IRequestHandler
    {
        public Task<bool> HandleRequestAsync(IHttpContext httpContext, IStore store, CancellationToken cancellationToken)
        {
            // Obtain response
            var response = httpContext.Response;

            // We're a DAV class 1 and 2 compatible server
            response.SetHeaderValue("Dav", "1, addressbook");

            // Set the Allow/Public headers
            response.SetHeaderValue("Allow", AllowedMethods);
            response.SetHeaderValue("Public", AllowedMethods);

            // Finished
            response.SetStatus(DavStatusCode.Ok);
            return Task.FromResult(true);
        }

        private static readonly string AllowedMethods = "OPTIONS, GET, HEAD, PROPFIND, REPORT";
    }
}