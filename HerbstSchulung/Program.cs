using System.Diagnostics.CodeAnalysis;
using HerbstSchulung;

Console.WriteLine("Hello, World!");

// Beispiel 1: Zuweisung von null an ein nicht-nullbares Feld erzeugt eine Warnung
Person p1 = new Person(null); // Warnung: Argument "name" kann nicht null sein

// Beispiel 2: Nachträgliche Zuweisung von null an nicht-nullbare Eigenschaft erzeugt eine Warnung
Person p2 = new Person("Max");
p2.Name = null; // Warnung: "Name" ist nicht-nullbar, null-Zuweisung nicht erlaubt
var x = GetString("null");
p2.Name = x;


// Beispiel 3: Nullable Eigenschaft kann ohne Warnung null zugewiesen werden
p2.Spitzname = null; // Keine Warnung, da Spitzname nullable ist
return;

[return: NotNullIfNotNull(nameof(parameter))]
static string? GetString(string? parameter) => null;

// Objektinitialisierung mit required und init Properties
var auto = new Fahrzeug { Marke = "VW", Modell = "Golf" };
var auto2 = new Fahrzeug { Marke = "aa" };
