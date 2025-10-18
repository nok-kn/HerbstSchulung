using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

// Werte ald Discriminator für die Person-Hierarchie
// nicht zwingend erforderlich, aber hilfreich für Typsicherheit
// einmal definiert, sollen die Werte nicht mehr geändert werden
public enum PersonArt
{
    Student,
    Teacher
}

public abstract class Person : EntityBase
{
    [MaxLength(100)]
    [Required]
    public required string Name { get; set; }

    [MaxLength(100)]
    public string? Nationality { get; set; }

    // Property, das auf die Discriminator-Spalte abbildet
    public abstract PersonArt Art { get;  } 
}

public class Student : Person
{
    public override PersonArt Art => PersonArt.Student;

    [MaxLength(50)]
    [Required]
    public required string School { get; set; }
}

public class Teacher : Person
{
    public override PersonArt Art => PersonArt.Teacher;

    [MaxLength(50)]
    [Required]
    public required string Subject { get; set; }
}

