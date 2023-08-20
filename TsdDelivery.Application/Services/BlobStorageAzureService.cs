using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.AspNetCore.Http;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;
using Azure.Storage.Blobs.Models;

namespace TsdDelivery.Application.Services;

public class BlobStorageAzureService : IBlobStorageAzureService
{
    private readonly AppConfiguration _configuration;
        private readonly BlobContainerClient _fileContainer;

    public BlobStorageAzureService(AppConfiguration appConfiguration)
    {
        _configuration = appConfiguration;
        var credential = new StorageSharedKeyCredential(_configuration.FileService.StorageAccount, _configuration.FileService.Key);
        var blobUri = $"https://{_configuration.FileService.StorageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
        _fileContainer = blobServiceClient.GetBlobContainerClient("images");
    }

    public async Task<string?> SaveImageAsync(IFormFile blob)
    {
        try
        {
            BlobClient client = _fileContainer.GetBlobClient(blob.FileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data, httpHeaders: new BlobHttpHeaders { ContentType = "image/png" }, conditions: null);
            }
            return client.Uri.AbsoluteUri;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
