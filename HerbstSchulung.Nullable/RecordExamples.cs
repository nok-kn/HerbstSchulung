using System.Text.Json.Serialization;

namespace HerbstSchulung.Nullable
{
    // Diese Klasse zeigt verschiedene Beispiele für C# Records.
    public static class RecordExamples
    {
        // Ein Record hat automatisch erzeugte Eigenschaften
        public record PersonDto(string FirstName, string LastName);

        // Vererbung von Records: Employee erbt von Person
        public record EmployeeDto(string FirstName, string LastName, string Position) : PersonDto(FirstName, LastName);

        // Normale Eigenschaften oder Methoden gehen auch
        public record Person2Dto(string FirstName, string LastName)
        {
            public int Age { get; set; } 
            
            public string GetFullName() => $"{FirstName} {LastName}";
        }

        // Atribute auf Eigenschaften
        public record Car(string Name, [property: JsonIgnore] decimal Preis);

        public static void Run()
        {
            var person1 = new PersonDto("Hans", "Müller");
            Console.WriteLine($"person1: {person1}"); // ToString gibt die Eigenschaftswerte aus

            // Immutability - Eigenschaften sind nicht veränderbar
            // deswegen sind records ideal für DTOs, Multhreading, Caching etc.
            // person1.FirstName = "Peter"; 

            // Zwei records mit gleichen Werten sind gleich
            var person1Clone = new PersonDto("Hans", "Müller");
            Console.WriteLine($"person1 == person1Clone: {person1 == person1Clone}"); // True (wertbasierte Gleichheit)

            // Bei record ist aber ReferenceEquals normalerweise false
            Console.WriteLine($"ReferenceEquals(person1, person1Clone): {ReferenceEquals(person1, person1Clone)}");

            // Kopieren und Ändern mit 'with'-Ausdruck (erstellt eine neue Instanz)
            var person2 = person1 with { FirstName = "Petra" };
            Console.WriteLine($"person2 (kopiert mit with): {person2}");

            // Deconstruction von records
            // man kann Deconstruct Methoden auch selbst implementieren
            var (vorname, nachname) = person2;
            Console.WriteLine($"Deconstructed: {vorname} {nachname}");

            // Vererbung: Employee erbt Eigenschaften von Person
            var employee = new EmployeeDto("Anna", "Schmidt", "Entwicklerin");
            Console.WriteLine($"employee: {employee}");

        }
    }
}
