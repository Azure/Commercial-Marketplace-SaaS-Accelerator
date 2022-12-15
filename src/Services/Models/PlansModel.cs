using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// The Plans Model.
/// </summary>
public class PlansModel
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
    public string PlanId { get; set; }

    /// <summary>
    /// Gets or sets the ismetering supported.
    /// </summary>
    /// <value>
    /// The ismetering supported.
    /// </value>
    public bool? IsmeteringSupported { get; set; }

    /// <summary>
    /// Gets or sets the offer identifier.
    /// </summary>
    /// <value>
    /// The offer identifier.
    /// </value>
    public Guid? OfferID { get; set; }

    /// <summary>
    /// Gets or sets the name of the offer.
    /// </summary>
    /// <value>
    /// The name of the offer.
    /// </value>
    public string OfferName { get; set; }

    /// <summary>
    /// Gets or sets the plans list.
    /// </summary>
    /// <value>
    /// The plans list.
    /// </value>
    public SelectList PlansList { get; set; }

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
    /// Gets or sets the plan unique identifier.
    /// </summary>
    /// <value>
    /// The plan unique identifier.
    /// </value>
    public Guid PlanGUID { get; set; }

    /// <summary>
    /// Gets or sets the plan attributes.
    /// </summary>
    /// <value>
    /// The plan attributes.
    /// </value>
    public List<PlanAttributesModel> PlanAttributes { get; set; }

    /// <summary>
    /// Gets or sets the plan events.
    /// </summary>
    /// <value>
    /// The plan events.
    /// </value>
    public List<PlanEventsModel> PlanEvents { get; set; }
}