namespace TsdDelivery.Application.Commons;

public class AppConfiguration
{
    public string DatabaseConnection { get; set; }
    public string RedisConnection { get; set; }
    public JwtSettings JwtSettings { get; set; }
    public FileService FileService { get; set; }
    public MomoConfig MomoConfig { get; set; }
    public ZaloPayConfig ZaloPayConfig { get; set; }
    public PaypalConfig PaypalConfig { get; set; }
    public Tomtom Tomtom { get; set; }
    public MailConfig MailConfig { get; set; }
    public ApiLayerConfig ApiLayerConfig { get; set; }
}

