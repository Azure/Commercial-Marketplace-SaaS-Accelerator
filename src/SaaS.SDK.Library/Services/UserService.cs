using System;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;

namespace  Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class UserService
    {
        /// <summary>
        /// The user repository
        /// </summary>
        public IUsersRepository UserRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UserService(IUsersRepository userRepository)
        {
            UserRepository = userRepository;
        }

        /// <summary>
        /// Adds the partner detail.
        /// </summary>
        /// <param name="partnerDetailViewModel">The partner detail view model.</param>
        /// <returns></returns>
        public int AddPartnerDetail(PartnerDetailViewModel partnerDetailViewModel)
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
                return UserRepository.Add(newPartnerDetail);
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
                return UserRepository.GetPartnerDetailFromEmail(partnerEmail).UserId;
            return 0;
        }
    }
}
