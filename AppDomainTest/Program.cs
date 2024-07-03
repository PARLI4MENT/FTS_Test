using System;

namespace std
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arch_docs.log"));

            Console.ReadKey();
        }
    }
}