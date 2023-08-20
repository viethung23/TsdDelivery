using System.ComponentModel.DataAnnotations;

namespace TsdDelivery.Application.Models.Role.Request;

public class RoleCreateUpdate
{
    [Required]
    public string RoleName { get; set; }
}
