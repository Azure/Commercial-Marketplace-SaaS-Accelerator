// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class MeteringUsageRequestAttributes
{
    public double Quantity { get; set; }
    public Guid ResourceId { get; set; }
    public string Dimension { get; set; }
    public DateTime EffectiveStartTime { get; set; }
    public string PlanId { get; set; }
}