﻿namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Plans Repository
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.DAL.Interface.IPlansRepository" />
    public class PlansRepository : IPlansRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PlansRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Plans> Get()
        {
            return Context.Plans;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Plans Get(int id)
        {
            return Context.Plans.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified plan details.
        /// </summary>
        /// <param name="planDetails">The plan details.</param>
        /// <returns></returns>
        public int Add(Plans planDetails)
        {
            if (planDetails != null && !string.IsNullOrEmpty(planDetails.PlanId))
            {
                var existingPlan = Context.Plans.Where(s => s.PlanId == planDetails.PlanId).FirstOrDefault();
                if (existingPlan != null)
                {
                    existingPlan.PlanId = planDetails.PlanId;
                    existingPlan.Description = planDetails.Description;
                    existingPlan.DisplayName = planDetails.DisplayName;

                    Context.Plans.Update(existingPlan);
                    Context.SaveChanges();
                    return existingPlan.Id;
                }
                else
                {
                    Context.Plans.Add(planDetails);
                    Context.SaveChanges();
                    return planDetails.Id;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the plan detail by plan identifier.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public Plans GetPlanDetailByPlanId(string planId)
        {
            return Context.Plans.Where(s => s.PlanId == planId).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified plan details.
        /// </summary>
        /// <param name="planDetails">The plan details.</param>
        public void Remove(Plans planDetails)
        {
            var existingPlan = Context.Plans.Where(s => s.Id == planDetails.Id).FirstOrDefault();
            if (existingPlan != null)
            {
                Context.Plans.Remove(existingPlan);
                Context.SaveChanges();
            }
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
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
