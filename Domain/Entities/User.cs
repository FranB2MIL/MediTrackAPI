

using Domain.Entities.Enums;

namespace Domain.Entities;
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public Admin? Admin { get; set; }
    
    public IEnumerable<Role> Roles
    {
        get
        {
            var roles = new List<Role>();
            if (Admin != null) roles.Add(Role.Admin);
            if (Doctor != null) roles.Add(Role.Doctor);
            if (Patient != null) roles.Add(Role.Patient);
            return roles;
        }
    }
}