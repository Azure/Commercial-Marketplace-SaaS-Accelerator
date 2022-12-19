using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Models;

public class OfferAttributesModel
{
    public Guid OfferId { get; set; }
    public int Id { get; set; }
    public DateTime? CreateDate { get; set; }
    public string Description { get; set; }
    public int? UserId { get; set; }
    public bool? IsDelete { get; set; }
    public string DisplayName { get; set; }
    public bool FromList { get; set; }
    public int? DisplaySequence { get; set; }
    public bool? IsRequired { get; set; }
    public bool IsActive { get; set; }
    public string ParameterId { get; set; }
    public int? ValueTypeId { get; set; }
    public string ValuesList { get; set; }
    public int? Max { get; set; }
    public int? Min { get; set; }
    public string Type { get; set; }
}
