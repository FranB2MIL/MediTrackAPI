using Domain.Entities.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class Admin : IAuthenticatable
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Role Role => Role.Admin;

    
}