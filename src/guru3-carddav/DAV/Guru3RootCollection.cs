using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using eventphone.guru3.carddav.DAL;
using Microsoft.EntityFrameworkCore;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class Guru3RootCollection : Guru3Collection
    {
        private readonly Guru3Context _context;
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");

        public Guru3RootCollection(Guru3Context context)
            :base("eventphone")
        {
            _context = context;
        }

        protected override XElement[] RessourceType => new []{s_xDavCollection};

        public override async Task<IList<IStoreItem>> GetItemsAsync(IHttpContext httpContext, CancellationToken cancellationToken)
        {
            var events = await _context.Events.AsNoTracking()
                .Active()
                .Select(x => new {x.Id, x.Name})
                .ToListAsync(cancellationToken);
            var result = new List<IStoreItem>();
            foreach (var addressbook in events)
            {
                result.Add(new Guru3Collection(addressbook.Id, addressbook.Name, _context));
            }
            return result;
        }

        protected override Task<string> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(String.Empty);
        }
    }
}