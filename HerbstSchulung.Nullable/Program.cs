using System.Diagnostics.CodeAnalysis;
using HerbstSchulung.Nullable;

Console.WriteLine("Hello, World!");

// Beispiel 1: Zuweisung von null an ein nicht-nullbares Feld erzeugt eine Warnung
var p1 = new Person(null); // Warnung: Cannot convert null literal to non-nullable reference type. 

// Beispiel 2: Nachträgliche Zuweisung von null an nicht-nullbare Eigenschaft erzeugt eine Warnung
var p2 = new Person("Max");
p2.Name = null; // Warnung: Cannot convert null literal to non-nullable reference type.

// Beispiel 3: Nullable Eigenschaft kann ohne Warnung null zugewiesen werden
p2.Spitzname = null; // Keine Warnung, da Spitzname nullable ist

















// Beispiel 4: Wie intelligent ist der Compiler? 
var x = GetString(null);
p2.Name = x;

[return: NotNullIfNotNull(nameof(parameter))]
static string? GetString(string? parameter) => null;

// Beispiel 5: Wie kann man den Compiler austricksen?
p2.Name = null!; // Der Null-Forgiving Operator (!) unterdrückt die Warnung

// Beispiel 6: Defensive Programmierung ist weiterhin für "externe" Daten, Calls (Bibliotheken, die keine Nullable-Annotationen nutzen) wichtig
var eingabe = GetSomeString();
if (eingabe == null)
{
    throw new ArgumentNullException(nameof(eingabe), "Eingabe darf nicht null sein!");
}

#pragma warning disable CS8603 
static string GetSomeString() => null;
#pragma warning restore CS8603 

















// Beispiel 7: Verwendung von required und init Properties
var auto = new Fahrzeug { Marke = "VW", Modell = "Golf" };
var auto2 = new Fahrzeug { Marke = null!, Modell = null};


