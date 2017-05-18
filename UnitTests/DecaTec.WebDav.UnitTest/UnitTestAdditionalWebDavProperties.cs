using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestAdditionalWebDavProperties
    {
        [TestMethod]
        public void UT_AdditionalWebDavProperties_GetValue_WithStringIndexer()
        {
            XNamespace ns = "http://owncloud.org/ns";
            var xElement = new XElement(ns + "favorite", "1");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var actual = additionalProperties["favorite"];

            Assert.AreEqual("1", actual);
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_GetValue_WithStringIndexer_WithNamespace()
        {
            XNamespace ns = "http://owncloud.org/ns";
            var xElement = new XElement(ns + "favorite", "1");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var actual = additionalProperties[$"{{http://owncloud.org/ns}}favorite"];

            Assert.AreEqual("1", actual);
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_GetValue_WithXNameIndexer()
        {
            XNamespace ns = "http://owncloud.org/ns";
            var xElement = new XElement(ns + "favorite", "1");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var actual = additionalProperties[xElement.Name];

            Assert.AreEqual("1", actual);
            Assert.IsFalse(additionalProperties.HasChanged);
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_AddProperty_WithXNameIndexer()
        {
            XNamespace ns1 = "http://owncloud.org/ns";
            var xElement1 = new XElement(ns1 + "favorite", "1");

            XNamespace ns2 = "http://owncloud.org/ns2";
            var xElement2 = new XElement(ns2 + "favorite", "2");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement1);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            additionalProperties[xElement2.Name] = "2";
            var actual = additionalProperties[xElement2.Name];

            Assert.AreEqual("2", actual);
            Assert.IsTrue(additionalProperties.HasChanged);
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_GetValue_WithStringIndexer_WithNamespace_MultipleValuesWithDifferentNamespaces()
        {
            XNamespace ns1 = "http://owncloud.org/ns";
            var xElement1 = new XElement(ns1 + "favorite", "1");

            XNamespace ns2 = "http://owncloud.org/ns2";
            var xElement2 = new XElement(ns2 + "favorite", "2");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement1);
            xElementList.Add(xElement2);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var actual = additionalProperties["{" + ns1.NamespaceName + "}" + "favorite"];

            Assert.AreEqual("1", actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UT_AdditionalWebDavProperties_GetValue_WithStringIndexer_ElementNameOnly_MultipleValuesWithDifferentNamespaces_ShouldThrowInvalidOperationException()
        {
            XNamespace ns1 = "http://owncloud.org/ns";
            var xElement1 = new XElement(ns1 + "favorite", "1");

            XNamespace ns2 = "http://owncloud.org/ns2";
            var xElement2 = new XElement(ns2 + "favorite", "2");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement1);
            xElementList.Add(xElement2);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var actual = additionalProperties["favorite"];
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UT_AdditionalWebDavProperties_SetValue_WithStringIndexer_ElementNameOnly_MultipleValuesWithDifferentNamespaces_ShouldThrowInvalidOperationException()
        {
            XNamespace ns1 = "http://owncloud.org/ns";
            var xElement1 = new XElement(ns1 + "favorite", "1");

            XNamespace ns2 = "http://owncloud.org/ns2";
            var xElement2 = new XElement(ns2 + "favorite", "2");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement1);
            xElementList.Add(xElement2);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            additionalProperties["favorite"] = "x";
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void UT_AdditionalWebDavProperties_GetValue_KeyDoesNotExist_WithStringIndexer_ShouldThrowKeyNotFoundException()
        {
            XNamespace ns = "http://owncloud.org/ns";
            var xElement = new XElement(ns + "favorite", "1");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var element = additionalProperties["fav"];
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void UT_AdditionalWebDavProperties_GetValue_KeyDoesNotExist_WithXNameIndexer_ShouldThrowKeyNotFoundException()
        {
            XNamespace ns1 = "http://owncloud.org/ns";
            var xElement1 = new XElement(ns1 + "favorite", "1");

            XNamespace ns2 = "http://owncloud.org/ns2";
            var xElement2 = new XElement(ns2 + "favorite", "2");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement1);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var element = additionalProperties[xElement2.Name];
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_ContainsKey_KeyDoesExist_WithXName()
        {
            XNamespace ns = "http://owncloud.org/ns";
            var xElement = new XElement(ns + "favorite", "1");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var exists = additionalProperties.ContainsKey(xElement.Name);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_ContainsKey_KeyDoesExist_WithString()
        {
            XNamespace ns = "http://owncloud.org/ns";
            string nsString = "http://owncloud.org/ns";
            var xElement = new XElement(ns + "favorite", "1");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var exists = additionalProperties.ContainsKey("{" + nsString + "}" + "favorite");

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_ContainsKey_KeyDoesNotExist_WithXName()
        {
            XNamespace ns1 = "http://owncloud.org/ns";
            var xElement1 = new XElement(ns1 + "favorite", "1");

            XNamespace ns2 = "http://owncloud.org/ns2";
            var xElement2 = new XElement(ns2 + "favorite", "2");

            var xElementList = new List<XElement>();
            xElementList.Add(xElement1);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var exists = additionalProperties.ContainsKey(xElement2.Name);

            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void UT_AdditionalWebDavProperties_ContainsKey_KeyDoesNotExist_WithString()
        {
            XNamespace ns1 = "http://owncloud.org/ns";
            var xElement1 = new XElement(ns1 + "favorite", "1");

            string ns2 = "http://owncloud.org/ns2";

            var xElementList = new List<XElement>();
            xElementList.Add(xElement1);

            var additionalProperties = new AdditionalWebDavProperties(xElementList.ToArray());
            var exists = additionalProperties.ContainsKey("{" + ns2 + "}" + "favorite");

            Assert.IsFalse(exists);
        }
    }
}
