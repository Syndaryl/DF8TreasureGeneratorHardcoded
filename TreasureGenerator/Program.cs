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
            var loop = true;
            while (loop)
            { 
                var TreasureResult = treasure.Generate();
                TreasureResult.Traverse(WriteItem);
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape || (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control) )
                    loop = false;
            }
        }

        private static void WriteItem(DF8Result obj)
        {
            Console.WriteLine(obj.Item);
        }
    }
}
