using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.User.Request;

public class LoginRequest
{
    [Required]
    [MinLength(5)]
    [MaxLength(11)]
    public string PhoneNumber { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string RoleId { get; set; }
}
