using System;

namespace HabitFlow.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime RegistrationDate { get; private set; }


    private User() { }

    public User(string name, string email, string passwordHash)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        RegistrationDate = DateTime.UtcNow;
    }




}

