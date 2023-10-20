using Microsoft.AspNetCore.Http;

namespace TsdDelivery.Application.Interface.V1;

public interface IBlobStorageAzureService
{
    public Task<string?> SaveImageAsync (IFormFile blob);
}
