using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Subscription Usage Log.
/// </summary>
/// <seealso cref="ISubscriptionUsageLogsRepository" />
public class SubscriptionUsageLogsRepository : ISubscriptionUsageLogsRepository
{
    /// <summary>
    /// The this.context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionUsageLogsRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public SubscriptionUsageLogsRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Adds the specified metered audit logs.
    /// </summary>
    /// <param name="meteredAuditLogs">The metered audit logs.</param>
    /// <returns> Audit log id.</returns>
    public int Save(MeteredAuditLogs meteredAuditLogs)
    {
        this.context.MeteredAuditLogs.Add(meteredAuditLogs);
        this.context.SaveChanges();
        return meteredAuditLogs.Id;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets all the records.
    /// </summary>
    /// <returns> Metered Audit Logs.</returns>
    public IEnumerable<MeteredAuditLogs> Get()
    {
        return this.context.MeteredAuditLogs.Include(s => s.Subscription);
    }

    /// <summary>
    /// Gets the record for the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    /// Entity for the given identifier.
    /// </returns>
    public MeteredAuditLogs Get(int id)
    {
        return this.context.MeteredAuditLogs.Include(s => s.Subscription).Where(s => s.Id == id).FirstOrDefault();
    }

    /// <summary>
    /// Gets the metered audit logs by subscription identifier.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="format">Specify to format the result properly.</param>
    /// <returns> Metered Audit Logs.</returns>
    public List<MeteredAuditLogs> GetMeteredAuditLogsBySubscriptionId(int subscriptionId, bool format = false)
    {
        if (format)
        {
            return this.context.MeteredAuditLogs.Include(s => s.Subscription).Where(s => s.Subscription.Id == subscriptionId).OrderByDescending(s => s.CreatedDate).Select(FormatJson).ToList();
        }
        else
        {
            return this.context.MeteredAuditLogs.Include(s => s.Subscription).Where(s => s.Subscription.Id == subscriptionId).OrderByDescending(s => s.CreatedDate).ToList();
        }
    }

    /// <summary>
    /// Converts the metering usage request/response JSON into user-friendly strings
    /// </summary>
    /// <param name="logs">The Metered Audit Log to modify.</param>
    /// <returns> The updated Metered Audit Logs.</returns>
    private static MeteredAuditLogs FormatJson(MeteredAuditLogs logs)
    {
        // Update request format
        MeteringUsageRequestAttributes parsedRequest = JsonSerializer.Deserialize<MeteringUsageRequestAttributes>(logs.RequestJson);
        logs.RequestJson = "ResourceId: " + parsedRequest.ResourceId;
        logs.RequestJson += "\r\nQuantity: " + parsedRequest.Quantity;
        logs.RequestJson += "\r\nDimension: " + parsedRequest.Dimension;
        logs.RequestJson += "\r\nPlanId: " + parsedRequest.PlanId;

        // Update response format
        MeteringUsageResponseAttributes parsedResponse = JsonSerializer.Deserialize<MeteringUsageResponseAttributes>(logs.ResponseJson);
        if (parsedResponse != null)
        {
            logs.ResponseJson = "Usage Event Id: " + parsedResponse.UsagePostedDate;
            logs.ResponseJson += "\r\nStatus: " + parsedResponse.Status;
            logs.ResponseJson += "\r\nResourceId: " + parsedResponse.ResourceId;
            logs.ResponseJson += "\r\nQuantity: " + parsedResponse.Quantity;
            logs.ResponseJson += "\r\nDimension: " + parsedResponse.Dimension;
            logs.ResponseJson += "\r\nPlanId: " + parsedResponse.PlanId;
            logs.ResponseJson += "\r\nUsage Posted Date " + parsedResponse.UsagePostedDate;
            logs.ResponseJson += "\r\nMessage Time: " + parsedResponse.MessageTime;
        }
        else
        {
            logs.ResponseJson = "No Response";
        }

        return logs;
    }


    /// <summary>
    /// Removes the specified metered audit logs.
    /// </summary>
    /// <param name="meteredAuditLogs">The metered audit logs.</param>
    public void Remove(MeteredAuditLogs meteredAuditLogs)
    {
        this.context.MeteredAuditLogs.Remove(meteredAuditLogs);
        this.context.SaveChanges();
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