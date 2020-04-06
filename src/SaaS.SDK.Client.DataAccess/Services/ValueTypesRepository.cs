using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class ValueTypesRepository : IValueTypesRepository
    {
        private readonly SaasKitContext Context;

        public ValueTypesRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        public IEnumerable<ValueTypes> GetValueTypes()
        {

            return Context.ValueTypes;
        }
        public ValueTypes GetValuetypeOnId(int Id)
        {
            return Context.ValueTypes.Where(s => s.ValueTypeId == Id).FirstOrDefault();

        }


        public IEnumerable<string> GetValueTypesValues()
        {
            return Context.ValueTypes.Select(s => s.ValueType);

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
                    Context.Dispose();
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
