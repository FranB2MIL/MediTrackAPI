using Domain.Entities.Enums;

namespace Domain.Interfaces
{
    public interface IAuthenticatable
    {
        int Id { get;  }
        string FirstName { get;  }
        string LastName { get;  }
        string Email { get;  }
        string Password { get; }
        Role Role { get; }
    }
}