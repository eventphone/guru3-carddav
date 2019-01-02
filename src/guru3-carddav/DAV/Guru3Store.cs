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
                return Task.FromResult(GetRoot());
            }
            if (parts.Length == 1)
            {
                return GetEventAsync(parts[0], cancellationToken);
            }
            if (parts.Length == 2)
            {
                return GetExtensionAsync(parts[0], parts[1], cancellationToken);
            }
            throw new NotImplementedException();
        }

        private IStoreItem GetRoot()
        {
            return new Guru3RootCollection(_context);
        }

        private async Task<IStoreItem> GetEventAsync(string eventName, CancellationToken cancellationToken)
        {
            var dbEvent = await _context.Events.AsNoTracking()
                .Active()
                .Where(x => x.Name == eventName)
                .Select(x=>new {x.Id, x.Name})
                .FirstOrDefaultAsync(cancellationToken);
            if (dbEvent == null)
                return null;
            return new Guru3Collection(dbEvent.Id, dbEvent.Name, _context);
        }

        private async Task<IStoreItem> GetExtensionAsync(string eventName, string number, CancellationToken cancellationToken)
        {
            var dbExtension = await _context.Extensions.AsNoTracking()
                .Active()
                .Where(x => x.Event.Name == eventName)
                .Where(x => x.Number == number)
                .Select(x => new {x.Id, x.Number})
                .FirstOrDefaultAsync(cancellationToken);
            if (dbExtension == null)
                return null;
            return new Guru3Item(dbExtension.Id, dbExtension.Number, _context);
        }

        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
