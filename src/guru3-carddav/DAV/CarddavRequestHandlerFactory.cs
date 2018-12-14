using NWebDav.Server;
using NWebDav.Server.Http;

namespace eventphone.guru3.carddav.DAV
{
    public class CarddavRequestHandlerFactory : IRequestHandlerFactory
    {
        private readonly IRequestHandlerFactory _requestHandlerFactory;
        private readonly CarddavOptionsHandler _optionsHandler;

        public CarddavRequestHandlerFactory()
        {
            _requestHandlerFactory = new RequestHandlerFactory();
            _optionsHandler = new CarddavOptionsHandler();
        }

        public IRequestHandler GetRequestHandler(IHttpContext httpContext)
        {
            if (httpContext.Request.HttpMethod == "OPTIONS")
            {
                return _optionsHandler;
            }
            return _requestHandlerFactory.GetRequestHandler(httpContext);
        }
    }
}