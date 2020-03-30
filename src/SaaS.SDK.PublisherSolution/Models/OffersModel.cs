using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.Saas.Web.Models
{
    public class OffersModel
    {
        public string offerID { get; set; }

        public string offerName { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? UserID { get; set; }
    }
}
