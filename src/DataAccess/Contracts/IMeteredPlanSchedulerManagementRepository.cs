﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    /// <summary>
    /// Scheduler Manager for Metered Plan.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.MeteredPlanSchedulerManagement}" />
    public interface IMeteredPlanSchedulerManagementRepository : IDisposable, IBaseRepository<MeteredPlanSchedulerManagement>
    {
        /// <summary>
        /// This will only update NextRun date and called when the meter trigger event is run
        /// </summary>
        /// <param name="meterPlanSchduleEvent"></param>
        /// <returns></returns>
        int UpdateNextRunDate(MeteredPlanSchedulerManagement meterPlanSchduleEvent);
    }
}
