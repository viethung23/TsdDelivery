using System.ComponentModel.DataAnnotations;
using TsdDelivery.Application.Models.Reservation.DTO;

namespace TsdDelivery.Application.Models.Reservation.Request;

public class CreateReservationRequest
{
    [Required]
    public string SendLocation { get; set; }
    
    [Required]
    [Range(-90, 90, ErrorMessage = "Invalid latitude value")]
    public double latitudeSendLocation { get; set; }
    
    [Required]
    [Range(-180, 180, ErrorMessage = "Invalid longitude value")]
    public double longitudeSendLocation { get; set; }
    
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
    public bool IsNow { get; set; }
    
    public DateTime? PickUpDateTime { get; set; }
    
    [Required]
    public GoodsDto GoodsDto { get; set; } = new GoodsDto();
    
    [Required]
    [Range(1,99999999)]
    public decimal TotalPrice { get; set; }
    
    [Required]
    public List<Guid> ServiceIds { get; set; }
    
    
    [Required]
    public string PaymentMethod { get; set; } 
}