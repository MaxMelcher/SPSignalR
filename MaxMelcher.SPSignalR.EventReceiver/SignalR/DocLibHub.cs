using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace MaxMelcher.SPSignalR.EventReceiver
{
    public class DocLibHub : Hub
    {

        public enum EventType
        {
            Added,
            Updated,
            Deleted
        }
    }
}
