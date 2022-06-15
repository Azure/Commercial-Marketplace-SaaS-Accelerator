﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{    /// <summary>
     /// Metered Plan Scheduler Management Repository.
     /// </summary>
     /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IMeteredPlanSchedulerManagementRepository" />
    public class MeteredPlanSchedulerManagementRepository : IMeteredPlanSchedulerManagementRepository
    {

        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredPlanSchedulerManagementRepository"/> class.
        /// </summary>
        /// <param name="context">SaaS DAL Context</param>
        public MeteredPlanSchedulerManagementRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get All Records
        /// </summary>
        /// <returns></returns>

        public IEnumerable<MeteredPlanSchedulerManagement> Get()
        {
            return this.context.MeteredPlanSchedulerManagement;
        }

        /// <summary>
        /// Get record by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MeteredPlanSchedulerManagement Get(int id)
        {
            return this.context.MeteredPlanSchedulerManagement.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Insert new record or Update existing record in Schedule Management
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public int Save(MeteredPlanSchedulerManagement entity)
        {
            if (entity.StartDate.HasValue)
            {
                int minute = entity.StartDate.Value.Minute;
                if (entity.StartDate.Value.Minute >= 30)
                {
                    minute = 60 - entity.StartDate.Value.Minute;
                    entity.StartDate = entity.StartDate.Value.AddMinutes(minute);
                }
                else
                {
                    entity.StartDate = entity.StartDate.Value.AddMinutes(-1 * minute);
                }
            }
            var existingEntity = this.context.MeteredPlanSchedulerManagement.Where(s => (s.SubscriptionId == entity.SubscriptionId)&&(s.PlanId == entity.PlanId) && (s.DimensionId == entity.DimensionId)).FirstOrDefault();
            if (existingEntity != null)
            {
                existingEntity.Quantity = entity.Quantity;
                existingEntity.SchedulerName = entity.SchedulerName;
                existingEntity.FrequencyId = entity.FrequencyId;
                existingEntity.StartDate = entity.StartDate.Value.ToUniversalTime();
                existingEntity.NextRunTime = entity.NextRunTime.HasValue? entity.NextRunTime.Value.ToUniversalTime():null;
                this.context.MeteredPlanSchedulerManagement.Update(existingEntity);
                this.context.SaveChanges();
                return existingEntity.Id;
            }
            else
            {
                this.context.MeteredPlanSchedulerManagement.Add(entity);
                this.context.SaveChanges();
                return entity.Id;

            }

        }
        /// <summary>
        /// Remove record from Schedule Management
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(MeteredPlanSchedulerManagement entity)
        {
            this.context.MeteredPlanSchedulerManagement.Remove(entity);
            this.context.SaveChanges();
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.context.Dispose();
                }
            }

            this.disposed = true;
        }

    }
}
