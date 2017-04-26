using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using DecaTec.WebDav.WebDavArtifacts;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestProp
    {
        [TestMethod]
        public void UT_Prop_CreatePropWithEmptyProperties_MultipleEmptyPropertiesWithStrings()
        {
            var serializer = new XmlSerializer(typeof(Prop));

            var prop = Prop.CreatePropWithEmptyProperties("creationdate", "getcontentlanguage", "displayname", "getcontentlength", "getcontenttype", "getlastmodified", "getetag",
                "resourcetype", "contentclass", "defaultdocument", "href", "isfolder", "ishidden", "isreadonly", "isroot", "isstructureddocument", "lastaccessed", "name", "parentname", "lockdiscovery");

            Assert.IsNotNull(prop);
            Assert.AreEqual(prop.CreationDateString, string.Empty);
            Assert.AreEqual(prop.GetContentLanguage, string.Empty);
            Assert.AreEqual(prop.DisplayName, string.Empty);
            Assert.AreEqual(prop.GetContentLengthString, string.Empty);
            Assert.AreEqual(prop.GetContentType, string.Empty);
            Assert.AreEqual(prop.GetLastModifiedString, string.Empty);
            Assert.AreEqual(prop.GetEtag, string.Empty);
            Assert.IsNotNull(prop.ResourceType);
            Assert.AreEqual(prop.ContentClass, string.Empty);
            Assert.AreEqual(prop.DefaultDocument, string.Empty);
            Assert.AreEqual(prop.HrefString, string.Empty);
            Assert.AreEqual(prop.IsFolderString, string.Empty);
            Assert.AreEqual(prop.IsHiddenString, string.Empty);
            Assert.AreEqual(prop.IsReadonlyString, string.Empty);
            Assert.AreEqual(prop.IsRootString, string.Empty);
            Assert.AreEqual(prop.IsStructuredDocumentString, string.Empty);
            Assert.AreEqual(prop.LastAccessedString, string.Empty);
            Assert.AreEqual(prop.Name, string.Empty);
            Assert.AreEqual(prop.ParentName, string.Empty);
            Assert.IsNotNull(prop.LockDiscovery);
        }

        [TestMethod]
        public void UT_Prop_CreatePropWithEmptyProperties_MultipleEmptyPropertiesWithConstants()
        {
            var serializer = new XmlSerializer(typeof(Prop));

            var prop = Prop.CreatePropWithEmptyProperties(PropNameConstants.CreationDate, PropNameConstants.GetContentLanguage, PropNameConstants.DisplayName, PropNameConstants.GetContentLength,
                PropNameConstants.GetContentType, PropNameConstants.GetLastModified, PropNameConstants.GetEtag, PropNameConstants.ResourceType, PropNameConstants.ContentClass,
                PropNameConstants.DefaultDocument, PropNameConstants.Href, PropNameConstants.IsFolder, PropNameConstants.IsHidden, PropNameConstants.IsReadonly, PropNameConstants.IsRoot,
                PropNameConstants.IsStructuredDocument, PropNameConstants.LastAccessed, PropNameConstants.Name, PropNameConstants.ParentName, PropNameConstants.LockDiscovery);

            Assert.IsNotNull(prop);
            Assert.AreEqual(prop.CreationDateString, string.Empty);
            Assert.AreEqual(prop.GetContentLanguage, string.Empty);
            Assert.AreEqual(prop.DisplayName, string.Empty);
            Assert.AreEqual(prop.GetContentLengthString, string.Empty);
            Assert.AreEqual(prop.GetContentType, string.Empty);
            Assert.AreEqual(prop.GetLastModifiedString , string.Empty);
            Assert.AreEqual(prop.GetEtag, string.Empty);
            Assert.IsNotNull(prop.ResourceType);
            Assert.AreEqual(prop.ContentClass, string.Empty);
            Assert.AreEqual(prop.DefaultDocument, string.Empty);
            Assert.AreEqual(prop.HrefString, string.Empty);
            Assert.AreEqual(prop.IsFolderString, string.Empty);
            Assert.AreEqual(prop.IsHiddenString, string.Empty);
            Assert.AreEqual(prop.IsReadonlyString, string.Empty);
            Assert.AreEqual(prop.IsRootString, string.Empty);
            Assert.AreEqual(prop.IsStructuredDocumentString, string.Empty);
            Assert.AreEqual(prop.LastAccessedString, string.Empty);
            Assert.AreEqual(prop.Name, string.Empty);
            Assert.AreEqual(prop.ParentName, string.Empty);
            Assert.IsNotNull(prop.LockDiscovery);
        }
    }
}
