using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Xml.Serialization;

namespace DecaTec.WebDav.Uwp.UnitTest
{
    [TestClass]
    public class UnitTestProp
    {
        [TestMethod]
        public void UT_UWP_Prop_CreateProp_WithMultipleEmptyPropertiesAithStrings()
        {
            var serializer = new XmlSerializer(typeof(Prop));

            var prop = Prop.CreatePropWithEmptyProperties("creationdate", "getcontentlanguage", "displayname", "getcontentlength", "getcontenttype", "getlastmodified", "getetag", "source",
                "resourcetype", "contentclass", "defaultdocument", "href", "iscollection", "ishidden", "isreadonly", "isroot", "isstructureddocument", "lastaccessed", "name", "parentname", "lockdiscovery");

            Assert.IsNotNull(prop);
            Assert.IsFalse(prop.CreationDateSpecified);
            Assert.AreEqual(prop.CreationDate, string.Empty);
            Assert.AreEqual(prop.GetContentLanguage, string.Empty);
            Assert.AreEqual(prop.DisplayName, string.Empty);
            Assert.AreEqual(prop.GetContentLength, string.Empty);
            Assert.AreEqual(prop.GetContentType, string.Empty);
            Assert.AreEqual(prop.GetLastModified, string.Empty);
            Assert.AreEqual(prop.GetEtag, string.Empty);
            Assert.IsNotNull(prop.Source);
            Assert.IsNotNull(prop.ResourceType);
            Assert.AreEqual(prop.ContentClass, string.Empty);
            Assert.AreEqual(prop.DefaultDocument, string.Empty);
            Assert.AreEqual(prop.Href, string.Empty);
            Assert.AreEqual(prop.IsCollection, string.Empty);
            Assert.AreEqual(prop.IsHidden, string.Empty);
            Assert.AreEqual(prop.IsReadonly, string.Empty);
            Assert.AreEqual(prop.IsRoot, string.Empty);
            Assert.AreEqual(prop.IsStructuredDocument, string.Empty);
            Assert.AreEqual(prop.LastAccessed, string.Empty);
            Assert.AreEqual(prop.Name, string.Empty);
            Assert.AreEqual(prop.ParentName, string.Empty);
            Assert.IsNotNull(prop.LockDiscovery);
        }

        [TestMethod]
        public void UT_UWP_Prop_CreatePropWithMultipleEmptyPropertiesWithConstants()
        {
            var serializer = new XmlSerializer(typeof(Prop));

            var prop = Prop.CreatePropWithEmptyProperties(PropNameConstants.CreationDate, PropNameConstants.GetContentLanguage, PropNameConstants.DisplayName, PropNameConstants.GetContentLength,
                PropNameConstants.GetContentType, PropNameConstants.GetLastModified, PropNameConstants.GetEtag, PropNameConstants.Source, PropNameConstants.ResourceType, PropNameConstants.ContentClass,
                PropNameConstants.DefaultDocument, PropNameConstants.Href, PropNameConstants.IsCollection, PropNameConstants.IsHidden, PropNameConstants.IsReadonly, PropNameConstants.IsRoot,
                PropNameConstants.IsStructuredDocument, PropNameConstants.LastAccessed, PropNameConstants.Name, PropNameConstants.ParentName, PropNameConstants.LockDiscovery);

            Assert.IsNotNull(prop);
            Assert.IsFalse(prop.CreationDateSpecified);
            Assert.AreEqual(prop.CreationDate, string.Empty);
            Assert.AreEqual(prop.GetContentLanguage, string.Empty);
            Assert.AreEqual(prop.DisplayName, string.Empty);
            Assert.AreEqual(prop.GetContentLength, string.Empty);
            Assert.AreEqual(prop.GetContentType, string.Empty);
            Assert.AreEqual(prop.GetLastModified, string.Empty);
            Assert.AreEqual(prop.GetEtag, string.Empty);
            Assert.IsNotNull(prop.Source);
            Assert.IsNotNull(prop.ResourceType);
            Assert.AreEqual(prop.ContentClass, string.Empty);
            Assert.AreEqual(prop.DefaultDocument, string.Empty);
            Assert.AreEqual(prop.Href, string.Empty);
            Assert.AreEqual(prop.IsCollection, string.Empty);
            Assert.AreEqual(prop.IsHidden, string.Empty);
            Assert.AreEqual(prop.IsReadonly, string.Empty);
            Assert.AreEqual(prop.IsRoot, string.Empty);
            Assert.AreEqual(prop.IsStructuredDocument, string.Empty);
            Assert.AreEqual(prop.LastAccessed, string.Empty);
            Assert.AreEqual(prop.Name, string.Empty);
            Assert.AreEqual(prop.ParentName, string.Empty);
            Assert.IsNotNull(prop.LockDiscovery);
        }
    }
}
