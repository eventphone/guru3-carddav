using System;
using System.Collections.Generic;
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
    public class Guru3Collection : IStoreCollection
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

        public Task<IStoreItem> GetItemAsync(string name, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<IStoreItem>> GetItemsAsync(IHttpContext httpContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<StoreItemResult> CreateItemAsync(string name, bool overwrite, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new StoreItemResult(DavStatusCode.NotImplemented));
        }

        public Task<StoreCollectionResult> CreateCollectionAsync(string name, bool overwrite, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new StoreCollectionResult(DavStatusCode.NotImplemented));
        }

        public bool SupportsFastMove(IStoreCollection destination, string destinationName, bool overwrite, IHttpContext httpContext)
        {
            return false;
        }

        public Task<StoreItemResult> MoveItemAsync(string sourceName, IStoreCollection destination, string destinationName, bool overwrite,
            IHttpContext httpContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new StoreItemResult(DavStatusCode.NotImplemented));
        }

        public Task<DavStatusCode> DeleteItemAsync(string name, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(DavStatusCode.NotImplemented);
        }

        public InfiniteDepthMode InfiniteDepthMode => InfiniteDepthMode.Rejected;
    }
}