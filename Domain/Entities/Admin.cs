using Domain.Entities.Enums;

namespace Domain.Entities;

public class Admin : User
{
    public override IEnumerable<Role> Roles => [Role.Admin];
}
