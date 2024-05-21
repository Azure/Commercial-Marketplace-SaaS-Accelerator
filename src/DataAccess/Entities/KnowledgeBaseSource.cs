using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;
public class KnowledgeBaseSource
{
    public int Id { get; set; }
    public int? SubscriptionPlanId { get; set; }
    public string Source { get; set; }
    public Plans Plan { get; set; }
}
