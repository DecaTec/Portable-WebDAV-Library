using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Xml.Serialization;

namespace DecaTec.WebDav.Uwp.UnitTest
{
    [TestClass]
    public class UnitTestProp
    {
        [TestMethod]
        public void UnitTestPropCreatePropWithMultipleEmptyProperties()
        {
            var serializer = new XmlSerializer(typeof(Prop));

            var prop = Prop.CreatePropWithEmptyProperties("creationdate", "getcontentlanguage", "displayname", "getcontentlength", "getcontenttype", "getlastmodified", "getetag", "source",
                "resourcetype", "contentclass", "defaultdocument", "href", "iscollection", "ishidden", "isreadonly", "isroot", "isstructureddocument", "lastaccessed", "name", "parentname");

            Assert.IsNotNull(prop);
            Assert.IsFalse(prop.CreationDateSpecified);
            Assert.IsNull(prop.LockDiscovery);
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
        }
    }
}
