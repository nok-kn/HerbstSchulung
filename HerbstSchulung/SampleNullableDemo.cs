// Beispielklassen zur Demonstration von Nullable Reference Types in C#
// Kommentare sind auf Deutsch verfasst

namespace HerbstSchulung
{
    // Die Klasse Person demonstriert die Verwendung von nicht-nullbaren und nullable Strings
    public class Person
    {
        // Name darf niemals null sein (nicht-nullbarer String)
        public string Name { get; set; }

        // Spitzname kann null sein (nullable String)
        public string? Spitzname { get; set; }

        public Person(string name, string? spitzname = null)
        {
            // Name muss �bergeben werden und darf nicht null sein
            Name = name;
            // Spitzname ist optional und kann null sein
            Spitzname = spitzname;
        }
    }

    // Die Klasse Address demonstriert ebenfalls Nullable Reference Types
    public class Address
    {
        // Stra�e darf niemals null sein
        public string Stra�e { get; set; }

        // Zusatz kann null sein (z.B. Wohnungsnummer, Stockwerk)
        public string? Zusatz { get; set; }

        public Address(string stra�e, string? zusatz = null)
        {
            Stra�e = stra�e;
            Zusatz = zusatz;
        }
    }
}
