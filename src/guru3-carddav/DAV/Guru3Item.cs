using System;
using System.IO;
using System.Threading.Tasks;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class Guru3Item : IStoreItem
    {
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string UniqueKey 
        {
            get { throw new NotImplementedException(); }
        }

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, Stream source)
        {
            throw new NotImplementedException();
        }

        public Task<StoreItemResult> CopyAsync(IStoreCollection destination, string name, bool overwrite, IHttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public IPropertyManager PropertyManager 
        {
            get { throw new NotImplementedException(); }
        }

        public ILockingManager LockingManager
        {
            get { throw new NotImplementedException(); }
        }
    }
}