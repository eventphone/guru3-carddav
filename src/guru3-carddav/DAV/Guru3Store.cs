using System;
using System.Collections.Generic;
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
            IList<string> parts = path.Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries);
            var root = "/";
            if (parts.Count > 0 && "dav".Equals(parts[0], StringComparison.OrdinalIgnoreCase))
            {
                //remove proxy prefix
                parts = new List<string>(parts.Skip(1));
                root = "/dav";
            }
            if (parts.Count == 0)
            {
                return Task.FromResult(GetRoot(root));
            }
            if (parts.Count == 1)
            {
                return GetEventAsync(root, parts[0], cancellationToken);
            }
            if (parts.Count == 2)
            {
                return GetExtensionAsync(parts[0], parts[1], cancellationToken);
            }
            throw new NotImplementedException();
        }

        private IStoreItem GetRoot(string root)
        {
            return new Guru3RootCollection(root, _context);
        }

        private async Task<IStoreItem> GetEventAsync(string root, string eventName, CancellationToken cancellationToken)
        {
            var dbEvent = await _context.Events.AsNoTracking()
                .Active()
                .Where(x => x.Name == eventName)
                .Select(x=>new {x.Id, x.Name, LastChanged = x.Extensions.Max(y=>(DateTimeOffset?)y.LastChanged)})
                .FirstOrDefaultAsync(cancellationToken);
            if (dbEvent == null)
                return null;
            return new Guru3Collection(root, dbEvent.Id, dbEvent.Name, dbEvent.LastChanged.GetValueOrDefault(), _context);
        }

        private async Task<IStoreItem> GetExtensionAsync(string eventName, string number, CancellationToken cancellationToken)
        {
            var dbExtension = await _context.Extensions.AsNoTracking()
                .Active()
                .Where(x => x.Event.Name == eventName)
                .Where(x => x.Number == number)
                .Select(x => new {x.Id, x.Number, x.LastChanged})
                .FirstOrDefaultAsync(cancellationToken);
            if (dbExtension == null)
                return null;
            return new Guru3Item(dbExtension.Id, dbExtension.Number, dbExtension.LastChanged, _context);
        }

        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
