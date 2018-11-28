namespace FindDuplicates.Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class FileLoaderExtensions
    {
        public static IEnumerable<RawSourceFile> LoadFiles(this FileLoader loader, params string[] filenames)
        {
            return loader.LoadFiles(filenames);
        }

        public static IEnumerable<RawSourceFile> LoadAllFiles(this FileLoader loader, string directory)
        {
            return loader.LoadFiles(Directory.GetFiles(directory));
        }

        public static IEnumerable<RawSourceFile> LoadAllFiles(this FileLoader loader, string directory, Func<string, bool> predicate)
        {
            return loader.LoadFiles(Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(predicate));
        }
    }
}