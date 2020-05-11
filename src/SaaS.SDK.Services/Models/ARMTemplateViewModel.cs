namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;

    /// <summary>
    /// ARMTemplate View Model.
    /// </summary>
    public class ARMTemplateViewModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the armtempalte identifier.
        /// </summary>
        /// <value>
        /// The armtempalte identifier.
        /// </value>
        public Guid? ArmtempalteId { get; set; }

        /// <summary>
        /// Gets or sets the name of the armtempalte.
        /// </summary>
        /// <value>
        /// The name of the armtempalte.
        /// </value>
        public string ArmtempalteName { get; set; }

        /// <summary>
        /// Gets or sets the template location.
        /// </summary>
        /// <value>
        /// The template location.
        /// </value>
        public string TemplateLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ARMTemplateViewModel"/> is isactive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if isactive; otherwise, <c>false</c>.
        /// </value>
        public bool Isactive { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int? UserId { get; set; }
    }
}
