using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;

namespace DecaTec.WebDav.Uwp.UnitTest
{
    [TestClass]
    public class UnitTestUriHelper
    {
        #region AddTrailingSlash

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashNeededUrl()
        {
            string url = "http://www.google.de/test";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = url + "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashNotNeededUrl()
        {
            string url = "http://www.google.de/test/";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashNotNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test/");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_UrlEmpty()
        {
            string url = string.Empty;
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_UrlNull()
        {
            string url = null;
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashTooManySlashesUrl()
        {
            string url = "http://www.google.de//test//";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashTooManySlashesUri()
        {
            var uri = new Uri("http://www.google.de//test//", UriKind.RelativeOrAbsolute);
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            var expected = new Uri("http://www.google.de/test/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashFileUrl()
        {
            string url = "http://www.google.de/test/test.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashFileUri()
        {
            Uri uri = new Uri("http://www.google.de/test/test.txt");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashFileUrlUrlNull()
        {
            string url = null;
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            Assert.AreEqual(@"/", urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashFileUriUriNull()
        {
            Uri uri = null;
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            Assert.AreEqual(new Uri(@"/", UriKind.RelativeOrAbsolute), uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashRelativeUri()
        {
            Uri uri = new Uri("/webdav/test", UriKind.Relative);
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("/webdav/test/", UriKind.Relative);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_TrailingSlashRelativeUrl()
        {
            var url = "/webdav/test";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            var expected = "/webdav/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_UriWithDot()
        {
            var uri = new Uri("http://www.google.de/a.test/file.txt");
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            var expected = new Uri("http://www.google.de/a.test/file.txt/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_UrlWithDot()
        {
            string url = "http://www.google.de/a.test/file.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/a.test/file.txt/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_UriWithDotFileExcpected()
        {
            var uri = new Uri("http://www.google.de/a.test/file.txt");
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            var expected = new Uri("http://www.google.de/a.test/file.txt");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlash_UrlWithDotFileExcpected()
        {
            string url = "http://www.google.de/a.test/file.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            string expected = "http://www.google.de/a.test/file.txt";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        #endregion AddTrailingSlash

        #region CombineUri

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("test2/", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithDoubleBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithBaseUriAndAbsoluteUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithBaseUriAndAbsoluteUriRemoveDuplicatePath()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithBaseUriAndAbsoluteUriNoRemoveDuplicatePath()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, false);
            Uri expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FromSameBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test");
            Uri combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            Uri expected = new Uri("http://www.google.de/test");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FromSameBaseUriAndPath()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test/test2");
            Uri combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            Uri expected = new Uri("http://www.google.de/test/test2");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithoutBaseUri()
        {
            Uri baseUri = null;
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithTrailingSlashWithTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = new Uri("/test2//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FromDifferentBaseUris()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                Uri baseUri = new Uri("http://www.google.de/test");
                Uri baseUri2 = new Uri("http://www.github.com/test2");
                Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, baseUri2, true);
            });
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FromBaseUriAndSlash()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri relativeUri = new Uri("/", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FromBaseUriAndSlashTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de/test//");
            Uri relativeUri = new Uri("//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_RelativeUriFirst()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                Uri baseUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
                Uri relativeUri = new Uri("http://www.google.de/test/");
                Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
                Uri expected = new Uri("http://www.google.de/test/");
                Assert.AreEqual(expected, combinedUri);
            });
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_TwoRelativeUris()
        {
            Uri baseUri = new Uri("/test", UriKind.RelativeOrAbsolute);
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FirstUriEndWithSecondUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            Uri relativeUri = new Uri("/test2/test.txt", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FirstUriEndWithSecondUriWithoutSlashSecondUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithSpacesAndBrackets()
        {
            Uri baseUri = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/");
            Uri relativeUri;

            if (!Uri.TryCreate("/remote.php/webdav/folder/folder%20with%20space%20and%20%28%29/x.mp3", UriKind.RelativeOrAbsolute, out relativeUri))
                Assert.Fail();

            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/x.mp3");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithDuplicatePathEntriesRemoveDuplicate()
        {
            Uri baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");
            Uri relativeUri;

            if (!Uri.TryCreate("/remote.php/webdav/folder-sync/x/folder/", UriKind.RelativeOrAbsolute, out relativeUri))
                Assert.Fail();

            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithDuplicatePathEntries()
        {
            Uri baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");
            Uri relativeUri;

            if (!Uri.TryCreate("/remote.php/webdav/folder-sync/x/folder/", UriKind.RelativeOrAbsolute, out relativeUri))
                Assert.Fail();

            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri);
            Uri expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_WithDuplicatePathEntriesDuplicateRepetition()
        {
            Uri baseUri = new Uri("https://google.de/remote.php/webdav/x/x/");
            Uri relativeUri;

            if (!Uri.TryCreate("/remote.php/webdav/x/x/folder/", UriKind.RelativeOrAbsolute, out relativeUri))
                Assert.Fail();

            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("https://google.de/remote.php/webdav/x/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_TwoRelativeUrisOverlapping()
        {
            var uri1 = new Uri("/test/test2", UriKind.RelativeOrAbsolute);
            var uri2 = new Uri("test2/test3", UriKind.RelativeOrAbsolute);
            var combine = UriHelper.CombineUri(uri1, uri2, true);
            var expected = new Uri("/test/test2/test3", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_FirstUriNull()
        {
            Uri uri1 = null;
            var uri2 = new Uri("/test/test2", UriKind.RelativeOrAbsolute);
            var combine = UriHelper.CombineUri(uri1, uri2, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUri_SecondUriNull()
        {
            var uri1 = new Uri("/test/test2", UriKind.RelativeOrAbsolute);
            Uri uri2 = null;
            var combine = UriHelper.CombineUri(uri1, uri2, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        #endregion CombineUri

        #region CombineUrl

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUrl_FirstUrlStringEmpty()
        {
            var url1 = string.Empty;
            var url2 = "/test/test2";
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUrl_SecondUrlStringEmpty()
        {
            var url1 = "/test/test2/";
            var url2 = string.Empty;
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2/";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUrl_FirstUrlNull()
        {
            string url1 = null;
            var url2 = "/test/test2";
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUrl_SecondUrlNull()
        {
            var url1 = "/test/test2/";
            string url2 = null;
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2/";
            Assert.AreEqual(expected, combine);
        }

        #endregion CombineUrl

        #region GetCombinedUriWithTrailingSlash

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUriWithTrailingSlash_WithoutTrailingSlashWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de");
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUriWithTrailingSlash_WithTrailingSlashWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de");
            Uri relativeUri = new Uri("test2/", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUriWithTrailingSlash_TooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = new Uri("//test2//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUriWithTrailingSlash_BaseUriNull()
        {
            Uri baseUri = null;
            var relativeUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("/test2/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUriWithTrailingSlash_RelativeUriNull()
        {
            var baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = null;
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUriWithTrailingSlash_BothUrisNull()
        {
            Uri baseUri = null;
            Uri relativeUri = null;
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        #endregion GetCombinedUriWithTrailingSlash

        #region GetCombinedUrlWithTrailingSlash

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUrlWithTrailingSlash_WithoutTrailingSlashWithBaseUrl()
        {
            var baseUrl = "http://www.google.de";
            var relativeUrl = "test2";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUrlWithTrailingSlash_WithTrailingSlashWithBaseUrl()
        {
            var baseUrl = "http://www.google.de";
            var relativeUrl = "test2/";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUrlWithTrailingSlash_TooManySlashes()
        {
            var baseUrl = "http://www.google.de//";
            var relativeUrl = "//test2//";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUrlWithTrailingSlash_BaseUrlNull()
        {
            string baseUrl = null;
            var relativeUrl = "/test2";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUrlWithTrailingSlash_RelativeUrlNull()
        {
            var baseUrl = "http://www.google.de//";
            string relativeUrl = null;
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUrlWithTrailingSlash_BothUrlsNull()
        {
            string baseUrl = null;
            string relativeUrl = null;
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = @"/";
            Assert.AreEqual(expected, combinedUrl);
        }

        #endregion GetCombinedUrlWithTrailingSlash

        #region RemovePortFromUri

        [TestMethod]
        public void UT_UWP_UriHelper_RemovePortFromUri()
        {
            Uri uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.RemovePort(uri);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_RemovePortFromUri_UriNull()
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                Uri uri = null;
                var actual = UriHelper.RemovePort(uri);
            });
        }

        #endregion RemovePortFromUri

        #region RemovePortFromUrl

        [TestMethod]
        public void UT_UWP_UriHelper_RemovePortFromUrl()
        {
            string url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.RemovePort(url);
            string expected = @"http://www.google.de/test/test2/test.txt";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_RemovePortFromUrl_UrlEmpty()
        {
            string url = string.Empty;
            var actual = UriHelper.RemovePort(url);
            string expected = string.Empty;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_RemovePortFromUrl_UrlNull()
        {
            string url = null;
            var actual = UriHelper.RemovePort(url);
            string expected = string.Empty;
            Assert.AreEqual(expected, actual);
        }

        #endregion RemovePortFromUrl

        #region SetPortToUri

        [TestMethod]
        public void UT_UWP_UriHelper_SetPortToUri()
        {
            Uri uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.SetPort(uri, 9999);
            Uri expected = new Uri("http://www.google.de:9999/test/test2/test.txt");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_SetPortToUri_RelativeUri()
        {
            Uri uri = new Uri("test/test2/test.txt", UriKind.RelativeOrAbsolute);
            var actual = UriHelper.SetPort(uri, 9999);
            Uri expected = new Uri("test/test2/test.txt", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_SetPortToUri_UriNull()
        {
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                Uri uri = null;
                var actual = UriHelper.SetPort(uri, 9999);
            });
        }

        #endregion SetPortToUri

        #region SetPortToUrl

        [TestMethod]
        public void UT_UWP_UriHelper_SetPortToUrl()
        {
            string url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.SetPort(url, 9999);
            string expected = @"http://www.google.de:9999/test/test2/test.txt";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_SetPortToUrl_UrlEmpty()
        {
            Assert.ThrowsException<UriFormatException>(() =>
            {
                string url = string.Empty;
                var actual = UriHelper.SetPort(url, 9999);
            });
        }

        [TestMethod]
        public void UT_UWP_UriHelper_SetPortToUrl_UrlNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                string url = null;
                var actual = UriHelper.SetPort(url, 9999);
            });
        }

        #endregion SetPortToUrl

        #region GetPortFromUri

        [TestMethod]
        public void UT_UWP_UriHelper_GetPortFromUri()
        {
            Uri uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.GetPort(uri);
            int expected = 8080;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetPortFromUri_RelativeUri()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                Uri uri = new Uri("test/test2/test.txt", UriKind.RelativeOrAbsolute);
                var actual = UriHelper.GetPort(uri);
            });
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetPortFromUri_UriNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Uri uri = null;
                var actual = UriHelper.GetPort(uri);
            });
        }

        #endregion GetPortFromUri

        #region GetPortFromUrl

        [TestMethod]
        public void UT_UWP_UriHelper_GetPortFromUrl()
        {
            string url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.GetPort(url);
            var expected = 8080;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetPortFromUrl_UrlEmpty()
        {
            Assert.ThrowsException<UriFormatException>(() =>
            {
                string url = string.Empty;
                var actual = UriHelper.GetPort(url);
            });
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetPortFromUrl_UrlNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                string url = null;
                var actual = UriHelper.GetPort(url);
            });
        }

        #endregion GetPortFromUrl       
    }
}
