using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.SystemWeb;
using Microsoft.SharePoint.Administration;

namespace MaxMelcher.SPSignalR
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
            //check if the routes are already attached
            if (!IsAttached) 
            {
                //lock it, could be called several times in parallel 
                lock (_lock)
                {
                    //check if the call before attached.
                    if (!IsAttached)
                    {
                        SPDiagnosticsService.Local.WriteTrace(0,
                                        new SPDiagnosticsCategory(
                                            "MaxMelcher.SPSignalR",
                                            TraceSeverity.Medium,
                                            EventSeverity.Information),
                                            TraceSeverity.Medium,
                                            string.Format("SignalRModule: Attaching Hub and VirtualPathProvider"),
                                            null);

                        Thread.Sleep(15000);

                        Debugger.Launch();
                        //this is usually call in the App_Start thing from SignalR and registers the hub endpoint.
                        RouteTable.Routes.MapHubs(new HubConfiguration
                            {
                                EnableDetailedErrors = true,
                                EnableJavaScriptProxies = true
                            });

                        //register the custom VirtualPath provider that trims the starting ~ from the requested url.
                        HostingEnvironment.RegisterVirtualPathProvider(new SignalRVirtualPathProvider());
                        IsAttached = true;
                    }
                }
            }
        }
    }
}
