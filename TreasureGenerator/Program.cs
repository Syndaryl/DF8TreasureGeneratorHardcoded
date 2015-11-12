using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syndaryl.TreasureGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var treasure = new Treasure("unifiedfile.xml");
            var TreasureResult = treasure.Generate();
            Console.WriteLine(TreasureResult);
        }
    }
}
