using HerbstSchulung.EntityFramework.DataModel;

namespace HerbstSchulung.EntityFramework.Seeding;

/// <summary>
/// Laufzeit-Seeding ohne HasData/Migrationen für Demo/Testdaten.
/// </summary>
public static class TestData
{
    public static void EnsureSeeded(AppDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        var hasPersons = db.Students.Any() || db.Teachers.Any();
        if (!hasPersons)
        {
            db.AddRange(GeneratePersons(20));
        }

        var hasVehicles = db.Set<Fahrzeug>().Any();
        if (!hasVehicles)
        {
            db.AddRange(GenerateVehicles(10));
        }

        if (!hasPersons || !hasVehicles)
        {
            db.SaveChanges();
        }
    }

    private static IEnumerable<Person> GeneratePersons(int count)
    {
        var result = new List<Person>(capacity: count);
        string[] countries = ["DE", "AT", "CH", "IT", "FR", "ES"];        
        string[] schools = ["Abcedef", "Oxford", "Test", "TU München", "LMU München"]; 
        string[] subjects = ["Mathematik", "Informatik", "Physik", "Geschichte", "Englisch"];        

        for (int i = 1; i <= count; i++)
        {
            var nationality = countries[(i - 1) % countries.Length];
            if (i % 2 == 0)
            {
                result.Add(new Student
                {
                    Name = $"Seed Student {i}",
                    Nationality = nationality,
                    School = schools[(i - 1) % schools.Length]
                });
            }
            else
            {
                result.Add(new Teacher
                {
                    Name = $"Seed Teacher {i}",
                    Nationality = nationality,
                    Subject = subjects[(i - 1) % subjects.Length]
                });
            }
        }

        return result;
    }

    private static IEnumerable<Fahrzeug> GenerateVehicles(int count)
    {
        var result = new List<Fahrzeug>(capacity: count);
        string[] hersteller = ["VW", "BMW", "Audi", "Mercedes", "MAN", "Scania"];
        string[] autoModelle = ["Golf", "Passat", "A4", "C-Klasse", "i3", "ID.3"]; 
        string[] lkwModelle = ["TGX", "ABC", "XXXX", "1234"]; 

        for (int i = 1; i <= count; i++)
        {
            bool makeAuto = i % 2 == 1; 
            if (makeAuto)
            {
                result.Add(new Auto
                {
                    Hersteller = hersteller[i % hersteller.Length],
                    Modell = autoModelle[i % autoModelle.Length],
                    Baujahr = 2015 + (i % 10),
                    AnzahlTueren = 3 + (i % 3),
                    HatHybridantrieb = (i % 4 == 0)
                });
            }
            else
            {
                result.Add(new Lastkraftwagen
                {
                    Hersteller = hersteller[(i + 2) % hersteller.Length],
                    Modell = lkwModelle[i % lkwModelle.Length],
                    Baujahr = 2010 + (i % 12),
                    ZuladungInTonnen = 10 + (i % 15),
                    Achsen = 2 + (i % 3)
                });
            }
        }

        return result;
    }
}

