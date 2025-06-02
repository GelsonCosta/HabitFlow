using System;

namespace HabitFlow.Infrastructure.Settings;

public class JwtSettings
{
    public string Secret { get; set; }
    public int ExpiryDays { get; set; }
}
