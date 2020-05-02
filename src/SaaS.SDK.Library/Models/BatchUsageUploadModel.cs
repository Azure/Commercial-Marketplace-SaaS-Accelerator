namespace Microsoft.Marketplace.SaaS.SDK.Library.Models
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System.Collections.Generic;

    /// <summary>
    /// Batch Usage Upload Model
    /// </summary>
    public class BatchUsageUploadModel
    {
        /// <summary>
        /// The subscriptionidheader
        /// </summary>
        public const string SUBSCRIPTIONIDHEADER = "subscriptionid";

        /// <summary>
        /// The apitypeheader
        /// </summary>
        public const string APITYPEHEADER = "apitype";

        /// <summary>
        /// The consumedunitsheader
        /// </summary>
        public const string CONSUMEDUNITSHEADER = "consumedunits";

        /// <summary>
        /// Gets the uploaded header strings.
        /// </summary>
        /// <value>
        /// The uploaded header strings.
        /// </value>
        public List<string> UploadedHeaderStrings
        {
            get
            {
                var headerList = new List<string>();
                headerList.Add(SUBSCRIPTIONIDHEADER);
                headerList.Add(APITYPEHEADER);
                headerList.Add(CONSUMEDUNITSHEADER);
                return headerList;
            }
        }

        /// <summary>
        /// Gets or sets the bulk upload usage stagings.
        /// </summary>
        /// <value>
        /// The bulk upload usage stagings.
        /// </value>
        public List<BulkUploadUsageStagingResult> BulkUploadUsageStagings { get; set; }

        /// <summary>
        /// Gets or sets the batch log identifier.
        /// </summary>
        /// <value>
        /// The batch log identifier.
        /// </value>
        public int BatchLogId { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public ResponseModel Response { get; set; }

        public DeploymentParameterViewModel DeploymentParameterViewModel { get; set; }
    }
}
