using Microsoft.AspNetCore.Http;

namespace TsdDelivery.Application.Interface;

public interface IBlobStorageAzureService
{
    public Task<string?> SaveImageAsync (IFormFile blob);
}
