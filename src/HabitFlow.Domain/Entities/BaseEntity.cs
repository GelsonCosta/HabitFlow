using System;

namespace HabitFlow.Domain.Entities;

public class BaseEntity
{

    public long Id { get; set; }
    public bool IsActive {get;set;} = true;

    public DateTime CreatedAt {get;set;}
}
