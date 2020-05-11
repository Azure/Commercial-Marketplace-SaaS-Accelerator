namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    /// <summary>
    /// The Application Configuration Entity.
    /// </summary>
    public partial class ApplicationConfiguration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
