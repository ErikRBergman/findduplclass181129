using System;

namespace FindDuplicates.Console
{
    using System.Diagnostics;
    using System.Linq;

    using FindDuplicates.Loader;

    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();

            var loader = new FileLoader();
            var rawFiles = loader.LoadAllFiles(@"C:\projects\linux\drivers", filename => filename.EndsWith(".c", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".h", StringComparison.OrdinalIgnoreCase)).ToArray();
            Console.WriteLine($"{rawFiles.Length} files loaded into memory");

            stopwatch.Stop();
            

            Console.WriteLine($"Loading files took {stopwatch.ElapsedMilliseconds}ms");

            Console.ReadKey(false);
        }
    }
}
