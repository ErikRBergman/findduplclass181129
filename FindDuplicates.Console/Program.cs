namespace FindDuplicates.Console
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using FindDuplicates.Comparer;
    using FindDuplicates.Loader;
    using FindDuplicates.Parser;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var totalTimeStopwatch = Stopwatch.StartNew();

            //// Load files
            var loadFileStopWatch = Stopwatch.StartNew();

            Console.WriteLine("Loading file data...");

            var loader = new FileLoader();
            var rawFiles = loader.LoadAllFiles(
                    @"C:\projects\linux",
                    filename => filename.EndsWith(".c", StringComparison.OrdinalIgnoreCase) || filename.EndsWith(".h", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            loadFileStopWatch.Stop();

            Console.WriteLine($"{rawFiles.Length} files loaded into memory");
            Console.WriteLine($"Loading files took {loadFileStopWatch.ElapsedMilliseconds}ms");

            var filesToProcess = rawFiles.ToArray();

            //// Parse files
            Console.WriteLine($"Processing files to statements from {filesToProcess.Length} files...");

            var toStatementsStopwatch = Stopwatch.StartNew();
            var fileParser = new FileParser();
            // var sourceFiles = fileParser.ParseFiles(filesToProcess);
            var sourceFiles = await fileParser.ParseFilesAsync(filesToProcess);

            Console.WriteLine($"Processing files to statements done after {toStatementsStopwatch.ElapsedMilliseconds}ms ...");

            //// Find duplicates
            var findDuplicatesTimer = Stopwatch.StartNew();

            // Sort by path to ensure we always test the same files
            //var sourceFileArray = sourceFiles.OrderBy(sf => sf.FullPath).ToArray();
            var sourceFileArray = sourceFiles.OrderBy(sf => sf.FullPath).Take(100).ToArray();

            Console.WriteLine($"Finding duplicates in {sourceFileArray.Length} files...");

            var sourceComparer = new SourceComparer();
            var duplicateResult = sourceComparer.FindDuplicates(sourceFileArray);

            findDuplicatesTimer.Stop();

            Console.WriteLine($"{duplicateResult.uniqueDuplicates} unique duplicate statements found with a total of {duplicateResult.duplicateInstances} instances");

            Console.WriteLine($"Finding duplicates in {sourceFileArray.Length} files done after {findDuplicatesTimer.ElapsedMilliseconds}ms...");

            //// All done
            totalTimeStopwatch.Stop();

            Console.WriteLine($"Total processing time {totalTimeStopwatch.ElapsedMilliseconds}ms");

            Console.ReadKey(false);
        }
    }
}