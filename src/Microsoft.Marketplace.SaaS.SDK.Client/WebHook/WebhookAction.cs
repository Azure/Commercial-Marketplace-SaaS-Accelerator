﻿namespace Microsoft.Marketplace.SaasKit.WebHook
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Enum Representation for Web hook Action
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WebhookAction
    {
        /// (When the resource has been deleted)
        /// <summary>
        /// The unsubscribe
        /// </summary>
        [EnumMember(Value = "Unsubscribe")]
        Unsubscribe,

        /// (When the change plan operation has completed)
        /// <summary>
        /// The change plan
        /// </summary>
        [EnumMember(Value = "ChangePlan")]
        ChangePlan,

        /// (When the change quantity operation has completed),
        /// <summary>
        /// The change quantity
        /// </summary>
        [EnumMember(Value = "ChangeQuantity")]
        ChangeQuantity,

        /// (When resource has been suspended)
        /// <summary>
        /// The suspend
        /// </summary>
        [EnumMember(Value = "Suspend")]
        Suspend,

        /// (When resource has been reinstated after suspension)
        /// <summary>
        /// The reinstate
        /// </summary>
        [EnumMember(Value = "Reinstate")]
        Reinstate,

        /// <summary>
        /// The transfer
        /// </summary>
        [EnumMember(Value = "Transfer")]
        Transfer
    }
}
