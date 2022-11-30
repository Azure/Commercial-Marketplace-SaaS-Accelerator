namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Email Content Model.
/// </summary>
public class EmailContentModel
{
    /// <summary>
    /// Gets or sets from email.
    /// </summary>
    /// <value>
    /// From email.
    /// </value>
    public string FromEmail { get; set; }

    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    /// <value>
    /// The name of the user.
    /// </value>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    /// <value>
    /// The port.
    /// </value>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="EmailContentModel"/> is SSL.
    /// </summary>
    /// <value>
    ///   <c>true</c> if SSL; otherwise, <c>false</c>.
    /// </value>
    public bool SSL { get; set; }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>
    /// The subject.
    /// </value>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the SMTP host.
    /// </summary>
    /// <value>
    /// The SMTP host.
    /// </value>
    public string SMTPHost { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    /// <value>
    /// The body.
    /// </value>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets converts to emails.
    /// </summary>
    /// <value>
    /// To emails.
    /// </value>
    public string ToEmails { get; set; }

    /// <summary>
    /// Gets or sets the cc emails.
    /// </summary>
    /// <value>
    /// The cc emails.
    /// </value>
    public string CCEmails { get; set; }

    /// <summary>
    /// Gets or sets the BCC emails.
    /// </summary>
    /// <value>
    /// The BCC emails.
    /// </value>
    public string BCCEmails { get; set; }

    /// <summary>
    /// Gets or sets the customer email.
    /// </summary>
    /// <value>
    /// The customer email.
    /// </value>
    public string CustomerEmail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [copy to customer].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [copy to customer]; otherwise, <c>false</c>.
    /// </value>
    public bool CopyToCustomer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive { get; set; }
}