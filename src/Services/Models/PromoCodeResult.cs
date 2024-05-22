using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Models;
public class PromoCodeResult
{
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public int UsageCount { get; set; }
    public int UsageAllocation { get; set; }
    public int Duration { get; set; }
}

public class ApiResult<T>
{
    public T Result { get; set; }
    public bool Success { get; set; }
    public string Error { get; set; }
}