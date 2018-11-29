namespace FindDuplicates.Loader
{
    using System;
    using System.Collections.Generic;

    public class FileLoader
    {
        public IEnumerable<RawSourceFile> LoadFiles(IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                var content = System.IO.File.ReadAllText(filename);
                yield return new RawSourceFile
                {
                    FullPath = filename,
                    Content = content
                };
            }
        }
    }
}