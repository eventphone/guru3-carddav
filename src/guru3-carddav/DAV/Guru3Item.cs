using System;
using System.IO;
using System.Threading;
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

        public Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, Stream source, CancellationToken cancellationToken)
        {
            return Task.FromResult(DavStatusCode.NotImplemented);
        }

        public Task<StoreItemResult> CopyAsync(IStoreCollection destination, string name, bool overwrite, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new StoreItemResult(DavStatusCode.NotImplemented));
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