using System.Xml.Linq;
using NWebDav.Server;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class AddressbookDescription<TEntry>:DavString<TEntry> where TEntry : IStoreItem
    {
        public static readonly XName PropertyName = CardDavNamespace.CardDavNs + "addressbook-description";

        public override XName Name => PropertyName;
    }

    public class CurrentUserPrincipal<TEntry> : DavXElement<TEntry> where TEntry : IStoreItem
    {
        public static readonly XName PropertyName = WebDavNamespaces.DavNs + "current-user-principal";

        public override XName Name => PropertyName;
    }

    public class AddressbookHomeSet<TEntry> : DavXElement<TEntry> where TEntry : IStoreItem
    {
        public static readonly XName PropertyName = CardDavNamespace.CardDavNs + "addressbook-home-set";

        public override XName Name => PropertyName;
    }

    public class AddressbookHomeSetColon<TEntry> : DavXElement<TEntry> where TEntry : IStoreItem
    {
        public static readonly XName PropertyName = CardDavNamespace.CardDavNsColon + "addressbook-home-set";

        public override XName Name => PropertyName;
    }

    public class GetCTag<TEntry> : DavString<TEntry> where TEntry : IStoreCollection
    {
        public static readonly XName PropertyName = CardDavNamespace.CalenderserverNs + "getctag";

        public override XName Name => PropertyName;
    }

    public class DavCurrentUserPrivilegeSet<TEntry> : DavXElement<TEntry> where TEntry : IStoreItem
    {
        public static readonly XName PropertyName = WebDavNamespaces.DavNs + "current-user-privilege-set";

        public override XName Name => PropertyName;
    }

    public class DavSupportedReportSet<TEntry> : DavXElementArray<TEntry> where TEntry : IStoreItem
    {
        public static readonly XName PropertyName = WebDavNamespaces.DavNs + "supported-report-set";

        public override XName Name => PropertyName;
    }

    public class AddressData<TEntry> : DavString<TEntry> where TEntry : IStoreItem
    {
        public static readonly XName PropertyName = CardDavNamespace.CardDavNs + "address-data";

        public override XName Name => PropertyName;
    }
}