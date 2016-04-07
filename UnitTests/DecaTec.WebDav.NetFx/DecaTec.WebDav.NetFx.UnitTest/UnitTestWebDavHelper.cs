using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestWebDavHelper
    {
        [TestMethod]
        public void UnitTestWebDavHelperGetUtf8EncodedXmlWebDavRequestStringNetFx()
        {
            var serializer = new XmlSerializer(typeof(PropFind));
            var propFind = PropFind.CreatePropFindAllProp();
            var str = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(serializer, propFind);
            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void UnitTestWebDavHelperGetUtf8EncodedXmlWebDavRequestStringWithUnsupportedTypeNetFx()
        {
            var serializer = new XmlSerializer(typeof(MyProp));
            var myProp = new MyProp();
            myProp.IntProperty = 1000;
            var kvp = new KeyValuePair<string, string>[1];
            kvp[0] = new KeyValuePair<string, string>("R", "http://ns.example.com/boxschema/");
            var str = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(serializer, myProp, kvp);
            var expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?><R:prop xmlns:D=\"DAV:\" xmlns:R=\"http://ns.example.com/boxschema/\"><R:intproperty>1000</R:intproperty></R:prop>";

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
