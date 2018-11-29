namespace FindDuplicates.Parser
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class FileParser
    {
        private readonly CParser parser = new CParser();

        public IEnumerable<SourceFile> ParseFiles(IEnumerable<RawSourceFile> rawSourceFiles)
        {
            var sourceFiles = new ConcurrentBag<SourceFile>();

            foreach (var rawFile in rawSourceFiles)
            {
                try
                {
                    var sourceFile = this.parser.Parse(rawFile);
                    sourceFiles.Add(sourceFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(rawFile.FullPath + " failed:" + e);
                    throw;
                }
            }

            return sourceFiles;
        }
    }
}