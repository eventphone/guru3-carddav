using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eventphone.guru3.carddav.DAL;
using Microsoft.EntityFrameworkCore;
using NWebDav.Server.Http;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class Guru3Store:IStore
    {
        private readonly Guru3Context _context;

        public Guru3Store(Guru3Context context)
        {
            _context = context;
        }

        public Task<IStoreItem> GetItemAsync(Uri uri, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            var path = uri.LocalPath;
            var parts = path.Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return GetRootAsync(cancellationToken);
            }
            if (parts.Length == 1)
            {
                return GetEventAsync(parts[0], cancellationToken);
            }
            throw new NotImplementedException();
        }

        private async Task<IStoreItem> GetRootAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<IStoreItem> GetEventAsync(string eventName, CancellationToken cancellationToken)
        {
            var dbEvent = _context.Events.Where(x => x.Name == eventName);
            if (!await dbEvent.AnyAsync(cancellationToken))
                return null;
            throw new NotImplementedException();
        }

        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
