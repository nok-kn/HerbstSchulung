using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerbstSchulung.CSharpFeatures;

public class EntityBeispiel
{
    [Required]  // Required in DB/Validation
    public string KeyIdAkademischerAbschluss { get; set; } // Warnung hier

    public string Name { get; set; }
}

public class EntityBeispiel1
{
    [Required]
    public string KeyIdAkademischerAbschluss { get; set; } = string.Empty; // Schnell und einfach aber möglicherweise nicht sinnvoll

    public string Name { get; set; } = string.Empty;
}

public class EntityBeispiel2
{
    // EF Core materialisiert Entitäten mit Reflection
    // und erzwingt Keyword required zur Laufzeit nicht 

    [Required]
    public required string KeyIdAkademischerAbschluss { get; set; } 

    public required string Name { get; set; }
}
