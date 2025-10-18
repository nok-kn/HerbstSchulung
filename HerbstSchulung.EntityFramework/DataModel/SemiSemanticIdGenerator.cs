using System.Security.Cryptography;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Generator für "semi-semantische" IDs. Wärschentlichkeit einer Kollision (randomLength = 8) ≈  9.09e−13
/// </summary>
public static class SemiSemanticIdGenerator
{
    // Feste Präfixe für bekannte Entitäten.
    private static readonly IReadOnlyDictionary<Type, string> PrefixMap = new Dictionary<Type, string>
    {
        { typeof(Auto),            "AUT" },
        { typeof(Lastkraftwagen),  "LKW" },
        { typeof(Student),         "STD" },
        { typeof(Teacher),         "TCR" },
        { typeof(Land),            "LND" },
        { typeof(Angebot),         "ANG" },
        { typeof(Rechnung),        "RNG" },

    };

    public static string GenerateFor(object entity, int randomLength = 8) => GenerateFor(entity.GetType(), randomLength);
    
    public static string GenerateFor(Type entityType, int randomLength = 8)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentOutOfRangeException.ThrowIfLessThan(randomLength, 6);
        
        var prefix = ResolvePrefix(entityType);
        var rnd = CreateRandomString(randomLength);
        return $"{prefix}-{rnd}";
    }

    private static string ResolvePrefix(Type entityType) => 
        PrefixMap.TryGetValue(entityType, out var mapped) ? mapped : throw new InvalidOperationException($"No prefix defined for {entityType.Name}");


    // Erzeugt eine gut lesbare Zufallssequenz ohne leicht verwechselbare Zeichen (I, O, 1, 0).
    private static string CreateRandomString(int length)
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; 
        Span<byte> buffer = stackalloc byte[32];
        var chars = new char[length];

        for (int i = 0; i < length; i++)
        {
            // Fülle bei Bedarf nach (kleine, effiziente Blöcke)
            if (i % buffer.Length == 0)
            {
                RandomNumberGenerator.Fill(buffer);
            }
            var b = buffer[i % buffer.Length];
            chars[i] = alphabet[b % alphabet.Length];
        }

        return new string(chars);
    }
}
