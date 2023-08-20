using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.User.Request;

public class UserCreateUpdate
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string RoleId { get; set; }
}
