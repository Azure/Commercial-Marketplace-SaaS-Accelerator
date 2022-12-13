using System;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Users Service.
/// </summary>
public class UserService
{
    /// <summary>
    /// The user repository.
    /// </summary>
    private IUsersRepository userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService" /> class.
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
    /// <returns> User id.</returns>
    public int AddUser(PartnerDetailViewModel partnerDetailViewModel)
    {
        if (!string.IsNullOrEmpty(partnerDetailViewModel.EmailAddress))
        {
            Users newPartnerDetail = new Users()
            {
                UserId = partnerDetailViewModel.UserId,
                EmailAddress = partnerDetailViewModel.EmailAddress,
                FullName = partnerDetailViewModel.FullName,
                CreatedDate = DateTime.Now,
            };
            return this.userRepository.Save(newPartnerDetail);
        }

        return 0;
    }

    /// <summary>
    /// Gets the user identifier from email address.
    /// </summary>
    /// <param name="partnerEmail">The partner email.</param>
    /// <returns>returns user id.</returns>
    public int GetUserIdFromEmailAddress(string partnerEmail)
    {
        if (!string.IsNullOrEmpty(partnerEmail))
        {
            return this.userRepository.GetPartnerDetailFromEmail(partnerEmail).UserId;
        }

        return 0;
    }
}