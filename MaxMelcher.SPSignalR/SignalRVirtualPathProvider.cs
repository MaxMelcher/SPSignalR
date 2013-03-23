/*
*  Copyright (c) 2013 Maximilian Melcher // http://melcher.it 
*
*  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
* 
*  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
* 
*  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Web.Hosting;

namespace MaxMelcher.SignalR
{
    public class SignalRVirtualPathProvider : VirtualPathProvider
    {
        public override string CombineVirtualPaths(string basePath, string relativePath)
        {
            return Previous.CombineVirtualPaths(basePath, relativePath);
        }

        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return Previous.CreateObjRef(requestedType);
        }
        /// <summary>
        /// This is the only method where we need to do something. Every request containing signalr and the evil character (~), we remove the evil character.
        /// </summary>
        /// <param name="virtualDir"></param>
        /// <returns></returns>
        public override bool DirectoryExists(string virtualDir)
        {
            //removing the evil character - otherwise the hell freezes and yeah, SharePoint.
            if (virtualDir != null && virtualDir.Contains("signalr"))
            {
                string tmp = virtualDir.TrimStart('~');
                return Previous.DirectoryExists(tmp);
            }
            try
            {
                return Previous.DirectoryExists(virtualDir);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool FileExists(string virtualPath)
        {
            return Previous.FileExists(virtualPath);
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath,
                                                                              System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override string GetCacheKey(string virtualPath)
        {
            return Previous.GetCacheKey(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return Previous.GetDirectory(virtualDir);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return Previous.GetFile(virtualPath);
        }

        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            return Previous.GetFileHash(virtualPath, virtualPathDependencies);
        }
    }
}