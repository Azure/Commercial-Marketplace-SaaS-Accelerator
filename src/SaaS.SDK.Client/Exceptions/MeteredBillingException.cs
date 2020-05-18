namespace Microsoft.Marketplace.SaasKit.Exceptions
{
    using System;
    using Microsoft.Marketplace.SaasKit.Models;

    /// <summary>
    /// Metered Billing Exception.
    /// </summary>
    /// <seealso cref="System.ApplicationException" />
    public class MeteredBillingException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingException"/> class.
        /// </summary>
        public MeteredBillingException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public MeteredBillingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="errorCode">The error code.</param>
        public MeteredBillingException(string message, string errorCode)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="meteredBillingErrorDetail">The metered billing error detail.</param>
        public MeteredBillingException(string message, string errorCode, MeteringErrorResult meteredBillingErrorDetail)
            : this(message, errorCode)
        {
            this.MeteredBillingErrorDetail = meteredBillingErrorDetail;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MeteredBillingException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteredBillingException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="meteredBillingErrorDetail">The metered billing error detail.</param>
        /// <param name="inner">The inner.</param>
        public MeteredBillingException(string message, string errorCode, MeteringErrorResult meteredBillingErrorDetail, System.Exception inner)
            : this(message, inner)
        {
            this.ErrorCode = errorCode;
            this.MeteredBillingErrorDetail = meteredBillingErrorDetail;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the metered billing error detail.
        /// </summary>
        /// <value>
        /// The metered billing error detail.
        /// </value>
        public MeteringErrorResult MeteredBillingErrorDetail { get; set; }
    }
}
