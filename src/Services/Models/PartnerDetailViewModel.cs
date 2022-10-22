using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Partner Detail ViewModel.
/// </summary>
public class PartnerDetailViewModel
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    /// <value>
    /// The email address.
    /// </value>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    /// <value>
    /// The created date.
    /// </value>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    /// <value>
    /// The full name.
    /// </value>
    public string FullName { get; set; }
}