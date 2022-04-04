namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Service to manage plans.
    /// </summary>
    public class MeteredPlanSchedulerManagementService
    {
        private ISchedulerFrequencyRepository frequencyRepository;
        private IMeteredPlanSchedulerManagementRepository schedulerRepository;
        private ISchedulerManagerViewRepository schedulerViewRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredPlanSchedulerManagementService"/> class.
        /// </summary>
        /// <param name="schedulerRepository">The Scheduler repository.</param>
        /// <param name="frequencyRepository">The Frequency attributes repository.</param>

        public MeteredPlanSchedulerManagementService(ISchedulerFrequencyRepository frequencyRepository, IMeteredPlanSchedulerManagementRepository schedulerRepository, ISchedulerManagerViewRepository schedulerViewRepository)
        {
            this.frequencyRepository = frequencyRepository;
            this.schedulerRepository = schedulerRepository;
            this.schedulerViewRepository = schedulerViewRepository;
        }

        /// <summary>
        /// Gets the Schedule Frequency.
        /// </summary>
        /// <returns>List of Frequency.</returns>
        public List<SchedulerFrequencyModel> GetAllFrequency()
        {
            List<SchedulerFrequencyModel> frequencyList = new List<SchedulerFrequencyModel>();
            var allFrequencyData = this.frequencyRepository.GetAll();
            foreach (var item in allFrequencyData)
            {
                SchedulerFrequencyModel frequency = new SchedulerFrequencyModel();
                frequency.Id = item.Id;
                frequency.Frequency = item.Frequency;
                frequencyList.Add(frequency);
            }
            return frequencyList;
        }

        public List<SchedulerManagerViewModel> GetAllSchedulerManagerList()
        {
            List<SchedulerManagerViewModel> schedulerList = new List<SchedulerManagerViewModel>();
            var allSchedulerViewData = this.schedulerViewRepository.GetAll();
            foreach (var item in allSchedulerViewData)
            {
                SchedulerManagerViewModel schedulerView = new SchedulerManagerViewModel();
                schedulerView.Id = item.Id;
                schedulerView.PlanId = item.PlanId;
                schedulerView.AMPSubscriptionId = item.AMPSubscriptionId;
                schedulerView.Dimension = item.Dimension;
                schedulerView.Frequency = item.Frequency;
                schedulerView.Quantity = item.Quantity;
                schedulerView.StartDate = item.StartDate;
                schedulerView.NextRunTime = item.NextRunTime;
                schedulerList.Add(schedulerView);
            }
            return schedulerList;
        }

        /// <summary>
        /// Gets the plan detail by plan gu identifier.
        /// </summary>
        /// <param name="planGuId">The plan gu identifier.</param>
        /// <returns> Plans.</returns>
        public MeteredPlanSchedulerManagementModel GetSchedulerDetailById(int id)
        {
            var existingScheduledMeteredPlan = this.schedulerRepository.Get(id);

            MeteredPlanSchedulerManagementModel meteredPlanSchedule = new MeteredPlanSchedulerManagementModel
            {
                Id = existingScheduledMeteredPlan.Id,
                PlanId = existingScheduledMeteredPlan.PlanId,
                SubscriptionId = existingScheduledMeteredPlan.SubscriptionId,
                DimensionId = existingScheduledMeteredPlan.DimensionId,
                FrequencyId = existingScheduledMeteredPlan.FrequencyId,
                Quantity = existingScheduledMeteredPlan.Quantity,
                StartDate = existingScheduledMeteredPlan.StartDate,
                NextRunTime = existingScheduledMeteredPlan.NextRunTime
            };
            return meteredPlanSchedule;
        }

        /// <summary>
        /// Saves the plan attributes.
        /// </summary>
        /// <param name="planAttributes">The plan attributes.</param>
        /// <returns> Plan Event Id.</returns>
        public int? SaveSchedulerDetail(MeteredPlanSchedulerManagementModel meteredPlanSchedulerModel)
        {
            MeteredPlanSchedulerManagement meteredPlanScheduler = new MeteredPlanSchedulerManagement
            {
                Id = meteredPlanSchedulerModel.Id,
                PlanId = meteredPlanSchedulerModel.PlanId,
                SubscriptionId = meteredPlanSchedulerModel.SubscriptionId,
                DimensionId = meteredPlanSchedulerModel.DimensionId,
                FrequencyId = meteredPlanSchedulerModel.FrequencyId,
                Quantity = meteredPlanSchedulerModel.Quantity,
                StartDate = meteredPlanSchedulerModel.StartDate
            };


            return this.schedulerRepository.Save(meteredPlanScheduler);
             
        }


        public void DeleteSchedulerDetail(MeteredPlanSchedulerManagementModel meteredPlanSchedulerModel)
        {
            MeteredPlanSchedulerManagement meteredPlanScheduler = new MeteredPlanSchedulerManagement
            {
                Id = meteredPlanSchedulerModel.Id,
                PlanId = meteredPlanSchedulerModel.PlanId,
                SubscriptionId = meteredPlanSchedulerModel.SubscriptionId,
                DimensionId = meteredPlanSchedulerModel.DimensionId,
                FrequencyId = meteredPlanSchedulerModel.FrequencyId,
                Quantity = meteredPlanSchedulerModel.Quantity,
                StartDate = meteredPlanSchedulerModel.StartDate
            };


             this.schedulerRepository.Remove(meteredPlanScheduler);

        }

        public void DeleteSchedulerDetailById(int id)
        {
            MeteredPlanSchedulerManagement meteredPlanScheduler = new MeteredPlanSchedulerManagement
            {
                Id = id

            };


            this.schedulerRepository.Remove(meteredPlanScheduler);

        }

    }
}