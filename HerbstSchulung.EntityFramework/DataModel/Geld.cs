using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Value Object f�r Geldbetr�ge mit W�hrung.
/// Wird in EF Core als Owned Entity (Complex Type) verwendet.
/// </summary>
public class Geld
{
    public static readonly Geld ZeroEuro = new Geld(0m, "EUR");

    /// <summary>
    /// Der Geldwert/Betrag
    /// </summary>
    [Range(0, 1_000_000_000)]
    public decimal Wert { get; set; }

    /// <summary>
    /// Die W�hrung (z.B. "EUR", "USD", "CHF")
    /// </summary>
    [Required]
    [MaxLength(3)]
    public required string Waehrung { get; set; }

    /// <summary>
    /// Parameterloser Konstruktor f�r EF Core
    /// </summary>
    public Geld()
    {
    }

    /// <summary>
    /// Konstruktor mit Parametern f�r bequeme Initialisierung
    /// </summary>
    [SetsRequiredMembers]
    public Geld(decimal wert, string waehrung)
    {
        Wert = wert;
        Waehrung = waehrung;
    }

    public override string ToString()
    {
        return $"{Wert:N2} {Waehrung}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Geld geld &&
               Wert == geld.Wert &&
               Waehrung == geld.Waehrung;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Wert, Waehrung);
    }
}
