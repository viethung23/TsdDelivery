namespace TsdDelivery.Application.Models.Service.Response;

public class ServiceResponseDetail
{
    public Guid Id { get; set; }
    public string ServiceName { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public bool IsShow { get; set; }
}