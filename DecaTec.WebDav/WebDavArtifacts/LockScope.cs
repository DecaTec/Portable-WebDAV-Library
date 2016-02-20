using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'lockscope' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [XmlType(TypeName = "lockscope", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class LockScope
    {
        /// <summary>
        /// Creates a LockScope (exclusive).
        /// </summary>
        /// <returns>The exclusive Lockscope.</returns>
        public static LockScope CreateExclusiveLockScope()
        {
            var lockScope = new LockScope();
            lockScope.Item = new Exclusive();
            return lockScope;
        }

        /// <summary>
        /// Creates a LockScope (shared).
        /// </summary>
        /// <returns>The shared LockScope.</returns>
        public static LockScope CreateSharedLockScope()
        {
            var lockScope = new LockScope();
            lockScope.Item = new Shared();
            return lockScope;
        }

        private object itemField;

        /// <summary>
        /// Gets opr sets the Item.
        /// </summary>
        [XmlElement(ElementName = "exclusive", Type = typeof(Exclusive))]
        [XmlElement(ElementName = "shared", Type = typeof(Shared))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }
}
