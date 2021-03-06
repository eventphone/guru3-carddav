﻿using System;
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
using NWebDav.Server.Locking;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class Guru3Collection : IStoreCollection
    {
        private readonly Guru3Context _context;
        protected readonly string Root;
        private readonly string _name;
        private readonly int _id;
        private static readonly XElement _addressbook = new XElement(CardDavNamespace.CardDavNs + "addressbook");
        private static readonly XElement s_xDavCollection = new XElement(WebDavNamespaces.DavNs + "collection");
        private static readonly XElement _privileges = new XElement(WebDavNamespaces.DavNs + "privilege", new XElement(WebDavNamespaces.DavNs + "read"));
        private readonly string _etag;

        public Guru3Collection(string root, string name)
        {
            Root = root;
            _name = name;
        }

        public Guru3Collection(string root, int id, string name, DateTimeOffset lastChanged, Guru3Context context)
            :this(root, name)
        {
            _context = context;
            _id = id;
            _etag = lastChanged.ToUnixTimeMilliseconds().ToString();
        }

        protected virtual XElement[] RessourceType => new []{s_xDavCollection, _addressbook};

        public string Name => _name;

        public string UniqueKey => $"ev:{_id}";

        public virtual Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, CancellationToken cancellationToken)
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

        public static PropertyManager<Guru3Collection> DefaultPropertyManager { get; } =
            new PropertyManager<Guru3Collection>(new DavProperty<Guru3Collection>[]
            {
                new DavGetResourceType<Guru3Collection>
                {
                    Getter = (context, collection) => collection.RessourceType
                },
                new CurrentUserPrincipal<Guru3Collection>
                {
                    Getter = (context, collection) => new XElement(WebDavNamespaces.DavNs + "href", collection.Root)
                },
                new AddressbookHomeSet<Guru3Collection>
                {
                    Getter = (context, collection) => new XElement(WebDavNamespaces.DavNs + "href", collection.Root)
                },
                new AddressbookHomeSetColon<Guru3Collection>
                {
                    Getter = (context, collection) => new XElement(WebDavNamespaces.DavNs + "href", collection.Root)
                },
                new DavDisplayName<Guru3Collection>
                {
                    Getter = (context, collection) => collection._name
                },
                new GetCTag<Guru3Collection>
                {
                    Getter = (context, collection) => collection._etag
                },
                new DavGetEtag<Guru3Collection>
                {
                    Getter = (context, collection) => collection._etag
                }, 
                new DavCurrentUserPrivilegeSet<Guru3Collection>
                {
                    Getter = (context, collection) => _privileges
                },
                new DavSupportedReportSet<Guru3Collection>
                {
                    Getter = (context, collection)=>new []
                    {
                        new XElement(WebDavNamespaces.DavNs + "supported-report", new XElement(WebDavNamespaces.DavNs + "report", "expand-property")), 
                    }
                },
                new AddressbookDescription<Guru3Collection>
                {
                    GetterAsync = (context, collection, cancellationToken) => collection.GetDescriptionAsync(cancellationToken)
                }
            });

        protected virtual Task<string> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return _context.Events.Where(x => x.Id == _id).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual IPropertyManager PropertyManager => DefaultPropertyManager;

        public ILockingManager LockingManager 
        {
            get { throw new NotImplementedException(); }
        }

        public Task<IStoreItem> GetItemAsync(string name, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IList<IStoreItem>> GetItemsAsync(IHttpContext httpContext, CancellationToken cancellationToken)
        {
            var extensions = await _context.Extensions.AsNoTracking()
                .Where(x => x.EventId == _id)
                .Active()
                .Select(x => new {x.Id, x.Number, x.LastChanged})
                .ToListAsync(cancellationToken);
            var result = new List<IStoreItem>();
            foreach (var extension in extensions)
            {
                result.Add(new Guru3Item(extension.Id, extension.Number, extension.LastChanged, _context));
            }
            return result;
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