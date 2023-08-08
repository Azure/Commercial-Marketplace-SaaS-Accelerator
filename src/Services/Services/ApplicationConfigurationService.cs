using System;
using System.Collections.Generic;
using System.IO;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.AspNetCore.Http;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Application Config Service.
/// </summary>
public class ApplicationConfigService
{

    /// <summary>
    /// The app config repository.
    /// </summary>
    private IApplicationConfigRepository appConfigRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationConfigurationService"/> class.
    /// </summary>
    /// <param name="ApplicationConfigRepository">The application config repository.</param>
    public ApplicationConfigService(IApplicationConfigRepository applicationConfigRepository)
    {
        this.appConfigRepository = applicationConfigRepository;
    }

    /// <summary>
    /// Get all Application Config items.
    /// </summary>
    public IEnumerable<ApplicationConfiguration> GetAllApplicationConfiguration()
    {
        return this.appConfigRepository.GetAll();
    }

    /// <summary>
    /// Save Application Config item.
    /// </summary>
    public int? SaveAppConfig(ApplicationConfiguration appConfig)
    {
        if (appConfig != null)
        {
            return this.appConfigRepository.SaveById(appConfig);
        }

        return null;
    }

    /// <summary>
    /// Get an Application Config item by Id.
    /// </summary>
    public ApplicationConfiguration GetById(int Id)
    {
        return this.appConfigRepository.GetById(Id);
    }

    /// <summary> 
    /// Save file to disk.
    /// </summary>
    /// <param name="configName">The app config name.</param>
    /// <param name="fileName">The file name.</param>
    public void SaveFileToDisk(string configName, string fileName)
    {
        var base64String = this.appConfigRepository.GetValueByName(configName);
        if (!String.IsNullOrEmpty(base64String))
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
            var bytes = Convert.FromBase64String(base64String);
            using (var imageFile = new FileStream(filepath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }
        }
    }

    /// <summary> 
    /// Upload file to database.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="fileExtension">The file extension.</param>
    /// <returns>True or false.</returns>
    public bool UploadFileToDatabase(IFormFile file, string fileExtension)
    {
        using (var ms = new MemoryStream())
        {
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            var base64String = Convert.ToBase64String(fileBytes);

            if (fileExtension == ".png")
            {
                return this.appConfigRepository.SaveValueByName("LogoFile", base64String);
            }
            else if (fileExtension == ".ico")
            {
                return this.appConfigRepository.SaveValueByName("FaviconFile", base64String);
            }
            return false;
        }
    }

    public string GetValueByName(string configName)
    {
        return this.appConfigRepository.GetValueByName(configName);
    }
}