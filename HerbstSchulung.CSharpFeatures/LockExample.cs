
namespace HerbstSchulung.CSharpFeatures
{
    public class LockExample
    {
        // funktioniert NICHT mit.NET 8
        //  System.Threading.Lock myLock = new();  //  existiert nicht

        private readonly object  _myLock = new object();

        private void DoSomething()
        {
            lock (_myLock) 
            {
                // ...
            }
        }
    }
}
