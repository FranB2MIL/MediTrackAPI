using Domain.Entities.Enums;

namespace Domain.Entities;

// Raíz de la jerarquía TPT (tabla Users). Es abstracta: toda cuenta es Admin, Patient o Doctor.
public abstract class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    // Cada subclase declara sus roles; el orden define la prioridad de portal (Admin, Doctor, Patient).
    public abstract IEnumerable<Role> Roles { get; }
}
