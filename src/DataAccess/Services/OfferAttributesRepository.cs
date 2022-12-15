using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository to access offer attributes.
/// </summary>
/// <seealso cref="IOfferAttributesRepository" />
public class OfferAttributesRepository : IOfferAttributesRepository
{
    /// <summary>
    /// The this.context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferAttributesRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public OfferAttributesRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <param name="offerAttributes">The offer attributes.</param>
    /// <returns>
    /// Id of the newly added offer attribute.
    /// </returns>
    public int? Add(OfferAttributes offerAttributes)
    {
        if (offerAttributes != null)
        {
            var existingOfferAttribute = this.context.OfferAttributes.Where(s => s.Id ==
                offerAttributes.Id).FirstOrDefault();
            if (existingOfferAttribute != null)
            {
                existingOfferAttribute.ParameterId = offerAttributes.ParameterId;
                existingOfferAttribute.DisplayName = offerAttributes.DisplayName;
                existingOfferAttribute.Description = offerAttributes.Description;
                existingOfferAttribute.ValueTypeId = offerAttributes.ValueTypeId;
                existingOfferAttribute.FromList = offerAttributes.FromList;
                existingOfferAttribute.ValuesList = offerAttributes.ValuesList;
                existingOfferAttribute.Max = offerAttributes.Max;
                existingOfferAttribute.Min = offerAttributes.Min;
                existingOfferAttribute.Type = offerAttributes.Type;
                existingOfferAttribute.DisplaySequence = offerAttributes.DisplaySequence;
                existingOfferAttribute.Isactive = offerAttributes.Isactive;
                existingOfferAttribute.UserId = offerAttributes.UserId;
                existingOfferAttribute.OfferId = offerAttributes.OfferId;
                existingOfferAttribute.IsRequired = offerAttributes.IsRequired;
                existingOfferAttribute.IsDelete = offerAttributes.IsDelete;

                this.context.OfferAttributes.Update(existingOfferAttribute);
                this.context.SaveChanges();
                return existingOfferAttribute.Id;
            }
            else
            {
                this.context.OfferAttributes.Add(offerAttributes);
                this.context.SaveChanges();
                return offerAttributes.Id;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all offer attributes across offers.
    /// </summary>
    /// <returns>
    /// List of offer attributes across offers.
    /// </returns>
    public IEnumerable<OfferAttributes> GetAll()
    {
        return this.context.OfferAttributes.Where(s => s.IsDelete != true && s.Type.ToLower() == "input");
    }

    /// <summary>
    /// Gets the input attributes by offer identifier.
    /// </summary>
    /// <param name="offerGuId">The offer gu identifier.</param>
    /// <returns> list of OfferAttributes.</returns>
    public IEnumerable<OfferAttributes> GetInputAttributesByOfferId(Guid offerGuId)
    {
        return this.context.OfferAttributes.Where(s => s.OfferId == offerGuId && (s.IsDelete != true) && s.Type.ToLower() == "input");
    }

    /// <summary>
    /// Gets all offer attributes by offer identifier ( includes deployment and input attributes ).
    /// </summary>
    /// <param name="offerGuId">The offer identifier.</param>
    /// <returns>
    /// List of offer attributes.
    /// </returns>
    public IEnumerable<OfferAttributes> GetAllOfferAttributesByOfferId(Guid offerGuId)
    {
        return this.context.OfferAttributes.Where(s => s.OfferId == offerGuId && (s.IsDelete != true));
    }

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> Offer Attributes.</returns>
    public OfferAttributes GetById(int id)
    {
        return this.context.OfferAttributes.Where(s => s.Id == id && s.IsDelete != true && s.Type.ToLower() == "input").FirstOrDefault();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.context.Dispose();
            }
        }

        this.disposed = true;
    }
}