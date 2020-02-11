namespace Microsoft.Marketplace.SaasKit.Client.Models
{
    using System;

    public class PartnerDetailViewModel
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FullName { get; set; }
    }
}
