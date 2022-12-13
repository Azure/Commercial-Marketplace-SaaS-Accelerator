using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.DataModel;

/// <summary>
/// Encapsulation for Plan attributes.
/// </summary>
public class PlanAttributesModel
{
    /// <summary>
    /// Gets or sets the plan attribute identifier.
    /// </summary>
    /// <value>
    /// The plan attribute identifier.
    /// </value>
    public int PlanAttributeId { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Gets or sets the offer attribute identifier.
    /// </summary>
    /// <value>
    /// The offer attribute identifier.
    /// </value>
    public int OfferAttributeId { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public string Type { get; set; }
}