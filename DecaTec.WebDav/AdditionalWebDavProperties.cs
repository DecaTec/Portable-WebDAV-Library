using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using DecaTec.WebDav.Extensions;
using DecaTec.WebDav.WebDavArtifacts;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing additional WebDAV properties not defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> or the IIS WebDAV specification.
    /// </summary>
    public class AdditionalWebDavProperties
    {
        private readonly IDictionary<XName, string> additionalPropertiesInternal;
        // For saving the original state of the list of additional properties.
        private readonly IDictionary<XName, string> additionalPropertiesInternalOriginal;

        /// <summary>
        /// Creates a new instance of AdditionalWebDavProperties with the additional properties specified by an <see cref="XElement"/> array.
        /// </summary>
        /// <param name="elements"></param>
        public AdditionalWebDavProperties(XElement[] elements)
        {
            this.additionalPropertiesInternal = elements.ToXNameDictonary();
            this.additionalPropertiesInternalOriginal = new Dictionary<XName, string>(this.additionalPropertiesInternal);
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set (as string).</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a KeyNotFoundException, and a set operation creates a new element with the specified key.</returns>
        /// <remarks>The string key can either be name of the property without namespace (but only if there is no other property with the same name and other namespace present), 
        /// or the full qualified name in the form '{namespace}key' (e.g. '{http://owncloud.org/ns}favorite').</remarks>
        public string this[string key]
        {
            get
            {
                // Try to get with key only.
                var property = additionalPropertiesInternal.Where(x => string.CompareOrdinal(x.Key.LocalName, key) == 0);
                var keyValuePairs = property as KeyValuePair<XName, string>[] ?? property.ToArray();

                if (keyValuePairs.Length == 1)
                {
                    // Key exists only once, namespace does not matter.
                    return keyValuePairs.First().Value;
                }
                else if (keyValuePairs.Length > 1 && !(key.Contains("{") && key.Contains("}")))
                {
                    // Key exists multiple times, no namespace was given.
                    throw new InvalidOperationException($"The key '{key}' exists multiple times in the AdditionalWebDavProperties (with different namespaces). Try to request the element with '{{namespace}}key' or use the getter accepting an XName.");
                }

                // Try to get with '{namespace}key'.
                return this[XName.Get(key)];
            }
            set
            {
                // Try to get with namespace ignored.
                var property = additionalPropertiesInternal.Where(x => string.CompareOrdinal(x.Key.LocalName, key) == 0);
                var keyValuePairs = property as KeyValuePair<XName, string>[] ?? property.ToArray();

                if (!keyValuePairs.Any())
                {
                    // Add by using the name only.
                    additionalPropertiesInternal[key] = value;
                }
                else if (keyValuePairs.Length == 1)
                {
                    // Edit by using namespace and key.
                    additionalPropertiesInternal[keyValuePairs.First().Key] = value;
                }
                else
                {
                    // There are multiple entries with different namespaces, it is not clear which one to edit.
                    throw new InvalidOperationException($"Cannot set element with the key '{key}' because it exists multiple times in the AdditionalWebDavProperties (with different namespaces) and it is not clear which element is affected by the setter. Try to set the element with '{{namespace}}key' or use the setter accepting an XName.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="xNameKey">The key of the value to get or set (as <see cref="XName"/>).</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a KeyNotFoundException, and a set operation creates a new element with the specified key.</returns>
        public string this[XName xNameKey]
        {
            get
            {
                var properties = additionalPropertiesInternal.Where(x => x.Key == xNameKey);

                if (!properties.Any())
                    throw new KeyNotFoundException($"The key '{xNameKey}' cannot be found in the AdditionalWebDavProperties.");

                return additionalPropertiesInternal.Single(x => x.Key == xNameKey).Value;
            }
            set
            {
                additionalPropertiesInternal[xNameKey] = value;
            }
        }

        /// <summary>
        /// Determines whether the AdditionalWebDavProperties contains the key (as string) specified.
        /// </summary>
        /// <param name="key">The key to locate in the AdditionalWebDavProperties.</param>
        /// <returns>True, if the AdditionalWebDavProperties contain an element withe the specified key, otherwise false.</returns>
        public bool ContainsKey(string key)
        {
            // Try to get with key only.
            var property = additionalPropertiesInternal.Where(x => string.CompareOrdinal(x.Key.LocalName, key) == 0);

            if (property.Count() == 1)
                return true;

            // Try to get with '{namespace}:key'.
            return ContainsKey(XName.Get(key));
        }

        /// <summary>
        /// Determines whether the AdditionalWebDavProperties contains the key (as <see cref="XName"/>) specified.
        /// </summary>
        /// <param name="key">The key to locate in the AdditionalWebDavProperties.</param>
        /// <returns>True, if the AdditionalWebDavProperties contain an element withe the specified key, otherwise false.</returns>
        public bool ContainsKey(XName key)
        {
            return this.additionalPropertiesInternal.ContainsKey(key);
        }

        /// <summary>
        /// Gets a list of <see cref="XElement"/> with changed and added properties.
        /// </summary>
        /// <returns>A list of <see cref="XElement"/> with changed and added properties.</returns>
        /// <remarks>When used in a <see cref="WebDavSessionItem"/>, additional properties can be changed, added or removed. This method is then used when it comes to a PROPPATCH 
        /// in order to determine which <see cref="Prop"/>s need to be set for the <see cref="PropertyUpdate"/>.</remarks>
        public IList<XElement> GetChangedAndAddedProperties()
        {
            // Changed/added when:
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
                        xElementList.Add(new XElement(changedProperty.Key, changedProperty.Value));
                    }
                }
            }

            return xElementList;
        }

        /// <summary>
        /// Gets a list of <see cref="XElement"/> properties which were removed.
        /// </summary>
        /// <returns>A list of <see cref="XElement"/> with properties which were removed.</returns>
        /// <remarks>When used in a <see cref="WebDavSessionItem"/>, additional properties can be changed, added or removed. This method is then used when it comes to a PROPPATCH 
        /// in order to determine which <see cref="Prop"/>s need to be set for the <see cref="PropertyUpdate"/>.</remarks>
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

        /// <summary>
        /// Gets a value indicating if the AdditionalWebDavProperties have changed.
        /// </summary>
        public bool HasChanged
        {
            get
            {
                var equal = false;

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