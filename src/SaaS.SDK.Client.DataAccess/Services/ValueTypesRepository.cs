namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Repository for value types
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IValueTypesRepository" />
    public class ValueTypesRepository : IValueTypesRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueTypesRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ValueTypesRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>
        /// List of all value types supported by the application
        /// </returns>
        public IEnumerable<ValueTypes> GetAll()
        {
            return context.ValueTypes;
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public ValueTypes GetById(int Id)
        {
            return context.ValueTypes.Where(s => s.ValueTypeId == Id).FirstOrDefault();

        }

        /// <summary>
        /// Gets the value type values.
        /// </summary>
        /// <returns>
        /// Returns all the values for value types
        /// </returns>
        public IEnumerable<string> GetValueTypeValues()
        {
            return context.ValueTypes.Select(s => s.ValueType);

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
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}