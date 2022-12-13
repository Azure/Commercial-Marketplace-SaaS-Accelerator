using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Offers Model.
/// </summary>
public class OffersModel
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
    /// Gets or sets the offer gu identifier.
    /// </summary>
    /// <value>
    /// The offer gu identifier.
    /// </value>
    public Guid? OfferGuId { get; set; }
}