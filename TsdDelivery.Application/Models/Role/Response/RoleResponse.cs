namespace TsdDelivery.Application.Models.Role.Response;

public class RoleResponse
{
    public Guid Id { get; set; }
    public string RoleName { get; set; }
    public DateTime CreationDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModificationDate { get; set; }
}
