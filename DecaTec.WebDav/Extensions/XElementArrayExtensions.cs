using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DecaTec.WebDav.Extensions
{
    /// <summary>
    /// Extensions for <see cref="XElement"/> arrays.
    /// </summary>
    public static class XElementArrayExtensions
    {
        /// <summary>
        /// Creates a Dictionary with <see cref="XName"/> as key from an array of <see cref="XElement"/>.
        /// </summary>
        /// <param name="array">The <see cref="XElement"/> array to convert into a Dictionary.</param>
        /// <returns>A Dictionary containing the elements of the <see cref="XElement"/> array.</returns>
        public static Dictionary<XName, string> ToXNameDictonary(this XElement[] array)
        {
            if (array == null)
                return new Dictionary<XName, string>();

            return array.ToDictionary(element => element.Name, element => element.Value);
        }

        /// <summary>
        /// Creates a Dictionary with string as key from an array of <see cref="XElement"/>.
        /// </summary>
        /// <param name="array">The <see cref="XElement"/> array to convert into a Dictionary.</param>
        /// <returns>A Dictionary containing the elements of the <see cref="XElement"/> array.</returns>
        public static Dictionary<string, string> ToStringDictonary(this XElement[] array)
        {
            if (array == null)
                return new Dictionary<string, string>();

            return array.ToDictionary(element => element.Name.LocalName, element => element.Value);
        }
    }
}
