namespace HerbstSchulung.CSharpFeatures
{
    // Standardimplementierungen
    public interface IHasTimestamp
    {
        // Standardimplementierung einer Eigenschaft
        DateTime Now => DateTime.UtcNow;

        // Standardimplementierung, die die Eigenschaft nutzt
        string FormatTimestamp() => Now.ToString("O");

        DateTime CalculateTimeStamp() => throw new NotImplementedException("Kommt hoffentlich in nächstem Sprint!");
    }

    // Erbt die Funktionalität vollständig aus dem Interface
    public class Entity : IHasTimestamp
    {
        // Keine Implementierung notwendig; die Standardimplementierungen reichen aus.
    }

    public class Entity2 : IHasTimestamp
    {
        public string FormatTimestamp() => "Entity2: " + ((IHasTimestamp)this).Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // Konfliktauflösung bei mehreren Standardimplementierungen gleicher Signatur
    public interface IWriterA
    {
        void Write(string message) => Console.WriteLine($"A: {message}");
    }

    public interface IWriterB
    {
        void Write(string message) => Console.WriteLine($"B: {message}");
    }

    // Der Konflikt kann man schon auflösen, aber am besten ist es solche Mehrdeutigkeiten zu vermeiden
    public class DualWriter : IWriterA, IWriterB
    {
        public void WriteMessage(string message)
        {
             // Write("test");
            
            // Eigene Implementierung, die beide Aufrufe kombiniert
            ((IWriterA)this).Write(message);
            ((IWriterB)this).Write(message);
        }

        // Öffentliche Fassade, um gezielt eine der Implementierungen zu verwenden
        public void WriteAsA(string message) => ((IWriterA)this).Write(message);
        public void WriteAsB(string message) => ((IWriterB)this).Write(message);

    }



}
