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
    public PersonArt Art { get; protected set; } 
}

public class Student : Person
{
    public Student()
    {
        Art = PersonArt.Student; // Setze den Discriminator-Wert
    }
    
    [MaxLength(50)]
    [Required]
    public required string School { get; set; }
}

public class Teacher : Person
{
    public Teacher()
    {
        Art = PersonArt.Teacher; // Setze den Discriminator-Wert
    }

    [MaxLength(50)]
    [Required]
    public required string Subject { get; set; }
}

