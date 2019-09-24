using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eventphone.guru3.carddav.DAL;
using Microsoft.EntityFrameworkCore;
using NWebDav.Server;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class Guru3Item : IStoreItem
    {
        private readonly int _id;
        private readonly string _number;
        private readonly string _etag;
        private readonly Guru3Context _context;

        public Guru3Item(int id, string number, DateTimeOffset lastChanged, Guru3Context context)
        {
            _id = id;
            _number = number;
            _etag = lastChanged.ToUnixTimeMilliseconds().ToString();
            _context = context;
        }

        public string Name => _number;

        public string UniqueKey => $"ex:{_id}";

        public async Task<Stream> GetReadableStreamAsync(IHttpContext httpContext, CancellationToken cancellationToken)
        {
            var vcard = await GetVCardAsync(cancellationToken);
            return new MemoryStream(Encoding.UTF8.GetBytes(vcard));
        }

        private async Task<string> GetVCardAsync(CancellationToken cancellationToken)
        {
            var extension = await _context.Extensions.AsNoTracking()
                .Active()
                .Where(x => x.Id == _id)
                .Select(x => new {x.Name, x.Location, x.Language})
                .FirstOrDefaultAsync(cancellationToken);
            if (extension == null) return String.Empty;
            return "BEGIN:VCARD\n" +
                        "VERSION:4.0\n" +
                        $"N:{extension.Name.Escape()}\n" +
                        $"TEL:{_number}\n" +
                        $"LANG:{extension.Language}\n" +
                        $"ADR:{extension.Location.Escape()}\n" +
                        "END:VCARD";
        }

        public Task<DavStatusCode> UploadFromStreamAsync(IHttpContext httpContext, Stream source, CancellationToken cancellationToken)
        {
            return Task.FromResult(DavStatusCode.NotImplemented);
        }

        public Task<StoreItemResult> CopyAsync(IStoreCollection destination, string name, bool overwrite, IHttpContext httpContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new StoreItemResult(DavStatusCode.NotImplemented));
        }

        public IPropertyManager PropertyManager => DefaultPropertyManager;

        public ILockingManager LockingManager
        {
            get { throw new NotImplementedException(); }
        }

        public static PropertyManager<Guru3Item> DefaultPropertyManager { get; } =
            new PropertyManager<Guru3Item>(new DavProperty<Guru3Item>[]
            {
                new DavGetResourceType<Guru3Item>
                {
                    Getter = (context, collection) => null
                },
                new DavGetEtag<Guru3Item>
                {
                    Getter = (context, item) => item._etag
                }, 
                new DavGetContentType<Guru3Item>
                {
                    Getter = (context, item) => "text/vcard"
                },
                new AddressData<Guru3Item>
                {
                    IsExpensive = true,
                    GetterAsync = (context, item, cancellationToken) => item.GetVCardAsync(cancellationToken)
                }, 
            });
    }
}