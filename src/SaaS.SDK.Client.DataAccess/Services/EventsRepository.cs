using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class EventsRepository : IEventsRepository
    {
        private readonly SaasKitContext Context;

        public EventsRepository(SaasKitContext context)
        {
            Context = context;
        }

        public int GetEventID(String Name)
        {
            var results = Context.Events.Where(s => s.EventsName == Name);
            if (results.Count() == 0)
                return 0;
            else
                return Context.Events.Where(s => s.EventsName == Name).FirstOrDefault().EventsId;
        }
    }
}
