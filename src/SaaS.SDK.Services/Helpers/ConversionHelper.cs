// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.
namespace Microsoft.Marketplace.SaaS.SDK.Services.Helpers
{
    using Microsoft.Marketplace.SaaS.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using System.Collections.Generic;

    static class ConversionHelper
    {
        public static List<SubscriptionResult> subscriptionResultList(this List<Subscription> subscriptions) {
            return new List<SubscriptionResult>();
        }

        public static SubscriptionResult subscriptionResult(this Subscription subscription)
        {
            return new SubscriptionResult();
        }

        public static ResolvedSubscriptionResult resolvedSubscriptionResult(this ResolvedSubscription resolvedSubscription)
        {
            return new ResolvedSubscriptionResult();
        }

        public static List<PlanDetailResult> planResults(this SubscriptionPlans subscriptionPlans)
        {
            return new List<PlanDetailResult>();
        }

        public static OperationResult operationResult(this Operation operation)
        {
            return new OperationResult();
        }
    }
}
