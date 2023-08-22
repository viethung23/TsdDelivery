namespace TsdDelivery.Application.Models.Service.Response;

public class ServiceResponse
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
}