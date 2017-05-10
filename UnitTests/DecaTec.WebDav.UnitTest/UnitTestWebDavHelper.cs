using DecaTec.WebDav.Tools;
using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestWebDavHelper
    {
        [TestMethod]
        public void UT_WebDavHelper_GetUtf8EncodedXmlWebDavRequestString()
        {
            var serializer = new XmlSerializer(typeof(PropFind));
            var propFind = PropFind.CreatePropFindAllProp();
            var str = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(serializer, propFind);
            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void UT_WebDavHelper_GetUtf8EncodedXmlWebDavRequestStringFromPropWithXmlLangAttribute()
        {
            var serializer = new XmlSerializer(typeof(PropFind));
            var propFind = PropFind.CreatePropFindAllProp();
            var str = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(serializer, propFind);
            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void UT_WebDavHelper_GetUtf8EncodedXmlWebDavRequestString_WithUnsupportedType()
        {
            var serializer = new XmlSerializer(typeof(Prop));

            XNamespace ns = "http://www.adventure-works.com";
            var xElementList = new List<XElement>();
            var xElement = new XElement(ns + "IntProperty", 1000);
            xElementList.Add(xElement);


            var prop = new Prop()
            {
                Language = "en-us",
                DisplayName = "DisplayName",
                AdditionalProperties = xElementList.ToArray()
            };

            var str = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(serializer, prop);
            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:prop xml:lang=\"en-us\" xmlns:D=\"DAV:\"><D:displayname>DisplayName</D:displayname><IntProperty xmlns=\"http://www.adventure-works.com\">1000</IntProperty></D:prop>";

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void UT_WebDavHelper_GetUtf8EncodedXmlWebDavRequestString_WithUnsupportedTypeAndDerivedPropClass()
        {
            var serializer = new XmlSerializer(typeof(MyProp));

            var myProp = new MyProp()
            {
                Language = "en-us",
                IntProperty = 1000
            };

            var kvp = new KeyValuePair<string, string>[1];
            kvp[0] = new KeyValuePair<string, string>("R", "http://ns.example.com/boxschema/");
            var str = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(serializer, myProp, kvp);
            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><R:prop xmlns:D=\"DAV:\" xml:lang=\"en-us\" xmlns:R=\"http://ns.example.com/boxschema/\"><R:intproperty>1000</R:intproperty></R:prop>";

            Assert.AreEqual(expected, str);
        }

        #region Helpers

        [DataContract]
        [XmlType(TypeName = "prop", Namespace = "http://ns.example.com/boxschema/")]
        [XmlRoot(Namespace = "http://ns.example.com/boxschema/", IsNullable = false)]
        public class MyProp : Prop
        {
            [XmlElement(ElementName = "intproperty")]
            public int IntProperty
            {
                get;
                set;
            }
        }

        #endregion Helpers
    }
}
