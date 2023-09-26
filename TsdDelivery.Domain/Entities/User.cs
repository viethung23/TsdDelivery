namespace TsdDelivery.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public string? AvatarUrl { get;set; }

    public Guid? RoleId { get; set; }
    public Role? Role { get; set; }

    public ICollection<Vehicle?> Vehicles { get; set; }
    public ICollection<Reservation?> ReservationUsers { get; set; }
    public ICollection<Reservation?> ReservationDrivers { get; set; }
    public ICollection<Wallet?> Wallets { get; set; }
}
