using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using DecaTec.WebDav.Extensions;

namespace DecaTec.WebDav
{ 
    public class AdditionalWebDavProperties
    {
        private readonly IDictionary<XName, string> additionalPropertiesInternal;
        // For saving the original state of the list of additional properties.
        private readonly IDictionary<XName, string> additionalPropertiesInternalOriginal;

        public AdditionalWebDavProperties(XElement[] elements)
        {
            // Save additional properties as Dictionary.
            this.additionalPropertiesInternal = elements.ToXNameDictonary();
            this.additionalPropertiesInternalOriginal = new Dictionary<XName, string>(this.additionalPropertiesInternal);
        }

        public string this[string key]
        {
            get
            {
                // Try to get with key only.
                var property = additionalPropertiesInternal.Where(x => string.CompareOrdinal(x.Key.LocalName, key) == 0);

                if (property.Count() == 1)
                    return property.First().Value;

                // Try to get with '{namespace}:{key}'
                var splitIndex = key.LastIndexOf(':');

                if (splitIndex != -1)
                { 
                    var ns = key.Substring(0, splitIndex);
                    var localName = key.Substring(splitIndex + 1);
                    var xName = XName.Get(localName, ns);
                    return this[xName];
                }
                else
                {
                    throw new InvalidOperationException($"The key '{key}' exists multiple times in the AdditionalWebDavProperties (with different namespaces). Try to request the element with '{{namespace}}:{{key}}' or use the getter accepting an XName.");
                }
            }
            set
            {
                // Try to get with namespace ignored.
                var property = additionalPropertiesInternal.Where(x => string.CompareOrdinal(x.Key.LocalName, key) == 0);

                if (property.Count() == 0)
                {
                    // Add/edit by using the name only.
                    additionalPropertiesInternal[key] = value;
                }
                else if (property.Count() == 1)
                {
                    // Edit by using namespace and key.
                    additionalPropertiesInternal[property.First().Key] = value;
                }
                else
                {
                    // There are multiple entries with different namespaces, it is not clear which one to edit.
                    throw new InvalidOperationException($"Cannot set element with the key '{key}' because it exists multiple times in the AdditionalWebDavProperties (with different namespaces) and it is not clear which element is affected by the setter. Try to set the element with '{{namespace}}:{{key}}' or use the setter accepting an XName.");
                }
            }
        }

        public string this[XName xNameKey]
        {
            get
            {
                return additionalPropertiesInternal.Single(x => x.Key == xNameKey).Value;
            }
            set
            {
                additionalPropertiesInternal[xNameKey] = value;
            }
        }

        public IList<XElement> GetChangedAndAddedProperties()
        {
            // Set for additional properties when:
            // - a property in the dictionary was changed (and is not null now).
            // - an element was added to the dictionary.
            var xElementList = new List<XElement>();

            if (this.HasChanged)
            {
                foreach (var propertyOriginal in this.additionalPropertiesInternalOriginal)
                {
                    var changedProperty = this.additionalPropertiesInternal.SingleOrDefault(x => x.Key == propertyOriginal.Key);

                    if (changedProperty.Equals(default(KeyValuePair<XName, string>)) || (propertyOriginal.Value != null && string.CompareOrdinal(changedProperty.Value, propertyOriginal.Value) != 0))
                    {
                        // Property was changed or added.
                        xElementList.Add(new XElement(changedProperty.Key, changedProperty.Value));
                    }
                }                
            }

            return xElementList;
        }

        public IList<XElement> GetRemovedProperties()
        {
            // The property is to remove if it is was deleted on the source dictionary or is null.
            var xElementList = new List<XElement>();

            if (this.HasChanged)
            {
                foreach (var property in additionalPropertiesInternalOriginal)
                {
                    var changedProperty = additionalPropertiesInternal.SingleOrDefault(x => x.Key == property.Key);

                    if (changedProperty.Equals(default(KeyValuePair<XName, string>)))
                    {
                        // Property was set to null.
                        xElementList.Add(new XElement(property.Key, string.Empty));
                    }
                    else if (changedProperty.Value == null)
                    {
                        xElementList.Add(new XElement(changedProperty.Key, string.Empty));
                    }
                }
            }

            return xElementList;
        }

        public bool HasChanged
        {
            get
            {
                bool equal = false;

                if (this.additionalPropertiesInternal.Count == this.additionalPropertiesInternalOriginal.Count)
                {
                    equal = true;

                    foreach (var pair in additionalPropertiesInternal)
                    {
                        if (additionalPropertiesInternalOriginal.TryGetValue(pair.Key, out string value))
                        {
                            if (string.CompareOrdinal(value, pair.Value) != 0)
                            {
                                equal = false;
                                break;
                            }
                        }
                        else
                        {
                            equal = false;
                            break;
                        }
                    }
                }

                return !equal;
            }
        }
    }
}
