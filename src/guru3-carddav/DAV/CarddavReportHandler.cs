using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NWebDav.Server;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Logging;
using NWebDav.Server.Props;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav.DAV
{
    public class CarddavReportHandler : IRequestHandler
    {
        private struct PropertyEntry
        {
            public Uri Uri { get; }
            public IStoreItem Entry { get; }

            public PropertyEntry(Uri uri, IStoreItem entry)
            {
                Uri = uri;
                Entry = entry;
            }
        }

        [Flags]
        private enum PropertyMode
        {
            None = 0,
            PropertyNames = 1,
            AllProperties = 2,
            SelectedProperties = 4
        }
        
        private static readonly ILogger s_log = LoggerFactory.CreateLogger(typeof(CarddavReportHandler));

        public Task<bool> HandleRequestAsync(IHttpContext httpContext, IStore store, CancellationToken cancellationToken)
        {
            // Obtain request and response
            var request = httpContext.Request;
            
            var xDocument = request.LoadXmlDocument();
            if (xDocument?.Root == null)
                throw new NotImplementedException();

            if (xDocument.Root.Name == CardDavNamespace.CardDavNs + "addressbook-query")
                return AddressbookQueryAsync(cancellationToken);
            if (xDocument.Root.Name == CardDavNamespace.CardDavNs + "addressbook-multiget")
                return AddressbookMultigetAsync(store, httpContext, xDocument.Root, cancellationToken);

            throw new NotImplementedException();
        }

        private Task<bool> AddressbookQueryAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> AddressbookMultigetAsync(IStore store, IHttpContext httpContext, XElement report, CancellationToken cancellationToken)
        {
            // Obtain entry
            var request = httpContext.Request;
            var response = httpContext.Response;

            var topEntry = await store.GetItemAsync(request.Url, httpContext, cancellationToken).ConfigureAwait(false);
            if (topEntry == null)
            {
                response.SetStatus(DavStatusCode.NotFound);
                return true;
            }

            PropertyMode propertyMode = PropertyMode.AllProperties;
            var propertyList = new List<XName>();
            foreach (var xProp in report.Elements())
            {
                // Check if we should fetch all property names
                if (xProp.Name == WebDavNamespaces.DavNs + "propname")
                {
                    propertyMode = PropertyMode.PropertyNames;
                }
                else if (xProp.Name == WebDavNamespaces.DavNs + "allprop")
                {
                    propertyMode = PropertyMode.AllProperties;
                }
                else if (xProp.Name == WebDavNamespaces.DavNs + "prop")
                {
                    propertyMode = PropertyMode.SelectedProperties;

                    // Include all specified properties
                    foreach (var xSubProp in xProp.Elements())
                        propertyList.Add(xSubProp.Name);
                }
            }
            
            // Generate the list of items from which we need to obtain the properties
            var entries = new List<PropertyEntry>();

            if (topEntry is IStoreCollection topCollection)
            {
                foreach (var href in report.Elements(WebDavNamespaces.DavNs + "href"))
                {
                    var uri = new Uri(request.Url, href.Value);
                    if (!request.Url.IsBaseOf(uri))
                    {
                        //need to be members (not necessarily internal members) of the resource identified by the Request-URI
                        continue;
                    }
                    var item = await store.GetItemAsync(uri, httpContext, cancellationToken).ConfigureAwait(false);
                    entries.Add(new PropertyEntry(uri, item));
                }
            }
            else
            {
                entries.Add(new PropertyEntry(request.Url, topEntry));
            }
            
            // Obtain the status document
            var xMultiStatus = new XElement(WebDavNamespaces.DavNs + "multistatus");
            var xDocument = new XDocument(xMultiStatus);

            // Add all the properties
            foreach (var entry in entries)
            {
                // Create the property
                var xResponse = new XElement(WebDavNamespaces.DavNs + "response",
                    new XElement(WebDavNamespaces.DavNs + "href", UriHelper.ToEncodedString(entry.Uri)));

                // Create tags for property values
                var xPropStatValues = new XElement(WebDavNamespaces.DavNs + "propstat");

                if (entry.Entry == null)
                {
                    xPropStatValues.Add(new XElement(WebDavNamespaces.DavNs + "status", "HTTP/1.1 404 Not Found"));
                    xResponse.Add(xPropStatValues);
                }
                else
                {
                    // Check if the entry supports properties
                    var propertyManager = entry.Entry.PropertyManager;
                    if (propertyManager != null)
                    {
                        // Handle based on the property mode
                        if (propertyMode == PropertyMode.PropertyNames)
                        {
                            // Add all properties
                            foreach (var property in propertyManager.Properties)
                                xPropStatValues.Add(new XElement(property.Name));

                            // Add the values
                            xResponse.Add(xPropStatValues);
                        }
                        else
                        {
                            var addedProperties = new List<XName>();
                            if ((propertyMode & PropertyMode.AllProperties) != 0)
                            {
                                foreach (var propertyName in propertyManager.Properties.Where(p => !p.IsExpensive)
                                    .Select(p => p.Name))
                                    await AddPropertyAsync(httpContext, xResponse, xPropStatValues, propertyManager,
                                            entry.Entry, propertyName, addedProperties, cancellationToken)
                                        .ConfigureAwait(false);
                            }

                            if ((propertyMode & PropertyMode.SelectedProperties) != 0)
                            {
                                foreach (var propertyName in propertyList)
                                    await AddPropertyAsync(httpContext, xResponse, xPropStatValues, propertyManager,
                                            entry.Entry, propertyName, addedProperties, cancellationToken)
                                        .ConfigureAwait(false);
                            }

                            // Add the values (if any)
                            if (xPropStatValues.HasElements)
                                xResponse.Add(xPropStatValues);
                        }
                    }

                    // Add the status
                    xPropStatValues.Add(new XElement(WebDavNamespaces.DavNs + "status", "HTTP/1.1 200 OK"));
                }
                // Add the property
                xMultiStatus.Add(xResponse);
            }

            // Stream the document
            await response.SendResponseAsync(DavStatusCode.MultiStatus, xDocument, cancellationToken).ConfigureAwait(false);

            // Finished writing
            return true;
        }

        private async Task AddPropertyAsync(IHttpContext httpContext, XElement xResponse, XElement xPropStatValues, IPropertyManager propertyManager, IStoreItem item, XName propertyName, IList<XName> addedProperties, CancellationToken cancellationToken)
        {
            if (!addedProperties.Contains(propertyName))
            {
                addedProperties.Add(propertyName);
                try
                {
                    // Check if the property is supported
                    if (propertyManager.Properties.Any(p => p.Name == propertyName))
                    {
                        var value = await propertyManager.GetPropertyAsync(httpContext, item, propertyName, false, cancellationToken).ConfigureAwait(false);
                        if (value is IEnumerable<XElement>)
                            value = ((IEnumerable<XElement>) value).Cast<object>().ToArray();

                        // Make sure we use the same 'prop' tag to add all properties
                        var xProp = xPropStatValues.Element(WebDavNamespaces.DavNs + "prop");
                        if (xProp == null)
                        {
                            xProp = new XElement(WebDavNamespaces.DavNs + "prop");
                            xPropStatValues.Add(xProp);
                        }

                        xProp.Add(new XElement(propertyName, value));
                    }
                    else
                    {
                        s_log.Log(LogLevel.Warning, () => $"Property {propertyName} is not supported on item {item.Name}.");
                        xResponse.Add(new XElement(WebDavNamespaces.DavNs + "propstat",
                            new XElement(WebDavNamespaces.DavNs + "prop", new XElement(propertyName, null)),
                            new XElement(WebDavNamespaces.DavNs + "status", "HTTP/1.1 404 Not Found"),
                            new XElement(WebDavNamespaces.DavNs + "responsedescription", $"Property {propertyName} is not supported.")));
                    }
                }
                catch (Exception exc)
                {
                    s_log.Log(LogLevel.Error, () => $"Property {propertyName} on item {item.Name} raised an exception.", exc);
                    xResponse.Add(new XElement(WebDavNamespaces.DavNs + "propstat",
                        new XElement(WebDavNamespaces.DavNs + "prop", new XElement(propertyName, null)),
                        new XElement(WebDavNamespaces.DavNs + "status", "HTTP/1.1 500 Internal server error"),
                        new XElement(WebDavNamespaces.DavNs + "responsedescription", $"Property {propertyName} on item {item.Name} raised an exception.")));
                }
            }
        }
    }
}