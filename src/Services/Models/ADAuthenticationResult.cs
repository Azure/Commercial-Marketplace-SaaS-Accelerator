// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Azure Active Directory authentication token result.
/// </summary>
public class ADAuthenticationResult
{
    /// <summary>
    /// Gets or sets the type of the token.
    /// </summary>
    /// <value>
    /// The type of the token.
    /// </value>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the expires in.
    /// </summary>
    /// <value>
    /// The expires in.
    /// </value>
    [JsonPropertyName("expires_in")]
    public string ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the ext expires in.
    /// </summary>
    /// <value>
    /// The ext expires in.
    /// </value>
    [JsonPropertyName("ext_expires_in")]
    public string ExtExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the expires on.
    /// </summary>
    /// <value>
    /// The expires on.
    /// </value>
    [JsonPropertyName("expires_on")]
    public string ExpiresOn { get; set; }

    /// <summary>
    /// Gets or sets the not before.
    /// </summary>
    /// <value>
    /// The not before.
    /// </value>
    [JsonPropertyName("not_before")]
    public string NotBefore { get; set; }

    /// <summary>
    /// Gets or sets the resource.
    /// </summary>
    /// <value>
    /// The resource.
    /// </value>
    [JsonPropertyName("resource")]
    public Guid Resource { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    /// <value>
    /// The access token.
    /// </value>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}