namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    /// <summary>
    /// Child Parameter View Model.
    /// </summary>
    public class ChildParameterViewModel
    {
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        public string ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter data.
        /// </summary>
        /// <value>
        /// The type of the parameter data.
        /// </value>
        public string ParameterDataType { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>
        /// The type of the parameter.
        /// </value>
        public string ParameterType { get; set; }
    }
}
