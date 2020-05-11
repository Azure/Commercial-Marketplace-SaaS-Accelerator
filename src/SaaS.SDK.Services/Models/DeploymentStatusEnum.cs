namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    /// <summary>
    /// Deployment Status Enum.
    /// </summary>
    public enum DeploymentStatusEnum
    {
        /// <summary>
        /// The pending fulfillment start
        /// </summary>
        ARMTemplateDeploymentPending,

        /// <summary>
        /// The subscribed.
        /// </summary>
        ARMTemplateDeploymentSuccess,

        /// <summary>
        /// The arm template deployment failure.
        /// </summary>
        ARMTemplateDeploymentFailure,

        /// <summary>
        /// The unsubscribed.
        /// </summary>
        DeleteResourceGroupPending,

        /// <summary>
        /// The not started.
        /// </summary>
        DeleteResourceGroupSuccess,

        /// <summary>
        /// The delete resource group failure.
        /// </summary>
        DeleteResourceGroupFailure,
    }
}
