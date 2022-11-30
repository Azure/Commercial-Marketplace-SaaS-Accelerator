namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Enum For Web
/// Actions.
/// </summary>
public enum WebhookAction
{
    // (When the resource has been deleted)

    /// <summary>
    /// The unsubscribe.
    /// </summary>
    Unsubscribe,

    // (When the change plan operation has completed)

    /// <summary>
    /// The change plan.
    /// </summary>
    ChangePlan,

    // (When the change quantity operation has completed)

    /// <summary>
    /// The change quantity.
    /// </summary>
    ChangeQuantity,

    // (When resource has been suspended)

    /// <summary>
    /// The suspend.
    /// </summary>
    Suspend,

    // (When resource has been reinstated after suspension)

    /// <summary>
    /// The reinstate.
    /// </summary>
    Reinstate,

    /// <summary>
    /// The transfer.
    /// </summary>
    Transfer,
}