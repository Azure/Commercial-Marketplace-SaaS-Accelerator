// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.Services.Models;

/// <summary>
/// Plan Details.
/// </summary>
/// <seealso cref="Microsoft.Marketplace.SaasKit.Models.SaaSApiResult" />
public class PlanDetailResult : SaaSApiResult
{

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
        
    public int Id { get; set; }
        
    /// <summary>
    /// Gets or sets the plan identifier.
    /// </summary>
    /// <value>
    /// The plan identifier.
    /// </value>
    [JsonPropertyName("planId")]
    [DisplayName("planId")]
    public string PlanId { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    [JsonPropertyName("displayName")]
    [DisplayName("displayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value to describe the plan
    /// </summary>
    /// <value>
    ///   Description
    /// </value>
    [JsonPropertyName("description")]
    [DisplayName("description")]
    public string Description { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether this instance is private.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is private; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("isPrivate")]
    [DisplayName("isPrivate")]
    public bool IsPrivate { get; set; }


    /// <summary>
    /// Gets or sets a value indicating whether this instance has free trials.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is private; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("hasFreeTrials")]
    [DisplayName("hasFreeTrials")]
    public bool? HasFreeTrials { get; set; }

/*
        /// <summary>
        /// Gets or sets a value indicating whether this instance is price per seat.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is private; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("isPricePerSeat")]
        [DisplayName("isPricePerSeat")]
        public bool? IsPricePerSeat { get; set; }
*/




    /// <summary>
    /// Gets or sets a value indicating whether this instance is inactive
    /// </summary>
    ///  <value>
    ///   <c>true</c> if this instance is private; otherwise, <c>false</c>.
    /// </value>
    [JsonPropertyName("isStopSell")]
    [DisplayName("isStopSell")]
    public bool? IsStopSell { get; set; }


    /// <summary>
    /// Gets or sets value for market target
    /// </summary>
    ///  <value>
    ///   target market
    /// </value>
    [JsonPropertyName("market")]
    [DisplayName("market")]
    public string Market { get; set; }



    [JsonPropertyName("planComponents")]
    [DisplayName("planComponents")]
    /// <summary>
    /// Gets or sets the plan component associate with the plan.
    /// </summary>
    /// <value>
    /// The Plan Components list.
    /// </value>
    public  PlanComponents PlanComponents { get; set; }


    /// <summary>
    /// Get  IsmeteringSupported associate with the plan.
    /// </summary>
    public bool? IsmeteringSupported {
        get
        {
            if (!m_IsmeteringSupported && this.PlanComponents != null && this.PlanComponents.MeteringDimensions.Count > 0)
            {
                m_IsmeteringSupported = true;
            }

            return m_IsmeteringSupported;
        }
    }

    /// <summary>
    /// Set IsmeteringSupported  in case of Meter Dimension is part of plan
    /// </summary>
    /// <value>
    /// set true if there is Meter Dimension count greater than 1 otherwise it is false
    /// </value>
    private bool m_IsmeteringSupported = false;



    /// <summary>
    /// Get Meter Dimensions list
    /// </summary>
    /// <returns>
    /// A list of Meter Dimensions
    /// </returns>
    public List<MeteredDimensions> GetmeteredDimensions()
    {
        List<MeteredDimensions> meteredDimensions = new List<MeteredDimensions>();

        if (this.PlanComponents != null && this.PlanComponents.MeteringDimensions.Count > 0)
        {

            foreach (MeteringDimension meterDim in PlanComponents.MeteringDimensions)
            {
                meteredDimensions.Add(
                    new MeteredDimensions()
                    {
                        Dimension = meterDim.Id,
                        Description = meterDim.DisplayName
                    }) ;
                m_IsmeteringSupported = true;
            }
        }

        return meteredDimensions;
    }
        


}