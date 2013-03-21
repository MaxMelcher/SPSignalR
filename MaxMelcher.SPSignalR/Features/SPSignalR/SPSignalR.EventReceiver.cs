using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace MaxMelcher.SPSignalR.Features.SPSignalR
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("dc1569c1-6a96-4690-a047-49ecf027f73c")]
    public class SPSignalREventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}



        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            SPDiagnosticsService.Local.WriteTrace(0,
                                        new SPDiagnosticsCategory(
                                            "MaxMelcher.SPSignalR",
                                            TraceSeverity.Medium,
                                            EventSeverity.Information),
                                            TraceSeverity.Medium,
                                            string.Format("Feature: Registering HTTPModule for MaxMelcher.SPSignalR"),
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
        /// Create the SPWebConfigModification object for the download tracking Http Module
        /// </summary>
        /// <returns>SPWebConfigModification object for adding the download tracking http module to the web.config</returns>
        private SPWebConfigModification CreateWebModificationObject()
        {
            string name = String.Format("add[@name=\"{0}\"]", typeof(SignalRModule).Name);
            string xpath = "/configuration/system.webServer/modules";

            SPWebConfigModification webConfigModification = new SPWebConfigModification(name, xpath);

            webConfigModification.Owner = "MaxMelcher.SPSignalR";
            webConfigModification.Sequence = 0;
            webConfigModification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

            /*
             * to be resistent to refactoring we use reflection to identity the module, 
             * so in case somebody changes the class name the web.config entry will be still valid
             */
            webConfigModification.Value = String.Format("<add name=\"{0}\" type=\"{1}\" />", typeof(SignalRModule).Name, typeof(SignalRModule).AssemblyQualifiedName);
            return webConfigModification;
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
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

                /*
                 * Iterate over all WebConfigModification and delete only those we have created
                 */
                for (int i = numberOfModifications - 1; i >= 0; i--)
                {
                    SPWebConfigModification currentModifiction = contentService.WebConfigModifications[i];

                    if (currentModifiction.Owner.Equals(webConfigModification.Owner))
                    {
                        contentService.WebConfigModifications.Remove(currentModifiction);
                    }
                }

                /*
                 * Update only if we have something deleted
                 */
                if (numberOfModifications > contentService.WebConfigModifications.Count)
                {
                    contentService.Update();
                    contentService.ApplyWebConfigModifications();
                }

            });
        }

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
