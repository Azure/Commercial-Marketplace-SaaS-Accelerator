using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Offer Attributes Model.
/// </summary>
public class OfferAttributesModel
{
    /// <summary>
    /// Gets or sets the attribute identifier.
    /// </summary>
    /// <value>
    /// The attribute identifier.
    /// </value>
    public int AttributeID { get; set; }

    /// <summary>
    /// Gets or sets the parameter identifier.
    /// </summary>
    /// <value>
    /// The parameter identifier.
    /// </value>
    public string ParameterId { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the value type identifier.
    /// </summary>
    /// <value>
    /// The value type identifier.
    /// </value>
    public int? ValueTypeId { get; set; }

    // public List<ValueTypes> ValueType { get; set; }

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
    public int? Max { get; set; }

    /// <summary>
    /// Gets or sets determines the minimum of the parameters.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public int? Min { get; set; }

    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    public Guid? OfferId { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the display sequence.
    /// </summary>
    /// <value>
    /// The display sequence.
    /// </value>
    public int? DisplaySequence { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OfferAttributesModel"/> is isactive.
    /// </summary>
    /// <value>
    ///   <c>true</c> if isactive; otherwise, <c>false</c>.
    /// </value>
    public bool Isactive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is remove.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is remove; otherwise, <c>false</c>.
    /// </value>
    public bool IsRemove { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the create date.
    /// </summary>
    /// <value>
    /// The create date.
    /// </value>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is delete.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is delete; otherwise, <c>false</c>.
    /// </value>
    public bool IsDelete { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is required.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is required; otherwise, <c>false</c>.
    /// </value>
    public bool IsRequired { get; set; }
}