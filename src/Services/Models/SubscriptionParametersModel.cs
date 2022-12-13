using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Subscription Parameters Model.
/// </summary>
public class SubscriptionParametersModel
{
    /// <summary>
    /// Gets or sets the row number.
    /// </summary>
    /// <value>
    /// The row number.
    /// </value>
    public int RowNumber { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the plan attribute identifier.
    /// </summary>
    /// <value>
    /// The plan attribute identifier.
    /// </value>
    public int PlanAttributeId { get; set; }

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
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public string ValueType { get; set; }

    /// <summary>
    /// Gets or sets the display sequence.
    /// </summary>
    /// <value>
    /// The display sequence.
    /// </value>
    public int DisplaySequence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is required.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is required; otherwise, <c>false</c>.
    /// </value>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the subscription identifier.
    /// </summary>
    /// <value>
    /// The subscription identifier.
    /// </value>
    public Guid SubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    public Guid OfferId { get; set; }

    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the create date.
    /// </summary>
    /// <value>
    /// The create date.
    /// </value>
    public DateTime? CreateDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [from list].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [from list]; otherwise, <c>false</c>.
    /// </value>
    public bool FromList { get; set; }

    /// <summary>
    /// Gets or sets the values list.
    /// </summary>
    /// <value>
    /// The values list.
    /// </value>
    public string ValuesList { get; set; }

    /// <summary>
    /// Gets or sets determines the maximum of the parameters.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    public int Max { get; set; }

    /// <summary>
    /// Gets or sets determines the minimum of the parameters.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public int Min { get; set; }

    /// <summary>
    /// Gets or sets the type of the HTML.
    /// </summary>
    /// <value>
    /// The type of the HTML.
    /// </value>
    public string Htmltype { get; set; }
}