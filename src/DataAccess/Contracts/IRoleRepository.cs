using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;
public interface IRoleRepository : IDisposable, IBaseRepository<Roles>
{
    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <returns>
    /// All roles.
    /// </returns>
    public IEnumerable<Roles> GetAllRoles();

}
