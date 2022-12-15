// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Marketplace.SaaS.Accelerator.Services.Exceptions;

/// <summary>
/// Fulfillment API Response ErrorCode
/// </summary>
public static class SaasApiErrorCode
{
    /// <summary>
    /// The bad request
    /// </summary>
    public const string BadRequest = "BadRequest";

    /// <summary>
    /// The unauthorized
    /// </summary>
    public const string Unauthorized = "Unauthorized";

    /// <summary>The conflict</summary>
    public const string Conflict = "Conflict";

    /// <summary>
    /// The not found
    /// </summary>
    public const string NotFound = "NotFound";

    /// <summary>
    /// The internal server error
    /// </summary>
    public const string InternalServerError = "UnexpectedError";

    /// <summary>
    /// The Accepted
    /// </summary>
    public const string Accepted = "Accepted";

    /// <summary>
    /// The NotProcessed
    /// </summary>
    public const string NotProcessed = "NotProcessed";

    /// <summary>
    /// The Expired
    /// </summary>
    public const string Expired = "Expired";

    /// <summary>
    /// The Duplicate
    /// </summary>
    public const string Duplicate = "Duplicate";

    /// <summary>
    /// The ResourceNotFound
    /// </summary>
    public const string ResourceNotFound = "ResourceNotFound";

    /// <summary>
    /// The ResourceNotAuthorized
    /// </summary>
    public const string ResourceNotAuthorized = "ResourceNotAuthorized";

    /// <summary>
    /// The InvalidDimension
    /// </summary>
    public const string InvalidDimension = "InvalidDimension";

    /// <summary>
    /// The BadArgument
    /// </summary>
    public const string BadArgument = "BadArgument";
}