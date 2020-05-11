namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;

    /// <summary>
    /// Subscription Template Parameters Model.
    /// </summary>
    public class SubscriptionTemplateParametersModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the offer.
        /// </summary>
        /// <value>
        /// The name of the offer.
        /// </value>
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets the offer unique identifier.
        /// </summary>
        /// <value>
        /// The offer unique identifier.
        /// </value>
        public Guid? OfferGuid { get; set; }

        /// <summary>
        /// Gets or sets the plan unique identifier.
        /// </summary>
        /// <value>
        /// The plan unique identifier.
        /// </value>
        public Guid? PlanGuid { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        public string PlanId { get; set; }

        /// <summary>
        /// Gets or sets the armtemplate identifier.
        /// </summary>
        /// <value>
        /// The armtemplate identifier.
        /// </value>
        public Guid? ArmtemplateId { get; set; }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string Parameter { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter data.
        /// </summary>
        /// <value>
        /// The type of the parameter data.
        /// </value>
        public string ParameterDataType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>
        /// The type of the parameter.
        /// </value>
        public string ParameterType { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public int? EventId { get; set; }

        /// <summary>
        /// Gets or sets the name of the events.
        /// </summary>
        /// <value>
        /// The name of the events.
        /// </value>
        public string EventsName { get; set; }

        /// <summary>
        /// Gets or sets the ampsubscription identifier.
        /// </summary>
        /// <value>
        /// The ampsubscription identifier.
        /// </value>
        public Guid? AmpsubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the subscription status.
        /// </summary>
        /// <value>
        /// The subscription status.
        /// </value>
        public string SubscriptionStatus { get; set; }

        /// <summary>
        /// Gets or sets the name of the subscription.
        /// </summary>
        /// <value>
        /// The name of the subscription.
        /// </value>
        public string SubscriptionName { get; set; }

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
