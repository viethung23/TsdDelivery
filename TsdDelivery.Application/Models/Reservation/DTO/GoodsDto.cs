using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.Reservation.DTO;

public class GoodsDto
{
    public string Name { get; set; }
    
    [Range(0,999999)]
    public decimal? Weight { get; set; }
    
    [Range(0,50)]
    public decimal? Length { get; set; }
    
    [Range(0,50)]
    public decimal? Width { get; set; }
    
    [Range(0,50)]
    public decimal? Height { get; set; }
}