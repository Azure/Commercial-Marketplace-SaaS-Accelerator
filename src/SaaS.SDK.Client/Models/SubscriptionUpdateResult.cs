namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using Microsoft.Marketplace.SaasKit.Attributes;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// Subscription Update Result.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
    public class SubscriptionUpdateResult : SaaSApiResult
    {
        /// <summary>
        /// Gets or sets the operation location.
        /// </summary>
        /// <value>
        /// The operation location.
        /// </value>
        [FromRequestHeader("operation-location")]
        public string OperationLocation { get; set; }

        /// <summary>
        /// Gets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        /// <exception cref="FulfillmentException">
        /// API did not return an operation ID.
        /// or
        /// URI is not recognized as an operation ID url.
        /// or
        /// Returned operation ID is not a Guid.
        /// </exception>
        [FromRequestHeader("OperationId")]
        public Guid OperationId
        {
            get
            {
                Uri operationUri;
                Guid operationGuid;
                Uri.TryCreate(this.OperationLocation, UriKind.Absolute, out operationUri);

                if (operationUri == default)
                {
                    throw new FulfillmentException("API did not return an operation ID", SaasApiErrorCode.NotFound);
                }

                // The URI should be like https://marketplaceapi.microsoft.com/api/saas/subscriptions/1be86829-c7ec-1738-ab03-a6cacebe3832/operations/ed10f0b7-6cd6-416d-b015-83c11c9f083b?api-version=2018-08-31
                // So segments should look like
                /*
                    /
                    api/
                    saas/
                    subscriptions/
                    1be86829-c7ec-1738-ab03-a6cacebe3832/
                    operations/
                    ed10f0b7-6cd6-416d-b015-83c11c9f083b
                */

                if (operationUri.Segments.Length != 7)
                {
                    throw new FulfillmentException("URI is not recognized as an operation ID url", SaasApiErrorCode.NotFound);
                }

                if (!Guid.TryParse(operationUri.Segments[6], out operationGuid))
                {
                    throw new FulfillmentException("Returned operation ID is not a Guid", SaasApiErrorCode.NotFound);
                }

                return operationGuid;
            }
        }
    }
}
