using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

/// <summary>
/// Repository for value types.
/// </summary>
public class ValueTypesRepository : IValueTypesRepository
{
    /// <summary>
    /// The context.
    /// </summary>
    private readonly SaasKitContext context;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueTypesRepository"/> class.
    /// </summary>
    /// <param name="context">The this.context.</param>
    public ValueTypesRepository(SaasKitContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <returns>
    /// List of all value types supported by the application.
    /// </returns>
    public IEnumerable<ValueTypes> GetAll()
    {
        return this.context.ValueTypes;
    }

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns> Value Types.</returns>
    public ValueTypes GetById(int id)
    {
        return this.context.ValueTypes.Where(s => s.ValueTypeId == id).FirstOrDefault();
    }

    /// <summary>
    /// Gets the value type values.
    /// </summary>
    /// <returns>
    /// Returns all the values for value types.
    /// </returns>
    public IEnumerable<string> GetValueTypeValues()
    {
        return this.context.ValueTypes.Select(s => s.ValueType);
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