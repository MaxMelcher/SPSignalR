/*
*  Copyright (c) 2013 Maximilian Melcher // http://melcher.it 
*
*  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
* 
*  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
* 
*  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.SharePoint.Administration;

namespace MaxMelcher.SignalR
{
    public class SignalRModule : IHttpModule
    {
        public static bool IsAttached = false;
        private readonly object _lock = new object();

        /// <summary>

        /// </summary>

        public void Dispose()
        {
            //nothing to clean up.
        }

        public void Init(HttpApplication context)
        {
            //check if the routes are already attached - this only happens once.
            if (IsAttached) return;

            //lock it, could be called several times in parallel 
            lock (_lock)
            {
                //check if the call before attached.
                if (IsAttached) return;

                SPDiagnosticsService.Local.WriteTrace(0,
                                                      new SPDiagnosticsCategory(
                                                          "MaxMelcher.SPSignalR",
                                                          TraceSeverity.Medium,
                                                          EventSeverity.Information),
                                                      TraceSeverity.Medium,
                                                      string.Format("SignalRModule: Attaching Hub and VirtualPathProvider"),
                                                      null);

                //this is usually call in the App_Start thing from SignalR and registers the hub endpoint.
                HubConfiguration hubConfiguration = new HubConfiguration
                    {
                        EnableDetailedErrors = true,
                        EnableJavaScriptProxies = true
                    };

                RouteTable.Routes.MapHubs(hubConfiguration);

                //register the custom VirtualPath provider that trims the starting ~ from the requested url.
                HostingEnvironment.RegisterVirtualPathProvider(new SignalRVirtualPathProvider());
                IsAttached = true;
            }
        }
    }
}
