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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace MaxMelcher.SignalR.Features.SPSignalR
{
    /// <summary>
    ///This class registers a http module in the web.config of the webapplication. 
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("25cdabbc-5201-4e78-b087-6a55f40ca1d2")]
    public class SPSignalREventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                                                  new SPDiagnosticsCategory(
                                                      "MaxMelcher.SPSignalR",
                                                      TraceSeverity.Medium,
                                                      EventSeverity.Information),
                                                  TraceSeverity.Medium,
                                                  string.Format(
                                                      "Feature: Registering HTTPModule for MaxMelcher.SPSignalR"),
                                                  null);
            RegisterHttpModule(properties);
        }

        private void RegisterHttpModule(SPFeatureReceiverProperties properties)
        {
            SPWebConfigModification webConfigModification = CreateWebModificationObject();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                SPWebService contentService = SPWebService.ContentService;

                contentService.WebConfigModifications.Add(webConfigModification);
                contentService.Update();
                contentService.ApplyWebConfigModifications();
            });
        }

        /// <summary>
        /// Create the SPWebConfigModification object for the signalr module
        /// </summary>
        /// <returns>SPWebConfigModification object for the http module to the web.config</returns>
        private SPWebConfigModification CreateWebModificationObject()
        {
            string name = String.Format("add[@name=\"{0}\"]", typeof(SignalRModule).Name);
            string xpath = "/configuration/system.webServer/modules";

            SPWebConfigModification webConfigModification = new SPWebConfigModification(name, xpath);

            webConfigModification.Owner = "MaxMelcher.SPSignalR";
            webConfigModification.Sequence = 0;
            webConfigModification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

            //reflection safe
            webConfigModification.Value = String.Format("<add name=\"{0}\" type=\"{1}\" />", typeof(SignalRModule).Name,
                                                        typeof(SignalRModule).AssemblyQualifiedName);
            return webConfigModification;
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                                                  new SPDiagnosticsCategory(
                                                      "MaxMelcher.SPSignalR",
                                                      TraceSeverity.Medium,
                                                      EventSeverity.Information),
                                                  TraceSeverity.Medium,
                                                  string.Format("Feature: Removing HTTPModule for MaxMelcher.SPSignalR"),
                                                  null);
            UnregisterHttpModule(properties);
        }

        private void UnregisterHttpModule(SPFeatureReceiverProperties properties)
        {

            SPWebConfigModification webConfigModification = CreateWebModificationObject();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                SPWebService contentService = properties.Definition.Farm.Services.GetValue<SPWebService>();

                int numberOfModifications = contentService.WebConfigModifications.Count;

                //Iterate over all WebConfigModification and delete only those we have created
                for (int i = numberOfModifications - 1; i >= 0; i--)
                {
                    SPWebConfigModification currentModifiction = contentService.WebConfigModifications[i];

                    if (currentModifiction.Owner.Equals(webConfigModification.Owner))
                    {
                        contentService.WebConfigModifications.Remove(currentModifiction);
                    }
                }

                //Update only if we have something deleted
                if (numberOfModifications > contentService.WebConfigModifications.Count)
                {
                    contentService.Update();
                    contentService.ApplyWebConfigModifications();
                }

            });
        }
    }
}
