namespace  Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;

    public class UserService
    {
        /// <summary>
        /// The user repository
        /// </summary>
        public IUsersRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UserService(IUsersRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Adds the partner detail.
        /// </summary>
        /// <param name="partnerDetailViewModel">The partner detail view model.</param>
        /// <returns></returns>
        public int AddUser(PartnerDetailViewModel partnerDetailViewModel)
        {
            if (!string.IsNullOrEmpty(partnerDetailViewModel.EmailAddress))
            {
                Users newPartnerDetail = new Users()
                {
                    UserId = partnerDetailViewModel.UserId,
                    EmailAddress = partnerDetailViewModel.EmailAddress,
                    FullName = partnerDetailViewModel.FullName,
                    CreatedDate = DateTime.Now
                };
                return userRepository.Save(newPartnerDetail);
            }
            return 0;
        }

        /// <summary>
        /// Gets the user identifier from email address.
        /// </summary>
        /// <param name="partnerEmail">The partner email.</param>
        /// <returns></returns>
        public int GetUserIdFromEmailAddress(string partnerEmail)
        {
            if (!string.IsNullOrEmpty(partnerEmail))
                return userRepository.GetPartnerDetailFromEmail(partnerEmail).UserId;
            return 0;
        }
    }
}