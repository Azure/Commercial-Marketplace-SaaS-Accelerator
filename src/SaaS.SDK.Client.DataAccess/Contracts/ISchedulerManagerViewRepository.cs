using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface ISchedulerManagerViewRepository : IDisposable
    {
        /// <summary>
        /// Get all scheduler Manager data
        /// </summary>
        /// <returns></returns>
        IEnumerable<SchedulerManagerView> GetAll();
        /// <summary>
        /// Get  scheduler Manager data by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SchedulerManagerView GetById(int id);
    }
}
