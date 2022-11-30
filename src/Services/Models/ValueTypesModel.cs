namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Value Types Model.
/// </summary>
public class ValueTypesModel
{
    /// <summary>
    /// Gets or sets the value type identifier.
    /// </summary>
    /// <value>
    /// The value type identifier.
    /// </value>
    public int ValueTypeId { get; set; }

    /// <summary>
    /// Gets or sets the type of the value.
    /// </summary>
    /// <value>
    /// The type of the value.
    /// </value>
    public string ValueType { get; set; }
}