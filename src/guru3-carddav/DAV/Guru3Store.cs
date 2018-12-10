using System;
using System.Threading.Tasks;
using NWebDav.Server.Http;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class Guru3Store:IStore
    {
        public Task<IStoreItem> GetItemAsync(Uri uri, IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
