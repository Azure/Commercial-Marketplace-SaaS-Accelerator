namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Response Model to return View Responses.
/// </summary>
public class ResponseModel
{
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is success.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.
    /// </value>
    public bool IsSuccess { get; set; }
}