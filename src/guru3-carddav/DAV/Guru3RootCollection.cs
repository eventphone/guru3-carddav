using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using eventphone.guru3.carddav.DAL;
using Microsoft.EntityFrameworkCore;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class Guru3RootCollection : Guru3Collection
    {
        private readonly Guru3Context _context;
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");

        public Guru3RootCollection(string root, Guru3Context context)
            :base(root, "eventphone")
        {
            _context = context;
        }

        protected override XElement[] RessourceType => new []{s_xDavCollection};

        public override async Task<IList<IStoreItem>> GetItemsAsync(IHttpContext httpContext, CancellationToken cancellationToken)
        {
            var events = await _context.Events.AsNoTracking()
                .Active()
                .Select(x => new {x.Id, x.Name, LastChanged = x.Extensions.Max(y=>(DateTimeOffset?)y.LastChanged)})
                .ToListAsync(cancellationToken);
            var result = new List<IStoreItem>();
            foreach (var addressbook in events)
            {
                result.Add(new Guru3Collection(Root, addressbook.Id, addressbook.Name, addressbook.LastChanged.GetValueOrDefault(), _context));
            }
            return result;
        }

        protected override Task<string> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(String.Empty);
        }

        public override Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, CancellationToken cancellationToken)
        {
            return Task.FromResult<Stream>(File.OpenRead("wwwroot/index.html"));
        }

        private static RootPropertyManager RootPropertyManager { get; } =
            new RootPropertyManager(DefaultPropertyManager);

        public override IPropertyManager PropertyManager => RootPropertyManager;
    }

    public class RootPropertyManager : IPropertyManager
    {
        private readonly IPropertyManager _inner;
        private readonly DavGetContentType<Guru3Collection> _contentType;

        public RootPropertyManager(IPropertyManager inner)
        {
            _inner = inner;
            _contentType = new DavGetContentType<Guru3Collection> {Getter = (context, collection) => "text/html"};
            Properties = new List<PropertyInfo>(_inner.Properties) {new PropertyInfo(_contentType.Name, false)};
        }

        public IList<PropertyInfo> Properties { get; }

        public Task<object> GetPropertyAsync(IHttpContext httpContext, IStoreItem item, XName propertyName, bool skipExpensive, CancellationToken cancellationToken)
        {
            if (propertyName == _contentType.Name)
                return Task.FromResult<object>(_contentType.Getter(httpContext, (Guru3Collection) item));
            return _inner.GetPropertyAsync(httpContext, item, propertyName, skipExpensive, cancellationToken);
        }

        public Task<DavStatusCode> SetPropertyAsync(IHttpContext httpContext, IStoreItem item, XName propertyName, object value, CancellationToken cancellationToken)
        {
            return _inner.SetPropertyAsync(httpContext, item, propertyName, value, cancellationToken);
        }
    }
}