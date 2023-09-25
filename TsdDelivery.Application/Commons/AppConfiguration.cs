namespace TsdDelivery.Application.Commons;

public class AppConfiguration
{
    public string DatabaseConnection { get; set; }
    public JwtSettings JwtSettings { get; set; }
    public FileService FileService { get; set; }
    public MomoConfig MomoConfig { get; set; }
}

