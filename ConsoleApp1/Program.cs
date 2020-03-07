using Application;
using RoR2;
using Console = System.Console;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var result = AugmentResolver.ResolveAugment(ItemIndex.Mushroom);
        }
    }
}