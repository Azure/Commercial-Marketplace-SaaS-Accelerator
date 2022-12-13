namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Enumerator witrh new  status.
/// </summary>
public enum SubscriptionStatusEnumExtension
{
    /// <summary>
    /// The pending fulfillment start
    /// </summary>
    PendingFulfillmentStart,

    /// <summary>
    /// The subscribed
    /// </summary>
    Subscribed,

    /// <summary>
    /// The unsubscribed
    /// </summary>
    Unsubscribed,

    /// <summary>
    /// When status cannot be parsed to any of the other Status types
    /// </summary>
    UnRecognized,

    /// <summary>
    /// Pending Activation
    /// </summary>
    PendingActivation,

    /// <summary>
    /// The pending unsubscribe
    /// </summary>
    PendingUnsubscribe,

    /// <summary>
    /// The activation failed
    /// </summary>
    ActivationFailed,

    /// <summary>
    /// The unsubscribe failed
    /// </summary>
    UnsubscribeFailed,

    /// <summary>
    /// The Suspend 
    /// </summary>
    Suspend,
}