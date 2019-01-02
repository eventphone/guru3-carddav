using NWebDav.Server;
using NWebDav.Server.Http;

namespace eventphone.guru3.carddav.DAV
{
    public class CarddavRequestHandlerFactory : IRequestHandlerFactory
    {
        private readonly IRequestHandlerFactory _requestHandlerFactory;
        private readonly CarddavOptionsHandler _optionsHandler;
        private readonly WellKnownHandler _wellknownHandler;
        private readonly CarddavReportHandler _reportHandler;

        public CarddavRequestHandlerFactory()
        {
            _requestHandlerFactory = new RequestHandlerFactory();
            _optionsHandler = new CarddavOptionsHandler();
            _reportHandler = new CarddavReportHandler();
            _wellknownHandler = new WellKnownHandler("/");
        }

        public IRequestHandler GetRequestHandler(IHttpContext httpContext)
        {
            if (httpContext.Request.HttpMethod == "OPTIONS")
            {
                return _optionsHandler;
            }
            if (httpContext.Request.HttpMethod == "REPORT")
            {
                return _reportHandler;
            }
            if (httpContext.Request.HttpMethod == "PROPFIND")
            {
                if (httpContext.Request.Url.AbsolutePath == "/.well-known/carddav")
                {
                    return _wellknownHandler;
                }
            }
            return _requestHandlerFactory.GetRequestHandler(httpContext);
        }
    }
}