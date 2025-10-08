// Beispielklasse für required und init Properties
namespace HerbstSchulung.Nullable;

public class Fahrzeug
{
    // Die Eigenschaft 'Marke' ist erforderlich und kann nur beim Initialisieren gesetzt werden (init == set nur im Initializer)
    public required string Marke { get; init; }

    // Die Eigenschaft 'Modell' ist optional, kann aber ebenfalls nur beim Initialisieren gesetzt werden
    public string? Modell { get; init; }

    public string? Farbe { get; set; }
    
}

