// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;

namespace Marketplace.SaaS.Accelerator.Services.Models.Attributes;

/// <summary>
/// FromRequestHeaderAttribute to set the Header Key with each Request.
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class FromRequestHeaderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FromRequestHeaderAttribute"/> class.
    /// </summary>
    /// <param name="headerKey">The header key.</param>
    public FromRequestHeaderAttribute(string headerKey)
    {
        this.HeaderKey = headerKey;
    }

    /// <summary>
    /// Gets or sets the header key.
    /// </summary>
    /// <value>
    /// The header key.
    /// </value>
    public string HeaderKey { get; set; }
}