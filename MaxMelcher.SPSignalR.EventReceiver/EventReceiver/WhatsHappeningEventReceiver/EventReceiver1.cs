using System;
using System.Security.Permissions;
using Microsoft.AspNet.SignalR;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace MaxMelcher.SPSignalR.EventReceiver.EventReceiver1
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class EventReceiver1 : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);

            //notify all clients that an item has been added.
            NotifyClient(properties, DocLibHub.EventType.Added);

        }

        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            //notify all clients that an item has been updated.
            NotifyClient(properties, DocLibHub.EventType.Updated);
        }
        /// <summary>
        /// Method to notify all clients
        /// </summary>
        private void NotifyClient(SPItemEventProperties properties, DocLibHub.EventType eventType)
        {
            try
            {
                //get the title and the url         
                string url = null;
                string title = null;
                if (properties.ListItem != null)
                {
                    url =  string.Concat(properties.List.ParentWebUrl, "/", properties.AfterUrl);
                }
                else
                {
                    //this is the deleted event - listitem does not exist anymore
                    url = string.Concat(properties.List.ParentWebUrl, "/", properties.BeforeUrl);
                }

                title = url.Substring(url.LastIndexOf("/") + 1);

                //get the hub from the outside, see https://github.com/SignalR/SignalR/wiki/Hubs#broadcasting-over-a-hub-from-outside-of-a-hub
                var context = GlobalHost.ConnectionManager.GetHubContext<DocLibHub>();

                //now call the method Notify on all conntected clients
                context.Clients.All.Notify(title, url, eventType);
            }
            catch (Exception exception)
            {
                SPDiagnosticsService.Local.WriteTrace(0,
                                                  new SPDiagnosticsCategory(
                                                      "MaxMelcher.SPSignalR",
                                                      TraceSeverity.Medium,
                                                      EventSeverity.Information),
                                                  TraceSeverity.Medium,
                                                  string.Format(
                                                      "Exception in EventReceiver: {0} Stack: {1}", exception.Message, exception.StackTrace),
                                                  null);
            }
        }


        /// <summary>
        /// An item was deleted.
        /// </summary>
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            base.ItemDeleted(properties);
            //notify all clients that an item has been deleted.
            NotifyClient(properties, DocLibHub.EventType.Deleted);
        }


    }
}