using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Provisioning.Webjob.Models
{
    public enum DeploymentStatusEnum
    {
        /// <summary>
        /// The pending fulfillment start
        /// </summary>
        ARMTemplateDeploymentPending,

        /// <summary>
        /// The subscribed
        /// </summary>
        ARMTemplateDeploymentSuccess,

        ARMTemplateDeploymentFailure,

        /// <summary>
        /// The unsubscribed
        /// </summary>
        DeleteResourceGroupPending,

        /// <summary>
        /// The not started
        /// </summary>
        DeleteResourceGroupSuccess,

        DeleteResourceGroupFailure

    }
}
