using System;

namespace FindDuplicates.Console
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using FindDuplicates.Comparer;
    using FindDuplicates.Loader;
    using FindDuplicates.Parser;

    using Console = System.Console;

    class Program
    {
        static void Main(string[] args)
        {
            var totalTimeStopwatch = Stopwatch.StartNew();

            //// Load files
            /// 
            var loadFileStopWatch = Stopwatch.StartNew();

            Console.WriteLine($"Loading file data...");

            //var loader = new FileLoader();
            //var rawFiles = loader.LoadAllFiles(@"C:\projects\linux\drivers", filename => filename.EndsWith(".c", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".h", StringComparison.OrdinalIgnoreCase)).ToArray();

            var loader = new AsyncFileLoader();
            //var rawFiles = loader.LoadAllFilesAsync(@"C:\projects\linux\drivers", filename => filename.EndsWith(".c", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".h", StringComparison.OrdinalIgnoreCase), 10).GetAwaiter().GetResult().ToArray();
            var rawFiles = loader.LoadAllFilesAsync(@"C:\projects\linux\drivers", filename => filename.EndsWith("acard-ahci.c", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".h", StringComparison.OrdinalIgnoreCase), 10).GetAwaiter().GetResult().ToArray();

            //var rawFiles = loader.LoadFilesAsync(1, @"C:\projects\linux\drivers\video\fbdev\omap2\omapfb\omapfb-main.c").GetAwaiter().GetResult().ToArray();

            //rawFiles = rawFiles.Where(rf => rf.FullPath.EndsWith("acard-ahci.c", StringComparison.OrdinalIgnoreCase)).ToArray();

            loadFileStopWatch.Stop();

            Console.WriteLine($"{rawFiles.Length} files loaded into memory");
            Console.WriteLine($"Loading files took {loadFileStopWatch.ElapsedMilliseconds}ms");

            //// Parse files
            var parser = new CParser();

            //rawFiles = rawFiles.Where(rf => rf.FullPath.EndsWith("acard-ahci.c", StringComparison.OrdinalIgnoreCase)).ToArray();
            //var filesToProcess = rawFiles.Take(5000).ToArray();
            var filesToProcess = rawFiles.ToArray();

            Console.WriteLine($"Processing files to statements from {filesToProcess.Length} files...");

            var toStatementsStopwatch = Stopwatch.StartNew();

            var sourceFiles = new ConcurrentBag<SourceFile>();

            foreach (var rawFile in filesToProcess)
            {
                try
                {
                    var sourceFile = parser.Parse(rawFile);
                   sourceFiles.Add(sourceFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(rawFile.FullPath + " failed:" + e);
                    throw;
                }
            }

            Console.WriteLine($"Processing files to statements done after {toStatementsStopwatch.ElapsedMilliseconds}ms ...");

            //// Find duplicates


            var findDuplicatesTimer = Stopwatch.StartNew();

            var sourceFileArray = sourceFiles.Take(50).ToArray();

            Console.WriteLine($"Finding duplicates in {sourceFileArray.Length} files...");

            var sourceComparer = new SourceComparer();
            var duplicateResult = sourceComparer.FindDuplicates(sourceFileArray);

            findDuplicatesTimer.Stop();

            Console.WriteLine($"{duplicateResult.uniqueDuplicates} unique duplicate statements found with a total of {duplicateResult.duplicateInstances} instance");

            Console.WriteLine($"Finding duplicates in {sourceFileArray.Length} files done after {findDuplicatesTimer.ElapsedMilliseconds}ms...");

            //// All done

            totalTimeStopwatch.Stop();

            Console.WriteLine($"Total processing time {totalTimeStopwatch.ElapsedMilliseconds}ms");

            Console.ReadKey(false);
        }
    }
}
