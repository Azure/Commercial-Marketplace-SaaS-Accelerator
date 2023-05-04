using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Service to manage plans.
/// </summary>
public class MeteredPlanSchedulerManagementService
{
    private ISchedulerFrequencyRepository frequencyRepository;
    private IMeteredPlanSchedulerManagementRepository schedulerRepository;
    private ISchedulerManagerViewRepository schedulerViewRepository;
    private ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository;
    private IApplicationConfigRepository applicationConfigRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeteredPlanSchedulerManagementService"/> class.
    /// </summary>
    /// <param name="meteredPlanSchedulerManagementRepository">The Scheduler repository.</param>
    /// <param name="schedulerFrequencyRepository">The Frequency attributes repository.</param>
    /// <param name="schedulerManagerViewRepository">The Scheduler Manager View attributes repository.</param>

    public MeteredPlanSchedulerManagementService(ISchedulerFrequencyRepository schedulerFrequencyRepository, IMeteredPlanSchedulerManagementRepository meteredPlanSchedulerManagementRepository, ISchedulerManagerViewRepository schedulerManagerViewRepository, ISubscriptionUsageLogsRepository subscriptionUsageLogsRepository,IApplicationConfigRepository applicationConfigRepository)
    {
        this.frequencyRepository = schedulerFrequencyRepository;
        this.schedulerRepository = meteredPlanSchedulerManagementRepository;
        this.schedulerViewRepository = schedulerManagerViewRepository;
        this.subscriptionUsageLogsRepository = subscriptionUsageLogsRepository;
        this.applicationConfigRepository = applicationConfigRepository;
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
    /// <summary>
    /// Gets Enabled Schedule Frequency.
    /// </summary>
    /// <returns>List of Enabled Frequency.</returns>
    public List<SchedulerFrequencyModel> GetAllEnabledFrequency()
    {
        List<SchedulerFrequencyModel> frequencyList = new List<SchedulerFrequencyModel>();
        var allFrequencyData = this.frequencyRepository.GetAll().ToList();
        var allAppConfigData = this.applicationConfigRepository.GetAll().ToList();

        foreach (var item in allFrequencyData)
        {
            SchedulerFrequencyModel frequency = new SchedulerFrequencyModel();
            var isEnabled = bool.TryParse(allAppConfigData.Where(s => s.Name == ($"Enable{item.Frequency}MeterSchedules")).FirstOrDefault().Value, out bool isFrequencyEnabled);

            if (isEnabled && isFrequencyEnabled)
            {
                frequency.Id = item.Id;
                frequency.Frequency = item.Frequency;
                frequencyList.Add(frequency);
            }
        }
        return frequencyList;
    }

    /// <summary>
    /// Get All Scheduled Metered trigger list
    /// </summary>
    /// <returns>List of Scheduler Manager View</returns>
    public List<SchedulerManagerViewModel> GetAllSchedulerManagerList()
    {
        List<SchedulerManagerViewModel> schedulerList = new List<SchedulerManagerViewModel>();
        var allSchedulerViewData = this.schedulerViewRepository.GetAll().OrderBy(s => s.AMPSubscriptionId);
        foreach (var item in allSchedulerViewData)
        {
            SchedulerManagerViewModel schedulerView = new SchedulerManagerViewModel();
            schedulerView.Id = item.Id;
            schedulerView.PlanId = item.PlanId;
            schedulerView.PurchaserEmail = item.PurchaserEmail;
            schedulerView.SchedulerName = item.SchedulerName;
            schedulerView.SubscriptionName = item.SubscriptionName;
            schedulerView.AMPSubscriptionId = item.AMPSubscriptionId;
            schedulerView.Dimension = item.Dimension;
            schedulerView.Frequency = item.Frequency;
            schedulerView.Quantity = item.Quantity;
            schedulerView.StartDate = item.StartDate;
            schedulerView.NextRunTime = item.NextRunTime;
                
            schedulerList.Add(schedulerView);
        }

        foreach (var item in schedulerList)
        {
            item.LastRunTime = this.GetSchedulerLastRunTime(item.Id,item.SchedulerName);
        }

        return schedulerList;
    }

    /// <summary>
    /// Get All Scheduled Metered trigger list
    /// </summary>
    /// <returns>List of Scheduler Manager View</returns>
    public SchedulerManagerViewModel GetSchedulerManagerById(int Id)
    {

        var item = this.schedulerViewRepository.GetById(Id);
        SchedulerManagerViewModel schedulerView = new SchedulerManagerViewModel();
        schedulerView.Id = item.Id;
        schedulerView.PlanId = item.PlanId;
        schedulerView.PurchaserEmail = item.PurchaserEmail;
        schedulerView.SchedulerName = item.SchedulerName;
        schedulerView.SubscriptionName = item.SubscriptionName;
        schedulerView.AMPSubscriptionId = item.AMPSubscriptionId;
        schedulerView.Dimension = item.Dimension;
        schedulerView.Frequency = item.Frequency;
        schedulerView.Quantity = item.Quantity;
        schedulerView.StartDate = item.StartDate;
        schedulerView.NextRunTime = item.NextRunTime;
        return schedulerView;
    }

    /// <summary>
    /// Gets the Scheduler detail by  identifier.
    /// </summary>
    /// <param name="Id">The identifier.</param>
    /// <returns> Scheduler Manager Model</returns>
    public MeteredPlanSchedulerManagementModel GetSchedulerDetailById(int id)
    {
        var existingScheduledMeteredPlan = this.schedulerRepository.Get(id);

        MeteredPlanSchedulerManagementModel meteredPlanSchedule = new MeteredPlanSchedulerManagementModel
        {
            Id = existingScheduledMeteredPlan.Id,
            PlanId = existingScheduledMeteredPlan.PlanId,
            SchedulerName = existingScheduledMeteredPlan.SchedulerName,
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
    /// Get Scheduler Item audit history
    /// </summary>
    /// <param name="id"></param>
    /// <returns>History of scheduled Item run result.</returns>
    public List<MeteredAuditLogs> GetSchedulerItemRunHistory(int id)
    {
        List<MeteredAuditLogs> schedulerItemRunHistory = new List<MeteredAuditLogs>();
        var scheduledItem = this.schedulerRepository.Get(id);
        var meteredAudits = this.subscriptionUsageLogsRepository.GetMeteredAuditLogsBySubscriptionId(Convert.ToInt32(scheduledItem.SubscriptionId));
        var scheduledItemView = this.schedulerViewRepository.GetById(id);
        foreach (var auditLog in meteredAudits)
        {
            var MeteringUsageRequest = JsonSerializer.Deserialize<MeteringUsageRequest>(auditLog.RequestJson);

            if (MeteringUsageRequest.Dimension== scheduledItemView.Dimension)
            {
                schedulerItemRunHistory.Add(auditLog);
            }
                
        }
        return schedulerItemRunHistory;

    }


    public DateTime? GetSchedulerLastRunTime(int id,string schedulerName)
    {
        DateTime? lastRunTime = null;
        var scheduledItem = this.schedulerRepository.Get(id);
        var meteredAudits = this.subscriptionUsageLogsRepository.GetMeteredAuditLogsBySubscriptionId(Convert.ToInt32(scheduledItem.SubscriptionId));
        var scheduledItemView = this.schedulerViewRepository.GetById(id);
        foreach (var auditLog in meteredAudits)
        {
            var MeteringUsageRequest = JsonSerializer.Deserialize<MeteringUsageRequest>(auditLog.RequestJson);

            if ((MeteringUsageRequest.Dimension == scheduledItemView.Dimension)&&(auditLog.RunBy == $"Scheduler - {schedulerName}"))
            {
                if ((lastRunTime == null)|| (lastRunTime < auditLog.CreatedDate))
                {
                    lastRunTime = auditLog.CreatedDate;
                }
            }

        }
        return lastRunTime;

    }

    /// <summary>
    /// Saves the Metered Plan Scheduler Management Model attributes.
    /// </summary>
    /// <param name="MeteredPlanSchedulerManagementModel">The Metered Plan Scheduler Management model.</param>
    /// <returns> Scheduler Id.</returns>
    public int? SaveSchedulerDetail(MeteredPlanSchedulerManagementModel meteredPlanSchedulerModel)
    {
        MeteredPlanSchedulerManagement meteredPlanScheduler = new MeteredPlanSchedulerManagement
        {
            Id = meteredPlanSchedulerModel.Id,
            PlanId = meteredPlanSchedulerModel.PlanId,
            SchedulerName = meteredPlanSchedulerModel.SchedulerName,
            SubscriptionId = meteredPlanSchedulerModel.SubscriptionId,
            DimensionId = meteredPlanSchedulerModel.DimensionId,
            FrequencyId = meteredPlanSchedulerModel.FrequencyId,
            Quantity = meteredPlanSchedulerModel.Quantity,
            StartDate = meteredPlanSchedulerModel.StartDate,
            NextRunTime = meteredPlanSchedulerModel.NextRunTime
        };
        return this.schedulerRepository.Save(meteredPlanScheduler);
    }

    /// <summary>
    /// Saves the Metered Plan Scheduler Management Model NextRunTime.
    /// </summary>
    /// <param name="MeteredPlanSchedulerManagementModel">The Metered Plan Scheduler Management model.</param>
    /// <returns> Scheduler Id.</returns>
    public int? UpdateSchedulerNextRunTime(MeteredPlanSchedulerManagementModel meteredPlanSchedulerModel)
    {
        MeteredPlanSchedulerManagement meteredPlanScheduler = new MeteredPlanSchedulerManagement
        {
            Id = meteredPlanSchedulerModel.Id,
            NextRunTime = meteredPlanSchedulerModel.NextRunTime
        };
        return this.schedulerRepository.UpdateNextRunDate(meteredPlanScheduler);
    }

    /// <summary>
    /// Remove Scheduler by Id
    /// </summary>
    /// <param name="id">Scheduler identifier</param>
    public void DeleteSchedulerDetailById(int id)
    {
        MeteredPlanSchedulerManagement meteredPlanScheduler = new MeteredPlanSchedulerManagement
        {
            Id = id
        };
        this.schedulerRepository.Remove(meteredPlanScheduler);
    }

}