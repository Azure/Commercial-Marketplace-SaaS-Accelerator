using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Offer Model.
/// </summary>
public class OfferModel
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    public string OfferID { get; set; }

    /// <summary>
    /// Gets or sets the name of the offer.
    /// </summary>
    /// <value>
    /// The name of the offer.
    /// </value>
    public string OfferName { get; set; }

    /// <summary>
    /// Gets or sets the create date.
    /// </summary>
    /// <value>
    /// The create date.
    /// </value>
    public DateTime? CreateDate { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int? UserID { get; set; }

    /// <summary>
    /// Gets or sets the offer guid identifier.
    /// </summary>
    /// <value>
    /// The offer guid identifier.
    /// </value>
    public Guid? OfferGuid { get; set; }
}