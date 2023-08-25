using System.ComponentModel.DataAnnotations;
using TsdDelivery.Application.Models.Reservation.DTO;

namespace TsdDelivery.Application.Models.Reservation.Request;

public class CreateReservationRequest
{
    [Required]
    public string SendLocation { get; set; }
    
    [Required]
    public string ReciveLocation { get; set; }
    
    [Required]
    public string RecipientName { get; set; }
    
    [Required]
    public string RecipientPhone { get; set;}
    
    [Required]
    [Range(0.1f,9999)]
    public decimal Distance { get; set; }
    
    [Required]
    public DateTime PickUpDateTime { get; set; }
    
    [Required]
    public GoodsDto GoodsDto { get; set; } = new GoodsDto();
    
    [Required]
    [Range(1,99999999)]
    public decimal TotalPrice { get; set; }
    
    [Required]
    public List<Guid> ServiceIds { get; set; }
}