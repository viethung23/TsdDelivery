﻿namespace TsdDelivery.Application.Models.User.Request;

public class UserCreateUpdate
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string RoleId { get; set; }
}
