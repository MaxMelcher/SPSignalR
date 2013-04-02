using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace MaxMelcher.SPSignalR.EventReceiver.Features.Feature2
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// We need to register the assembly in an <assembly/> tag in the web.config - otherwise the bootstrapper wont see the referenced hub.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("fd13f687-6ee7-4cc4-8126-602f8d153a6a")]
    public class Feature2EventReceiver : SPFeatureReceiver
    {
        private void RegisterHttpModule(SPFeatureReceiverProperties properties)
        {
            SPWebConfigModification webConfigModification = CreateWebModificationObject();

            //this is a webapp feature
            SPWebApplication webapp = properties.Feature.Parent as SPWebApplication;

            if (webapp != null)
            {
                webapp.WebConfigModifications.Add(webConfigModification);
                webapp.Update();
                webapp.WebService.ApplyWebConfigModifications();
                webapp.WebService.Update();
            }
        }

        /// <summary>
        /// Create the SPWebConfigModification object for the assembly
        /// </summary>
        /// <returns>SPWebConfigModification object for adding the download tracking http module to the web.config</returns>
        private SPWebConfigModification CreateWebModificationObject()
        {
            string strUniqueName = string.Format("add[@assembly='{0}']", this.GetType().Assembly.FullName);
            string strNode = "/configuration/system.web/compilation/assemblies";
            SPWebConfigModification webConfigModification = new SPWebConfigModification(strUniqueName, strNode)
            {
                Owner = this.GetType().Assembly.FullName,
                Sequence = 1,
                Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode,
                Value = string.Format("<add assembly='{0}' />", this.GetType().Assembly.FullName)
            };
            return webConfigModification;
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            RegisterHttpModule(properties);
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
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
    }
}
