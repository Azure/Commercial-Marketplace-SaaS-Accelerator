using System;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        string connStr = configuration["connectionstrings:blobstorage"];
        string container = configuration["saasapiconfiguration:containername"];
        containerClient = new BlobContainerClient(connStr, container);
    }

    public string GenerateSasUri(string blobName, int expiryMinutes = 15)
    {
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        if (!blobClient.Exists())
        {
            return null;
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

        return sasUri.ToString();
    }
}
