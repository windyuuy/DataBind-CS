using Game.Diagnostics;

namespace RunDataBindDemo
{

    internal class Program
    {
        static void Main(string[] args)
        {
            var target = new TSampleTarget();
            target.PropertyChanged += (s, e) =>
              {
                  Debug.Log("wefwe");
              };
            target.DoubleFV = 234;
        }
    }
}
