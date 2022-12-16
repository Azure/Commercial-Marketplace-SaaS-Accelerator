using System;
using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.AdminSite.Models.Offer;

/// <summary>
/// Offer View Model.
/// </summary>
public record OfferDetailsViewModel
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
    public string OfferId { get; set; }

    /// <summary>
    /// Gets or sets the name of the offer.
    /// </summary>
    /// <value>
    /// The name of the offer.
    /// </value>
    public string OfferName { get; set; }

    /// <summary>
    /// Gets or sets the offer unique identifier.
    /// </summary>
    /// <value>
    /// The offer unique identifier.
    /// </value>
    public Guid OfferGuid { get; set; }

    /// <summary>
    /// Gets or sets the offer attributes.
    /// </summary>
    /// <value>
    /// The offer attributes.
    /// </value>
    public List<OfferAttributesViewModel> OfferAttributes { get; set; }
}