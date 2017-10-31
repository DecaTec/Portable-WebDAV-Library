using DecaTec.WebDav.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestUriHelper
    {
        #region CreateUriFromUrl

        [TestMethod]
        public void UT_UriHelper_CreateUriFromUrl_WithAbsoluteUri()
        {
            var url = "http://google.de/test";
            var actual = UriHelper.CreateUriFromUrl(url);
            var expected = new Uri(url, UriKind.Absolute);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void UT_UriHelper_CreateUriFromUrl_WithRelativeUri()
        {
            var url = "/test";
            var actual = UriHelper.CreateUriFromUrl(url);
            var expected = new Uri(url, UriKind.Relative);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_CreateUriFromUrl_WithAbsoluteFileUri()
        {
            var url = "file:///test";
            var actual = UriHelper.CreateUriFromUrl(url);
            var expected = new Uri("/test", UriKind.Relative);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_CreateUriFromUrl_WithAbsoluteFileUriWithSpaces()
        {
            var url = "file:///nextcloud/remote.php/webdav/Test%20Folder/";
            var actual = UriHelper.CreateUriFromUrl(url);
            var expected = new Uri("/nextcloud/remote.php/webdav/Test Folder/", UriKind.Relative);
            Assert.AreEqual(expected, actual);
        }

        #endregion CreateUriFromUrl

        #region TryCreateUriFromUrl

        [TestMethod]
        public void UT_UriHelper_TryCreateUriFromUrl_WithAbsoluteUri()
        {
            var url = "http://google.de/test";
            var result = UriHelper.TryCreateUriFromUrl(url, out Uri actual);
            var expected = new Uri(url, UriKind.Absolute);
            Assert.IsTrue(result);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void UT_UriHelper_TryCreateUriFromUrl_WithRelativeUri()
        {
            var url = "/test";
            var result = UriHelper.TryCreateUriFromUrl(url, out Uri actual);
            var expected = new Uri(url, UriKind.Relative);
            Assert.IsTrue(result);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_TryCreateUriFromUrl_WithAbsoluteFileUri()
        {
            var url = "file:///test";
            var result = UriHelper.TryCreateUriFromUrl(url, out Uri actual);
            var expected = new Uri("/test", UriKind.Relative);
            Assert.IsTrue(result);
            Assert.AreEqual(expected, actual);
        }

        #endregion TryCreateUriFromUrl

        #region AddTrailingSlash

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashNeededUrl()
        {
            var url = "http://www.google.de/test";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            var expected = url + "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashNeededUri()
        {
            var uri = new Uri("http://www.google.de/test");
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            var expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashNotNeededUrl()
        {
            var url = "http://www.google.de/test/";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashNotNeededUri()
        {
            var uri = new Uri("http://www.google.de/test/");
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_UrlEmpty()
        {
            var url = string.Empty;
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            var expected = "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_UrlNull()
        {
            string url = null;
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            var expected = "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashTooManySlashesUrl()
        {
            var url = "http://www.google.de//test//";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            var expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashTooManySlashesUri()
        {
            var uri = new Uri("http://www.google.de//test//", UriKind.RelativeOrAbsolute);
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            var expected = new Uri("http://www.google.de/test/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashFileUrl()
        {
            var url = "http://www.google.de/test/test.txt";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashFileUri()
        {
            var uri = new Uri("http://www.google.de/test/test.txt");
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashFileUrlUrlNull()
        {
            string url = null;
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            Assert.AreEqual(@"/", urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashFileUriUriNull()
        {
            Uri uri = null;
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            Assert.AreEqual(new Uri(@"/", UriKind.RelativeOrAbsolute), uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashRelativeUri()
        {
            var uri = new Uri("/webdav/test", UriKind.Relative);
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            var expected = new Uri("/webdav/test/", UriKind.Relative);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_TrailingSlashRelativeUrl()
        {
            var url = "/webdav/test";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            var expected = "/webdav/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_UriWithDot()
        {
            var uri = new Uri("http://www.google.de/a.test/file.txt");
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            var expected = new Uri("http://www.google.de/a.test/file.txt/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_UrlWithDot()
        {
            var url = "http://www.google.de/a.test/file.txt";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            var expected = "http://www.google.de/a.test/file.txt/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_UriWithDotFileExcpected()
        {
            var uri = new Uri("http://www.google.de/a.test/file.txt");
            var uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            var expected = new Uri("http://www.google.de/a.test/file.txt");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UriHelper_AddTrailingSlash_UrlWithDotFileExcpected()
        {
            var url = "http://www.google.de/a.test/file.txt";
            var urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            var expected = "http://www.google.de/a.test/file.txt";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        #endregion AddTrailingSlash

        #region CombineUri

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithBaseUri()
        {
            var baseUri = new Uri("http://www.google.de/test/");
            var relativeUri = new Uri("test2/", UriKind.Relative);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithDoubleBaseUri()
        {
            var baseUri = new Uri("http://www.google.de/test/");
            var relativeUri = new Uri("http://www.google.de/test/test2/");
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithBaseUriAndAbsoluteUri()
        {
            var baseUri = new Uri("http://www.google.de/test/test2");
            var relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri);
            var expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithBaseUriAndAbsoluteUriRemoveDuplicatePath()
        {
            var baseUri = new Uri("http://www.google.de/test/test2");
            var relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithBaseUriAndAbsoluteUriNoRemoveDuplicatePath()
        {
            var baseUri = new Uri("http://www.google.de/test/test2");
            var relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, false);
            var expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_FromSameBaseUri()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var baseUri2 = new Uri("http://www.google.de/test");
            var combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            var expected = new Uri("http://www.google.de/test");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_FromSameBaseUriAndPath()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var baseUri2 = new Uri("http://www.google.de/test/test2");
            var combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            var expected = new Uri("http://www.google.de/test/test2");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithoutBaseUri()
        {
            Uri baseUri = null;
            var relativeUri = new Uri("http://www.google.de/test/test2/");
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_FromBaseUriAndSlash()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var relativeUri = new Uri("/", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_FromBaseUriAndSlashTooManySlashes()
        {
            var baseUri = new Uri("http://www.google.de/test//");
            var relativeUri = new Uri("//", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_CombineUri_RelativeUriFirst_ShouldThrowArgumentException()
        {
            var baseUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
            var relativeUri = new Uri("http://www.google.de/test/");
            UriHelper.CombineUri(baseUri, relativeUri, true);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_TwoRelativeUris()
        {
            var baseUri = new Uri("/test", UriKind.RelativeOrAbsolute);
            var relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_FirstUriEndWithSecondUri()
        {
            var baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            var relativeUri = new Uri("/test2/test.txt", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_FirstUriEndsWithSecondUriWithoutSlashSecondUri()
        {
            var baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            var relativeUri = new Uri("test2/test.txt", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithSpacesAndBrackets()
        {
            var baseUri = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/");

            if (!Uri.TryCreate("/remote.php/webdav/folder/folder%20with%20space%20and%20%28%29/x.mp3", UriKind.RelativeOrAbsolute, out Uri relativeUri))
                Assert.Fail();

            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/x.mp3");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithDuplicatePathEntriesRemoveDuplicate()
        {
            var baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");

            if (!Uri.TryCreate("/remote.php/webdav/folder-sync/x/folder/", UriKind.RelativeOrAbsolute, out Uri relativeUri))
                Assert.Fail();

            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithDuplicatePathEntriesDoNotRemoveDuplicate()
        {
            var baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");

            if (!Uri.TryCreate("/remote.php/webdav/folder-sync/x/folder/", UriKind.RelativeOrAbsolute, out Uri relativeUri))
                Assert.Fail();

            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri);
            var expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_WithDuplicatePathEntriesDuplicateRepetition()
        {
            var baseUri = new Uri("https://google.de/remote.php/webdav/x/x/");

            if (!Uri.TryCreate("/remote.php/webdav/x/x/folder/", UriKind.RelativeOrAbsolute, out Uri relativeUri))
                Assert.Fail();

            var combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            var expected = new Uri("https://google.de/remote.php/webdav/x/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_TwoRelativeUrisOverlapping()
        {
            var uri1 = new Uri("/test/test2", UriKind.RelativeOrAbsolute);
            var uri2 = new Uri("test2/test3", UriKind.RelativeOrAbsolute);
            var combine = UriHelper.CombineUri(uri1, uri2, true);
            var expected = new Uri("/test/test2/test3", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_FirstUriNull()
        {
            Uri uri1 = null;
            var uri2 = new Uri("/test/test2", UriKind.RelativeOrAbsolute);
            var combine = UriHelper.CombineUri(uri1, uri2, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_SecondUriNull()
        {
            var uri1 = new Uri("/test/test2", UriKind.RelativeOrAbsolute);
            Uri uri2 = null;
            var combine = UriHelper.CombineUri(uri1, uri2, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUri_SamePathWithAndWithoutSlash()
        {
            var uri1 = new Uri("http://google.de/test");
            var uri2 = new Uri("http://google.de/test/");
            var combine = UriHelper.CombineUri(uri1, uri2, true);
            var expected = new Uri("http://google.de/test/");
            Assert.AreEqual(expected, combine);
        }

        #endregion CombineUri

        #region CombineUrl

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithBaseUrl()
        {
            var baseUrl = "http://www.google.de/test/";
            var relativeUrl = "test2/";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithDoubleBaseUrl()
        {
            var baseUrl = "http://www.google.de/test/";
            var relativeUrl = "http://www.google.de/test/test2/";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithBaseUrlAndAbsoluteUrl()
        {
            var baseUrl = "http://www.google.de/test/test2";
            var relativeUrl = "test2/test.txt";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl);
            var expected = "http://www.google.de/test/test2/test2/test.txt";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithBaseUrlAndAbsoluteUrlRemoveDuplicatePath()
        {
            var baseUrl = "http://www.google.de/test/test2";
            var relativeUrl = "test2/test.txt";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test/test2/test.txt";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithBaseUrlAndAbsoluteUrlNoRemoveDuplicatePath()
        {
            var baseUrl = "http://www.google.de/test/test2";
            var relativeUrl = "test2/test.txt";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, false);
            var expected = "http://www.google.de/test/test2/test2/test.txt";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FromSameBaseUrl()
        {
            var baseUrl = "http://www.google.de/test";
            var baseUrl2 = "http://www.google.de/test";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, baseUrl2, true);
            var expected = "http://www.google.de/test";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FromSameBaseUrlAndPath()
        {
            var baseUrl = "http://www.google.de/test";
            var baseUrl2 = "http://www.google.de/test/test2";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, baseUrl2, true);
            var expected = "http://www.google.de/test/test2";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_CombineUrl_WithoutBaseUrl_ShouldThrowArgumentException()
        {
            var relativeUrl = "http://www.google.de/test/test2/";
            UriHelper.CombineUrl(null, relativeUrl, true);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FromBaseUriAndSlash()
        {
            var baseUrl = "http://www.google.de/test";
            var relativeUrl = "/";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FromBaseUrlAndSlashTooManySlashes()
        {
            var baseUrl = "http://www.google.de/test//";
            var relativeUrl = "//";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_CombineUrl_RelativeUrlFirst_ShouldThrowArgumentException()
        {
            var baseUrl = "/test2";
            var relativeUrl = "http://www.google.de/test/";
            UriHelper.CombineUrl(baseUrl, relativeUrl, true);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_TwoRelativeUrls()
        {
            var baseUrl = "/test";
            var relativeUrl = "test2";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "/test/test2";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FirstUrlEndWithSecondUrl()
        {
            var baseUrl = "http://www.google.de/test/test2/test.txt";
            var relativeUrl = "/test2/test.txt";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test/test2/test.txt";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FirstUrlEndsWithSecondUrlWithoutSlashSecondUrl()
        {
            var baseUrl = "http://www.google.de/test/test2/test.txt";
            var relativeUrl = "test2/test.txt";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test/test2/test.txt";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithSpacesAndBrackets()
        {
            var baseUrl = "http://google.de/remote.php/webdav/folder/folder with space and ()/";
            var relativeUrl = "/remote.php/webdav/folder/folder%20with%20space%20and%20%28%29/x.mp3";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://google.de/remote.php/webdav/folder/folder with space and ()/x.mp3";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithDuplicatePathEntriesRemoveDuplicate()
        {
            var baseUrl = "http://google.de/remote.php/webdav/folder-sync/x/";
            var relativeUrl = "/remote.php/webdav/folder-sync/x/folder/";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "http://google.de/remote.php/webdav/folder-sync/x/folder/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithDuplicatePathEntriesDoNotRemoveDuplicate()
        {
            var baseUrl = "http://google.de/remote.php/webdav/folder-sync/x/";
            var relativeUrl = "/remote.php/webdav/folder-sync/x/folder/";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl);
            var expected = "http://google.de/remote.php/webdav/folder-sync/x/remote.php/webdav/folder-sync/x/folder/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_WithDuplicatePathEntriesDuplicateRepetition()
        {
            var baseUrl = "https://google.de/remote.php/webdav/x/x/";
            var relativeUrl = "/remote.php/webdav/x/x/folder/";
            var combinedUrl = UriHelper.CombineUrl(baseUrl, relativeUrl, true);
            var expected = "https://google.de/remote.php/webdav/x/x/folder/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_TwoRelativeUrlsOverlapping()
        {
            var url1 = "/test/test2";
            var url2 = "test2/test3";
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2/test3";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FirstUrlStringEmpty()
        {
            var url1 = string.Empty;
            var url2 = "/test/test2";
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_SecondUrlStringEmpty()
        {
            var url1 = "/test/test2/";
            var url2 = string.Empty;
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2/";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_FirstUrlNull()
        {
            string url1 = null;
            var url2 = "/test/test2";
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_SecondUrlNull()
        {
            var url1 = "/test/test2/";
            string url2 = null;
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "/test/test2/";
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrl_SamePathWithAndWithoutSlash()
        {
            var url1 = "http://google.de/test";
            var url2 = "http://google.de/test/";
            var combine = UriHelper.CombineUrl(url1, url2, true);
            var expected = "http://google.de/test/";
            Assert.AreEqual(expected, combine);
        }

        #endregion CombineUrl

        #region CombineUriAndUrl

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithBaseUri()
        {
            var baseUri = new Uri("http://www.google.de/test/");
            var relativeUrl = "test2/";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithDoubleBaseUri()
        {
            var baseUri = new Uri("http://www.google.de/test/");
            var relativeUrl = "http://www.google.de/test/test2/";
            var combinedUri = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithBaseUriAndAbsoluteUrl()
        {
            var baseUri = new Uri("http://www.google.de/test/test2");
            var relativeUrl = "test2/test.txt";
            var combinedUri = UriHelper.CombineUriAndUrl(baseUri, relativeUrl);
            var expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithBaseUriAndAbsoluteUrlRemoveDuplicatePath()
        {
            var baseUri = new Uri("http://www.google.de/test/test2");
            var relativeUrl = "test2/test.txt";
            var combinedUri = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithBaseUriAndAbsoluteUrlNoRemoveDuplicatePath()
        {
            var baseUri = new Uri("http://www.google.de/test/test2");
            var relativeUrl = "test2/test.txt";
            var combinedUri = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, false);
            var expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_FromSameBaseUri()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var baseUrl = "http://www.google.de/test";
            var combinedUri = UriHelper.CombineUriAndUrl(baseUri, baseUrl, true);
            var expected = new Uri("http://www.google.de/test");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_FromSameBaseUriAndPath()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var baseUrl2 = "http://www.google.de/test/test2";
            var combinedUri = UriHelper.CombineUriAndUrl(baseUri, baseUrl2, true);
            var expected = new Uri("http://www.google.de/test/test2");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithoutBaseUri()
        {
            var relativeUrl = "http://www.google.de/test/test2/";
            var combinedUri = UriHelper.CombineUriAndUrl(null, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_FromBaseUriAndSlash()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var relativeUrl = "/";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_FromBaseUriAndSlashTooManySlashes()
        {
            var baseUri = new Uri("http://www.google.de/test//");
            var relativeUrl = "//";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_CombineUriAndUrl_RelativeUriFirst_ShouldThrowArgumentException()
        {
            var baseUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
            var relativeUrl = "http://www.google.de/test/";
            UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_TwoRelativeUris()
        {
            var baseUri = new Uri("/test", UriKind.RelativeOrAbsolute);
            var relativeUrl = "test2";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_FirstUriEndWithSecondUrl()
        {
            var baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            var relativeUrl = "/test2/test.txt";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_FirstUriEndsWithSecondUrlWithoutSlashSecondUrl()
        {
            var baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            var relativeUrl = "test2/test.txt";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithSpacesAndBrackets()
        {
            var baseUri = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/");
            var relativeUrl = "/remote.php/webdav/folder/folder%20with%20space%20and%20%28%29/x.mp3";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/x.mp3");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithDuplicatePathEntriesRemoveDuplicate()
        {
            var baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");
            var relativeUrl = "/remote.php/webdav/folder-sync/x/folder/";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithDuplicatePathEntriesDoNotRemoveDuplicate()
        {
            var baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");
            var relativeUrl = "/remote.php/webdav/folder-sync/x/folder/";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl);
            var expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_WithDuplicatePathEntriesDuplicateRepetition()
        {
            var baseUri = new Uri("https://google.de/remote.php/webdav/x/x/");
            var relativeUrl = "/remote.php/webdav/x/x/folder/";
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUri, relativeUrl, true);
            var expected = new Uri("https://google.de/remote.php/webdav/x/x/folder/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_TwoRelativeUrisOverlapping()
        {
            var uri = new Uri("/test/test2", UriKind.RelativeOrAbsolute);
            var url = "test2/test3";
            var combine = UriHelper.CombineUriAndUrl(uri, url, true);
            var expected = new Uri("/test/test2/test3", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_SecondUrlStringEmpty()
        {
            var uri = new Uri("/test/test2/", UriKind.RelativeOrAbsolute);
            var url2 = string.Empty;
            var combine = UriHelper.CombineUriAndUrl(uri, url2, true);
            var expected = new Uri("/test/test2/", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUriAndUrl_FirstUriNull()
        {
            var url = "/test/test2";
            var combine = UriHelper.CombineUriAndUrl(null, url, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UT_UriHelper_CombineUriAndUrl_SecondUrlNull_ShouldThrowArgumentNullException()
        {
            var uri = new Uri("/test/test2/", UriKind.RelativeOrAbsolute);
            UriHelper.CombineUriAndUrl(uri, null, true);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithBaseUrl()
        {
            var baseUrl = "http://www.google.de/test/";
            var relativeUri = new Uri("test2/", UriKind.RelativeOrAbsolute);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithDoubleBaseUri()
        {
            var baseUrl = "http://www.google.de/test/";
            var relativeUri = new Uri("http://www.google.de/test/test2/");
            var combinedUri = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithBaseUrlAndAbsoluteUri()
        {
            var baseUrl = "http://www.google.de/test/test2";
            var relativeUri = new Uri("test2/test.txt", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUriAndUrl(baseUrl, relativeUri);
            var expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithBaseUrlAndAbsoluteUriRemoveDuplicatePath()
        {
            var baseUrl = "http://www.google.de/test/test2";
            var relativeUri = new Uri("test2/test.txt", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithBaseUrlAndAbsoluteUriNoRemoveDuplicatePath()
        {
            var baseUrl = "http://www.google.de/test/test2";
            var relativeUri = new Uri("test2/test.txt", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, false);
            var expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_FromSameBaseUri()
        {
            var baseUrl = "http://www.google.de/test";
            var baseUri = new Uri("http://www.google.de/test");
            var combinedUri = UriHelper.CombineUriAndUrl(baseUrl, baseUri, true);
            var expected = new Uri("http://www.google.de/test");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_FromSameBaseUrlAndPath()
        {
            var baseUrl = "http://www.google.de/test";
            var baseUri = new Uri("http://www.google.de/test/test2");
            var combinedUri = UriHelper.CombineUriAndUrl(baseUrl, baseUri, true);
            var expected = new Uri("http://www.google.de/test/test2");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UT_UriHelper_CombineUrlAndUri_WithoutBaseUrl_ShouldThrowArgumentNullException()
        {
            var relativeUri = new Uri("http://www.google.de/test/test2/");
            UriHelper.CombineUriAndUrl(null, relativeUri, true);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_FromBaseUrlAndSlash()
        {
            var baseUrl = "http://www.google.de/test";
            var relativeUri = new Uri("/", UriKind.Relative);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_FromBaseUrlAndSlashTooManySlashes()
        {
            var baseUrl = "http://www.google.de/test//";
            var relativeUri = new Uri("//", UriKind.RelativeOrAbsolute);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/", UriKind.Absolute);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_CombineUrlAndUri_RelativeUrlFirst_ShouldThrowArgumentException()
        {
            var baseUrl = "/test2";
            var relativeUri = new Uri("http://www.google.de/test/");
            UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_TwoRelativeUrls()
        {
            var baseUrl = "/test";
            var relativeUri = new Uri("test2", UriKind.Relative);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_FirstUrlEndWithSecondUri()
        {
            var baseUrl = "http://www.google.de/test/test2/test.txt";
            var relativeUri = new Uri("/test2/test.txt", UriKind.Relative);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt", UriKind.Absolute);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_FirstUrlEndsWithSecondUriWithoutSlashSecondUrl()
        {
            var baseUrl = "http://www.google.de/test/test2/test.txt";
            var relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test/test2/test.txt", UriKind.Absolute);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithSpacesAndBrackets()
        {
            var baseUrl = "http://google.de/remote.php/webdav/folder/folder with space and ()/";
            var relativeUri = new Uri("/remote.php/webdav/folder/folder%20with%20space%20and%20%28%29/x.mp3", UriKind.RelativeOrAbsolute);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/x.mp3", UriKind.Absolute);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithDuplicatePathEntriesRemoveDuplicate()
        {
            var baseUrl = "http://google.de/remote.php/webdav/folder-sync/x/";
            var relativeUri = new Uri("/remote.php/webdav/folder-sync/x/folder/", UriKind.Relative);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/folder/", UriKind.Absolute);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithDuplicatePathEntriesDoNotRemoveDuplicate()
        {
            var baseUrl = "http://google.de/remote.php/webdav/folder-sync/x/";
            var relativeUri = new Uri("/remote.php/webdav/folder-sync/x/folder/", UriKind.RelativeOrAbsolute);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri);
            var expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/remote.php/webdav/folder-sync/x/folder/", UriKind.Absolute);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_WithDuplicatePathEntriesDuplicateRepetition()
        {
            var baseUrl = "https://google.de/remote.php/webdav/x/x/";
            var relativeUri = new Uri("/remote.php/webdav/x/x/folder/", UriKind.Relative);
            var combinedUrl = UriHelper.CombineUriAndUrl(baseUrl, relativeUri, true);
            var expected = new Uri("https://google.de/remote.php/webdav/x/x/folder/", UriKind.Absolute);
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_TwoRelativeUrlsOverlapping()
        {
            var url = "/test/test2";
            var uri = new Uri("test2/test3", UriKind.Relative);
            var combine = UriHelper.CombineUriAndUrl(url, uri, true);
            var expected = new Uri("/test/test2/test3", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_SecondUriNull()
        {
            var url = "/test/test2/";
            var combine = UriHelper.CombineUriAndUrl(url, null, true);
            var expected = new Uri("/test/test2/", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        [TestMethod]
        public void UT_UriHelper_CombineUrlAndUri_FirstUrlStringEmpty()
        {
            var uri = new Uri("/test/test2", UriKind.Relative);
            var combine = UriHelper.CombineUriAndUrl(string.Empty, uri, true);
            var expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combine);
        }

        #endregion CombineUriAndUrl

        #region GetCombinedUriWithTrailingSlash

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_WithoutTrailingSlashWithBaseUri()
        {
            var baseUri = new Uri("http://www.google.de");
            var relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_WithTrailingSlashWithBaseUri()
        {
            var baseUri = new Uri("http://www.google.de");
            var relativeUri = new Uri("test2/", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_TooManySlashes()
        {
            var baseUri = new Uri("http://www.google.de//");
            var relativeUri = new Uri("//test2//", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_BaseUriNull()
        {
            Uri baseUri = null;
            var relativeUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("/test2/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_RelativeUriNull()
        {
            var baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = null;
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_BothUrisNull()
        {
            Uri baseUri = null;
            Uri relativeUri = null;
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_WithTrailingSlashWithTooManySlashes()
        {
            var baseUri = new Uri("http://www.google.de//");
            var relativeUri = new Uri("/test2//", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_GetCombinedUriWithTrailingSlash_FromDifferentBaseUris_ShouldThrowArgumentException()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var baseUri2 = new Uri("http://www.github.com/test2");
            UriHelper.GetCombinedUriWithTrailingSlash(baseUri, baseUri2, true);
        }

        #endregion GetCombinedUriWithTrailingSlash

        #region GetCombinedUrlWithTrailingSlash

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_WithoutTrailingSlashWithBaseUrl()
        {
            var baseUrl = "http://www.google.de";
            var relativeUrl = "test2";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_WithTrailingSlashWithBaseUrl()
        {
            var baseUrl = "http://www.google.de";
            var relativeUrl = "test2/";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_TooManySlashes()
        {
            var baseUrl = "http://www.google.de//";
            var relativeUrl = "//test2//";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_BaseUrlNull()
        {
            string baseUrl = null;
            var relativeUrl = "/test2";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_RelativeUrlNull()
        {
            var baseUrl = "http://www.google.de//";
            string relativeUrl = null;
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_BothUrlsNull()
        {
            string baseUrl = null;
            string relativeUrl = null;
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = @"/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_WithTrailingSlashWithTooManySlashes()
        {
            var baseUrl = "http://www.google.de//";
            var relativeUrl = "/test2//";
            var combinedUrl = UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, relativeUrl, true);
            var expected = "http://www.google.de/test2/";
            Assert.AreEqual(expected, combinedUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_GetCombinedUrlWithTrailingSlash_FromDifferentBaseUrls_ShouldThrowArgumentException()
        {
            var baseUrl = "http://www.google.de/test";
            var baseUrl2 = "http://www.github.com/test2";
            UriHelper.GetCombinedUrlWithTrailingSlash(baseUrl, baseUrl2, true);
        }

        #endregion GetCombinedUrlWithTrailingSlash

        #region GetCombinedUriAndUrlWithTrailingSlash

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_WithoutTrailingSlashWithBaseUri()
        {
            var baseUri = new Uri("http://www.google.de");
            var relativeUrl = "test2";
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_WithTrailingSlashWithBaseUri()
        {
            var baseUri = new Uri("http://www.google.de");
            var relativeUrl = "test2/";
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_TooManySlashes()
        {
            var baseUri = new Uri("http://www.google.de//");
            var relativeUrl = "//test2//";
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_BaseUriNull()
        {
            Uri baseUri = null;
            var relativeUrl = "/test2";
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, relativeUrl, true);
            var expected = new Uri("/test2/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_RelativeUrlNull_ShouldThrowArgumentNullException()
        {
            var baseUri = new Uri("http://www.google.de//");
            UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, null, true);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_RelativeUrlStringEmpty()
        {
            var baseUri = new Uri("http://www.google.de//");
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, string.Empty, true);
            var expected = new Uri("http://www.google.de/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_WithTrailingSlashWithTooManySlashes()
        {
            var baseUri = new Uri("http://www.google.de//");
            var relativeUrl = "/test2//";
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, relativeUrl, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_GetCombinedUriAndUrlWithTrailingSlash_FromDifferentBaseUrls_ShouldThrowArgumentException()
        {
            var baseUri = new Uri("http://www.google.de/test");
            var baseUrl2 = "http://www.github.com/test2";
            UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUri, baseUrl2, true);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlAndUriWithTrailingSlash_WithoutTrailingSlashWithBaseUrl()
        {
            var baseUrl = "http://www.google.de";
            var relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlAndUriWithTrailingSlash_WithTrailingSlashWithBaseUrl()
        {
            var baseUrl = "http://www.google.de";
            var relativeUri = new Uri("test2/", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlAndUriWithTrailingSlash_TooManySlashes()
        {
            var baseUrl = "http://www.google.de//";
            var relativeUri = new Uri("//test2//", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UT_UriHelper_GetCombinedUrlAndUriWithTrailingSlash_BaseUrlNull_ShouldThrowArgumentNullException()
        {
            var relativeUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
            UriHelper.GetCombinedUriAndUrlWithTrailingSlash(null, relativeUri, true);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlAndUriWithTrailingSlash_RelativeUrlNull()
        {
            var baseUrl = "http://www.google.de//";
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUrl, null, true);
            var expected = new Uri("http://www.google.de/", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UriHelper_GetCombinedUrlAndUriWithTrailingSlash_WithTrailingSlashWithTooManySlashes()
        {
            var baseUrl = "http://www.google.de//";
            var relativeUri = new Uri("/test2//", UriKind.RelativeOrAbsolute);
            var combinedUri = UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUrl, relativeUri, true);
            var expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_UriHelper_GetCombinedUrlAndUriWithTrailingSlash_FromDifferentBaseUrls_ShouldThrowArgumentException()
        {
            var baseUrl = "http://www.google.de/test";
            var baseUri = new Uri("http://www.github.com/test2", UriKind.RelativeOrAbsolute);
            UriHelper.GetCombinedUriAndUrlWithTrailingSlash(baseUrl, baseUri, true);
        }

        #endregion GetCombinedUriAndUrlWithTrailingSlash

        #region RemovePortFromUri

        [TestMethod]
        public void UT_UriHelper_RemovePortFromUri()
        {
            var uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.RemovePort(uri);
            var expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UT_UriHelper_RemovePortFromUri_UriNull_ShouldThrowNullReferenceException()
        {
            Uri uri = null;
            UriHelper.RemovePort(uri);
        }

        #endregion RemovePortFromUri

        #region RemovePortFromUrl

        [TestMethod]
        public void UT_UriHelper_RemovePortFromUrl()
        {
            var url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.RemovePort(url);
            var expected = @"http://www.google.de/test/test2/test.txt";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_RemovePortFromUrl_UrlEmpty()
        {
            var url = string.Empty;
            var actual = UriHelper.RemovePort(url);
            var expected = string.Empty;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_RemovePortFromUrl_UrlNull()
        {
            string url = null;
            var actual = UriHelper.RemovePort(url);
            var expected = string.Empty;
            Assert.AreEqual(expected, actual);
        }

        #endregion RemovePortFromUrl

        #region SetPortToUri

        [TestMethod]
        public void UT_UriHelper_SetPortToUri()
        {
            var uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.SetPort(uri, 9999);
            var expected = new Uri("http://www.google.de:9999/test/test2/test.txt");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_SetPortToUri_RelativeUri()
        {
            var uri = new Uri("test/test2/test.txt", UriKind.RelativeOrAbsolute);
            var actual = UriHelper.SetPort(uri, 9999);
            var expected = new Uri("test/test2/test.txt", UriKind.RelativeOrAbsolute);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UT_UriHelper_SetPortToUri_UriNull_ShouldThrowNullReferenceException()
        {
            Uri uri = null;
            UriHelper.SetPort(uri, 9999);
        }

        #endregion SetPortToUri

        #region SetPortToUrl

        [TestMethod]
        public void UT_UriHelper_SetPortToUrl()
        {
            var url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.SetPort(url, 9999);
            var expected = @"http://www.google.de:9999/test/test2/test.txt";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void UT_UriHelper_SetPortToUrl_UrlEmpty_ShouldThrowUriFormatException()
        {
            var url = string.Empty;
            UriHelper.SetPort(url, 9999);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UT_UriHelper_SetPortToUrl_UrlNull_ShouldThrowArgumentNullException()
        {
            string url = null;
            UriHelper.SetPort(url, 9999);
        }

        #endregion SetPortToUrl

        #region GetPortFromUri

        [TestMethod]
        public void UT_UriHelper_GetPortFromUri()
        {
            var uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.GetPort(uri);
            var expected = 8080;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UT_UriHelper_GetPortFromUri_RelativeUri_ShouldThrowInvalidOperationException()
        {
            var uri = new Uri("test/test2/test.txt", UriKind.RelativeOrAbsolute);
            UriHelper.GetPort(uri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UT_UriHelper_GetPortFromUri_UriNull_ShouldThrowArgumentNullException()
        {
            Uri uri = null;
            UriHelper.GetPort(uri);
        }

        #endregion GetPortFromUri

        #region GetPortFromUrl

        [TestMethod]
        public void UT_UriHelper_GetPortFromUrl()
        {
            var url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.GetPort(url);
            var expected = 8080;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void UT_UriHelper_GetPortFromUrl_UrlEmpty_ShouldThrowUriFormatException()
        {
            var url = string.Empty;
            var actual = UriHelper.GetPort(url);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UT_UriHelper_GetPortFromUrl_UrlNull_ShouldThrowArgumentNullException()
        {
            string url = null;
            UriHelper.GetPort(url);
        }

        #endregion GetPortFromUrl

        #region EscapeFolders

        [TestMethod]
        public void UT_UriHelper_EscapeFolders_Url()
        {
            var url = @"http://google.de/test/test2";
            var actual = UriHelper.EscapePathSegments(url);
            var expected = @"http://google.de/test/test2";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_EscapeFolders_Url_DomainOnly()
        {
            var url = @"http://google.de/";
            var actual = UriHelper.EscapePathSegments(url);
            var expected = @"http://google.de/";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_EscapeFolders_Url_WithSpecialCharacters()
        {
            var url = @"http://google.de/test/test2 [test3]";
            var actual = UriHelper.EscapePathSegments(url);
            var expected = @"http://google.de/test/test2%20%5Btest3%5D";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_EscapeFolders_Uri()
        {
            var url = @"http://google.de/test/test2";
            var uri = new Uri(url);
            var actual = UriHelper.EscapePathSegments(uri);
            var expected = @"http://google.de/test/test2";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_EscapeFolders_Uri_DomainOnly()
        {
            var url = @"http://google.de/";
            var uri = new Uri(url);
            var actual = UriHelper.EscapePathSegments(uri);
            var expected = @"http://google.de/";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_EscapeFolders_Uri_WithSpecialCharacters()
        {
            var url = @"http://google.de/te#st/test2# [test3]";
            var uri = new Uri(url);
            var actual = UriHelper.EscapePathSegments(uri);
            var expected = @"http://google.de/te%23st/test2%23%20%5Btest3%5D";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_UriHelper_EscapeFolders_Uri_WithPortAndSpecialCharacters()
        {
            var url = @"http://google.de:8080/te#st/test2# [test3]";
            var uri = new Uri(url);
            var actual = UriHelper.EscapePathSegments(uri);
            var expected = @"http://google.de:8080/te%23st/test2%23%20%5Btest3%5D";
            Assert.AreEqual(expected, actual);
        }

        #endregion EscapeFolders
    }
}