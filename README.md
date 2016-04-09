# Project description

Portable WebDAV Library is a fully [RFC 4918](http://tools.ietf.org/html/rfc4918) compliant WebDAV client library which is implemented das portable class library (PCL) for use on desktop environments as well as mobile devices.

# Main project aims
* Full compliance to RFC 4918 (*HTTP Extensions for Web Distributed Authoring and Versioning (WebDAV)*)
* Portability: Mobile and desktop environment (project targets .NET Framework 4.5.1, Windows 8.1 and Windows Phone 8.1 or later)
* Level of abstraction: There is a low level of abstraction (class **WebDavClient**) which supports all WebDAV operations directly. This is recommended for users who are familiar with the RFC 4918 specification. A higher level of abstraction is also provided (class **WebDavSession**) that hides most of the WebDAV specific operations and provides an easy access to WebDAV Servers
* Fast and fluid: All operations which might last longer are implemented as asynchronous methods
* WebDAV object model: Object model that represents all WebDAV artifacts used in WebDAV communication (as XML request/response content). No need to build own request XML content strings or parsing the contents of a response of a WebDAV server
So far the project is tested against IIS and ownCloud (sabre/dav).

# Two versions of the Portable WebDAV Lirary
Since v0.3.0.0, the Portable WebDAV Library is split into two parts:
* DecaTec.WebDav.NetFx: To be used in projects targeting .NET Framework 4.5 (or later), Windows 8 and ASP.NET Core.
* DecaTec.WebDav.Uwp: To be used in projects targeting Windows 8.1, Windows Phone 8.1 and Universal Windows Platform (UWP) apps.
* 
----
A full (online) documentation of the library can be found here:
**[Portable WebDAV Library online documentation](https://decatec.de/ext/PortableWebDAVLibrary/Doc/index.html)**

For offline use, you can download the help file (CHM) here:
**[Portable WebDAV Library offline documentation](https://decatec.de/ext/PortableWebDAVLibrary/Doc/DecaTec.WebDav.Documentation.chm)**

----

## Important when using Portable WebDAV Library in a UWP project
Currently there is a bug in the _System.Net.Http.HttpClient_ which is referenced in a Universal Windows Platform (UWP) project. See [this wiki article](https://github.com/DecaTec/Portable-WebDAV-Library/wiki/Portable-WebDAV-Library-in-UWP-projects) for more information and a workaround.
