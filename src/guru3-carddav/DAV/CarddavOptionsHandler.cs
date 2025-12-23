using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NWebDav.Server;
using NWebDav.Server.Handlers;
using NWebDav.Server.Helpers;

namespace eventphone.guru3.carddav.DAV
{
    public class CarddavOptionsHandler : IRequestHandler
    {
        public Task<bool> HandleRequestAsync(HttpContext httpContext)
        {
            // Obtain response
            var response = httpContext.Response;

            // We're a DAV class 1 and 2 compatible server
            response.Headers.Append("Dav", "1, addressbook");

            // Set the Allow/Public headers
            response.Headers.Append("Allow", AllowedMethods);
            response.Headers.Append("Public", AllowedMethods);

            // Finished
            response.SetStatus(DavStatusCode.Ok);
            return Task.FromResult(true);
        }

        private static readonly string AllowedMethods = "OPTIONS, GET, HEAD, PROPFIND, REPORT";
    }
}