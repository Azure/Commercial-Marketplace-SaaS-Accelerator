// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Term UnitEnum.
/// </summary>
public enum SchedulerFrequencyEnum
{
    /// <summary>
    /// The Hourly.
    /// </summary>
    Hourly = 1,

    /// <summary>
    /// The Daily.
    /// </summary>
    Daily,

    /// <summary>
    /// The Weekly.
    /// </summary>
    Weekly,

    /// <summary>
    /// The Monthly.
    /// </summary>
    Monthly,

    /// <summary>
    /// The Yearly.
    /// </summary>
    Yearly,

    /// <summary>
    /// The OneTime.
    /// </summary>
    OneTime
}