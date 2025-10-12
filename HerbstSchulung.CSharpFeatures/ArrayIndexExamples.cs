namespace HerbstSchulung.CSharpFeatures;

public class ArrayIndexExamples
{
    public void ArrayZugriffe()
    {
        //                0   1   2   3   4
        int[] zahlen = { 10, 20, 30, 40, 50 };

        // caret 
        int letzt = zahlen[^1];    // 50 (letztes Element)
        int vorLetzt = zahlen[^2]; // 40 (vorletztes Element)
        int erst = zahlen[^5];     // 10 (erstes Element)

        // ranges
        int[] mitte = zahlen[1..4];    // { 20, 30, 40 }, matematisch <1..4)
        int[] bisEnde = zahlen[2..];   // { 30, 40, 50 }
        int[] vonAnfang = zahlen[..3]; // { 10, 20, 30 }
        int[] alles = zahlen[..];      // { 10, 20, 30, 40, 50 }

        int[] leer = [];

        // praktisch für Initalisierung von leeren Listen
        List<int> list = [];

    }
}
