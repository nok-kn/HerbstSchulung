namespace HerbstSchulung.CSharpFeatures;


// Die Klasse Person demonstriert die Verwendung von nicht-nullbaren und nullable Strings
public class Person
{
    // Name darf niemals null sein (nicht-nullbarer String)
    public string Name { get; set; }

    // Spitzname kann null sein (nullable String)
    public string? Spitzname { get; set; }

    public Person(string name, string? spitzname = null)
    {
        Name = name;
        Spitzname = spitzname;
    }
}

