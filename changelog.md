v0.4.0.0:
- New method overloads in WebDavClient without requiring a LockToken
- NuGet package available: https://www.nuget.org/packages/PortableWebDavLibrary/

v0.3.0.0:
- Split the Portable WebDAV Library into two parts: DecaTec.WebDav.NetFx.dll (to be used in projects targeting  the .NET Framework, Windows 8 or ASP.NET Core) and DecaTec.WebDav.Uwp.dll (to be used in projects targeting Windows 8.1, Windows Phone 8.1 and apps for the Universal Windows Platform (UWP))
- The split was necessary because there were some issues using the Portable WebDAV Library in Universal Windows Platform (UWP) projects (not being able to ignore invalid SSL certificates and a bug in System.Net.Http.HttpClient making it impossible to access WebDAV resources without receiving an exception). Maybe the Portable WebDAV Library gets merged into one DLL in the future, but as long as there is no consistent API to access web resources in .NET and UWP apps, the library will consist of these two parts

v0.2.0.0:
- Retarget for .NET 4.5 and ASP.NET Core 5.0
- The WebDavSession now uses the class NetworkCredential for user credentials. The class WebDavCredential is not longer supported

v0.1.1.0:
- Optimized URL handling in WebDavSession
- Added a more meaningful WebDavException when ListAsync (WebDavSession) fails due to wrong response status code
- WebDavException is now thrown when parsing a response (multi status or prop) fails

v0.1.0.0:
- Initial release of Portable WebDAV Library
