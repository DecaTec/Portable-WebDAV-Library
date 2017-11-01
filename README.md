[![NuGet](https://img.shields.io/nuget/v/PortableWebDavLibrary.svg)](https://www.nuget.org/packages/PortableWebDavLibrary/)
[![Build status](https://ci.appveyor.com/api/projects/status/kywd18luty1eve2a?svg=true)](https://ci.appveyor.com/project/DecaTec/portable-webdav-library)
[![GitHub Stats](https://img.shields.io/badge/github-stats-ff5500.svg)](http://githubstats.com/DecaTec/Portable-WebDAV-Library)
[![Microsoft Public License](https://img.shields.io/github/license/DecaTec/Portable-WebDAV-Library.svg)](https://github.com/DecaTec/Portable-WebDAV-Library/blob/master/LICENSE)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=jr%40decatec%2ede&lc=US&item_name=Portable%20WebDAV%20Library&no_note=1&no_shipping=1&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted)

# Project description

The Portable WebDAV Library is a strongly typed, async WebDAV client library which is fully compliant to [RFC 4918](http://tools.ietf.org/html/rfc4918), [RFC 4331](https://tools.ietf.org/html/rfc4331), [Additional WebDAV Collection Properties](https://tools.ietf.org/html/draft-hopmann-collection-props-00) and a some WebDAV properties specific to IIS WebDAV. It is implemented as [.NETStandard 1.1](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) library in oder to be used on any platform supporting .NETStandard 1.1.

# Main project aims
* Full compliance to [RFC 4918 (*HTTP Extensions for Web Distributed Authoring and Versioning (WebDAV)*)](http://tools.ietf.org/html/rfc4918), [RFC 4331 (*Quota and Size Properties for Distributed Authoring and Versioning (DAV) Collections*)](https://tools.ietf.org/html/rfc4331) and [*Additional WebDAV Collection Properties*](https://tools.ietf.org/html/draft-hopmann-collection-props-00)
* Portability: The library can be used in any project targeting .NETStandard 1.1
* Level of abstraction: There is a low level of abstraction (class **WebDavClient**) which supports all WebDAV operations directly. This is recommended for users who are familiar with the RFC 4918 specification. A higher level of abstraction is also provided (class **WebDavSession**) that hides most of the WebDAV specific operations and provides an easy access to WebDAV Servers
* Fast and fluid: All operations which might last longer are implemented as asynchronous methods
* WebDAV object model: Object model that represents all WebDAV artifacts used in WebDAV communication (as XML request/response content). No need to build own request XML content strings or parsing the contents of a response of a WebDAV server
* Full HTTP/2 support

So far the project is tested against IIS and ownCloud/Nextcloud (sabre/dav) (note that WebDAV locking is only working with ownCloud 8 or earlier releases as with ownCloud 9 WebDAV locking is not supported anymore).

# Obtaining the library
* There is a NuGet package available: Just search for '**PortableWebDavLibrary**' in the '*Manage NuGet Packages...*' window in Visual Studio. You can also find the package [here](https://www.nuget.org/packages/PortableWebDavLibrary/).
* If you prefer the manual installation method, you can find the current release as ZIP file on the [GitHub release page](https://github.com/DecaTec/Portable-WebDAV-Library/releases).
 
# Documentation
There is a full documentation of the library with some example code available.

**[Portable WebDAV Library online documentation](https://decatec.de/ext/PortableWebDAVLibrary/Doc/index.html)**

For offline use, you can download the help file (CHM) here:
**[Portable WebDAV Library offline documentation](https://decatec.de/ext/PortableWebDAVLibrary/Doc/DecaTec.WebDav.Documentation.chm)**

# Beta versions
When there will be some considerable/breaking changes in a future version of the library, a preview version is released on [MyGet](https://www.myget.org/feed/decatec-preview/package/nuget/PortableWebDavLibrary) before releasing the final version on NuGet. This way, projects using the Portable WebDAV Library can test beta versions before a new version is released officially.

# Projects using the Portable WebDAV Library
* [FontoXML Editor](https://fontoxml.com/)
* Official [Nextcloud](https://nextcloud.com/) Windows app: [Windows App Store](https://www.microsoft.com/store/apps/9nblggh532xq)/[GitHub](https://github.com/nextcloud/windows-universal)
* CCPlayer Pro ([Windows App Store](https://www.microsoft.com/store/apps/9wzdncrfjljw))/CCPlayer UWP Ad ([Windows App Store](https://www.microsoft.com/store/apps/9nblggh4z7q0))
* [WebDAV-AudioPlayer](https://github.com/StefH/WebDAV-AudioPlayer)
