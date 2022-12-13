// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Exceptions;

/// <summary>
/// Metered Billing Exception.
/// </summary>
/// <seealso cref="System.ApplicationException" />
public class MarketplaceException : ApplicationException
{
    /// <summary>
    /// Gets or sets the Fulfiment error detail.
    /// </summary>
    /// <value>
    /// The metered billing error detail.
    /// </value>
    public MeteringErrorResult MeteredBillingErrorDetail { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketplaceException"/> class.
    /// </summary>
    public MarketplaceException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketplaceException" /> class.
    /// </summary>
    /// <param name="message">A message that describes the error.</param>
    public MarketplaceException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketplaceException" /> class.
    /// </summary>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="errorCode">The error code.</param>
    public MarketplaceException(string message, string errorCode)
        : base(message)
    {
        this.ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketplaceException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public MarketplaceException(string message, Exception inner)
        : base(message, inner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketplaceException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    /// <param name="errorCode">The error code.</param>
    public MarketplaceException(string message, string errorCode, Exception inner)
        : base(message, inner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketplaceException" /> class.
    /// </summary>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="meteredBillingErrorDetail">The metered billing error detail.</param>
    public MarketplaceException(string message, string errorCode, MeteringErrorResult meteredBillingErrorDetail)
        : this(message, errorCode)
    {
        this.MeteredBillingErrorDetail = meteredBillingErrorDetail;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarketplaceException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="meteredBillingErrorDetail">The metered billing error detail.</param>
    /// <param name="inner">The inner.</param>
    public MarketplaceException(string message, string errorCode, MeteringErrorResult meteredBillingErrorDetail, Exception inner)
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

        
}