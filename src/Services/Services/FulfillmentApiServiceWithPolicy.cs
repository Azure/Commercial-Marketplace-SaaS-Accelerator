// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services.Resilience;
using Microsoft.Marketplace.SaaS.Models;
using Polly;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Decorator that wraps every <see cref="IFulfillmentApiService"/> method in the
/// composed Polly 8.x <see cref="ResiliencePipeline"/> built by
/// <see cref="MarketplaceResiliencePolicy"/>.
///
/// <para>
/// Each call populates <see cref="MarketplaceResiliencePolicy.OperationKey"/> and
/// <see cref="MarketplaceResiliencePolicy.SubscriptionIdKey"/> on a pooled
/// <see cref="ResilienceContext"/> so that retry / circuit-breaker log entries
/// carry the operation name and subscription identifier.
/// </para>
///
/// <para>
/// Register the inner <see cref="FulfillmentApiService"/> as a concrete scoped/singleton
/// dependency and expose this class as the public <see cref="IFulfillmentApiService"/>
/// binding so that all call sites benefit from resilience without modification.
/// </para>
/// </summary>
public class FulfillmentApiServiceWithPolicy : IFulfillmentApiService
{
    private readonly FulfillmentApiService _inner;
    private readonly ResiliencePipeline _pipeline;

    /// <summary>
    /// Initializes a new instance of the <see cref="FulfillmentApiServiceWithPolicy"/> class.
    /// </summary>
    /// <param name="inner">The concrete inner service whose calls will be wrapped.</param>
    /// <param name="pipeline">The resilience pipeline built by <see cref="MarketplaceResiliencePolicy"/>.</param>
    public FulfillmentApiServiceWithPolicy(FulfillmentApiService inner, ResiliencePipeline pipeline)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>
    /// Creates a pooled <see cref="ResilienceContext"/> pre-populated with the
    /// operation name and an optional subscription id.
    /// The caller MUST call <see cref="ResilienceContextPool.Shared.Return"/> when done.
    /// </summary>
    private static ResilienceContext CreateContext(string operationName, string subscriptionId = null)
    {
        var ctx = ResilienceContextPool.Shared.Get(operationName);
        ctx.Properties.Set(MarketplaceResiliencePolicy.OperationKey, operationName);
        if (subscriptionId != null)
            ctx.Properties.Set(MarketplaceResiliencePolicy.SubscriptionIdKey, subscriptionId);
        return ctx;
    }

    // -----------------------------------------------------------------------
    // IFulfillmentApiService implementation — each method delegates to _inner
    // via the resilience pipeline.
    // Use the Func<ResilienceContext, ValueTask<TResult>> overload so the context
    // (and its properties) flow through the pipeline strategies' callbacks.
    // -----------------------------------------------------------------------

    /// <inheritdoc/>
    public async Task<List<SubscriptionResult>> GetAllSubscriptionAsync()
    {
        var ctx = CreateContext(nameof(GetAllSubscriptionAsync));
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<List<SubscriptionResult>>(_inner.GetAllSubscriptionAsync()),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionResult> GetSubscriptionByIdAsync(Guid subscriptionId)
    {
        var ctx = CreateContext(nameof(GetSubscriptionByIdAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<SubscriptionResult>(_inner.GetSubscriptionByIdAsync(subscriptionId)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public SubscriptionResult GetSubscriptionById(Guid subscriptionId)
    {
        // Synchronous path: use the synchronous Execute overload.
        var ctx = CreateContext(nameof(GetSubscriptionById), subscriptionId.ToString());
        try
        {
            return _pipeline.Execute(
                _ => _inner.GetSubscriptionById(subscriptionId),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<ResolvedSubscriptionResult> ResolveAsync(string marketPlaceAccessToken)
    {
        var ctx = CreateContext(nameof(ResolveAsync));
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<ResolvedSubscriptionResult>(_inner.ResolveAsync(marketPlaceAccessToken)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<List<PlanDetailResultExtension>> GetAllPlansForSubscriptionAsync(Guid subscriptionId)
    {
        var ctx = CreateContext(nameof(GetAllPlansForSubscriptionAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<List<PlanDetailResultExtension>>(_inner.GetAllPlansForSubscriptionAsync(subscriptionId)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionUpdateResult> ChangePlanForSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID)
    {
        var ctx = CreateContext(nameof(ChangePlanForSubscriptionAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<SubscriptionUpdateResult>(_inner.ChangePlanForSubscriptionAsync(subscriptionId, subscriptionPlanID)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionUpdateResult> ChangeQuantityForSubscriptionAsync(Guid subscriptionId, int? subscriptionQuantity)
    {
        var ctx = CreateContext(nameof(ChangeQuantityForSubscriptionAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<SubscriptionUpdateResult>(_inner.ChangeQuantityForSubscriptionAsync(subscriptionId, subscriptionQuantity)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId)
    {
        var ctx = CreateContext(nameof(GetOperationStatusResultAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<OperationResult>(_inner.GetOperationStatusResultAsync(subscriptionId, operationId)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<Response> PatchOperationStatusResultAsync(Guid subscriptionId, Guid operationId, UpdateOperationStatusEnum updateOperationStatus)
    {
        var ctx = CreateContext(nameof(PatchOperationStatusResultAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<Response>(_inner.PatchOperationStatusResultAsync(subscriptionId, operationId, updateOperationStatus)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionUpdateResult> DeleteSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID)
    {
        var ctx = CreateContext(nameof(DeleteSubscriptionAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<SubscriptionUpdateResult>(_inner.DeleteSubscriptionAsync(subscriptionId, subscriptionPlanID)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public async Task<Response> ActivateSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID)
    {
        var ctx = CreateContext(nameof(ActivateSubscriptionAsync), subscriptionId.ToString());
        try
        {
            return await _pipeline.ExecuteAsync(
                _ => new ValueTask<Response>(_inner.ActivateSubscriptionAsync(subscriptionId, subscriptionPlanID)),
                ctx);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(ctx);
        }
    }

    /// <inheritdoc/>
    public string GetSaaSAppURL()
    {
        // No network call — no resilience wrapping needed.
        return _inner.GetSaaSAppURL();
    }
}
