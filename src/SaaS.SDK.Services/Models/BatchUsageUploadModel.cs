namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;

    /// <summary>
    /// Batch Usage Upload Model.
    /// </summary>
    public class BatchUsageUploadModel
    {
        /// <summary>
        /// The subscriptionidheader.
        /// </summary>
        public const string SUBSCRIPTIONIDHEADER = "subscriptionid";

        /// <summary>
        /// The apitypeheader.
        /// </summary>
        public const string APITYPEHEADER = "apitype";

        /// <summary>
        /// The consumedunitsheader.
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

        /// <summary>
        /// Gets or sets the deployment parameter view model.
        /// </summary>
        /// <value>
        /// The deployment parameter view model.
        /// </value>
        public DeploymentParameterViewModel DeploymentParameterViewModel { get; set; }
    }
}
