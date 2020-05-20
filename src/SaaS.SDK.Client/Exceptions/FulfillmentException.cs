// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;

    /// <summary>
    /// Fulfillment API Exception.
    /// </summary>
    /// <seealso cref="System.ApplicationException" />
    public class FulfillmentException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentException"/> class.
        /// </summary>
        public FulfillmentException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public FulfillmentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentException" /> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="errorCode">The error code.</param>
        public FulfillmentException(string message, string errorCode)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public FulfillmentException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FulfillmentException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="inner">The inner.</param>
        public FulfillmentException(string message, string errorCode, System.Exception inner)
            : base(message, inner)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode { get; set; }
    }
}
