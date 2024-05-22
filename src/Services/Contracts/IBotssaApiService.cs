using Marketplace.SaaS.Accelerator.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;
public interface IBotssaApiService
{
    Task<string> GetAuthTokenAsync();
    Task<PromoCodeResult> ValidatePromoCodeAsync(string promoCode);
}
