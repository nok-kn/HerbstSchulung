using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

// Beispiel für TPT (Table-per-Type) Vererbung in EF Core
// Bei TPT erhält jeder Typ seiner Vererbungshierarchie eine eigene Tabelle.
// Die Tabellen sind über den Primärschlüssel (Id) 1:1 verknüpft.


/// <summary>
/// Abstrakte Basisklasse für Fahrzeuge. Gemeinsame Eigenschaften aller Fahrzeuge.
/// </summary>
public abstract class Fahrzeug : EntityBase
{
    /// <summary>
    /// Hersteller des Fahrzeugs (z. B. "VW", "BMW").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Hersteller { get; set; }

    /// <summary>
    /// Modellbezeichnung (z. B. "Golf", "i3").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Modell { get; set; }

    /// <summary>
    /// Baujahr (optional).
    /// </summary>
    public int? Baujahr { get; set; }
}

/// <summary>
/// Abgeleiteter Typ "Auto". Erhält bei TPT eine eigene Tabelle "Autos",
/// die per 1:1 über die Id mit "Fahrzeuge" verknüpft ist.
/// </summary>
public class Auto : Fahrzeug
{
    /// <summary>
    /// Anzahl der Türen.
    /// </summary>
    [Range(0, 10)]
    public int AnzahlTueren { get; set; }

    /// <summary>
    /// Verfügt das Auto über einen Hybridantrieb?
    /// </summary>
    public bool HatHybridantrieb { get; set; }
}

/// <summary>
/// Abgeleiteter Typ "Lastkraftwagen". Bekommt bei TPT eine eigene Tabelle
/// "Lastkraftwagen" und verweist per Id auf den Basiseintrag in "Fahrzeuge".
/// </summary>
public class Lastkraftwagen : Fahrzeug
{
    /// <summary>
    /// Zuladung in Tonnen.
    /// </summary>
    [Range(0, 200)]
    public double ZuladungInTonnen { get; set; }

    /// <summary>
    /// Anzahl der Achsen.
    /// </summary>
    [Range(1, 10)]
    public int Achsen { get; set; }
}

