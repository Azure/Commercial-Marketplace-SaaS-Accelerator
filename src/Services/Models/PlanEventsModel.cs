using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Plan Events Model.
/// </summary>
public class PlanEventsModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Gets or sets the event identifier.
    /// </summary>
    /// <value>
    /// The event identifier.
    /// </value>
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the name of the event.
    /// </summary>
    /// <value>
    /// The name of the event.
    /// </value>
    public string EventName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PlanEventsModel"/> is isactive.
    /// </summary>
    /// <value>
    ///   <c>true</c> if isactive; otherwise, <c>false</c>.
    /// </value>
    public bool Isactive { get; set; }

    /// <summary>
    /// Gets or sets the success state emails.
    /// </summary>
    /// <value>
    /// The success state emails.
    /// </value>
    public string SuccessStateEmails { get; set; }

    /// <summary>
    /// Gets or sets the failure state emails.
    /// </summary>
    /// <value>
    /// The failure state emails.
    /// </value>
    public string FailureStateEmails { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [copy to customer].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [copy to customer]; otherwise, <c>false</c>.
    /// </value>
    public bool CopyToCustomer { get; set; }
}