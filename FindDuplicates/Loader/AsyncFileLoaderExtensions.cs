namespace FindDuplicates.Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public static class AsyncFileLoaderExtensions
    {
        public static Task<IEnumerable<RawSourceFile>> LoadAllFilesAsync(this AsyncFileLoader loader, string directory, Func<string, bool> predicate, int concurrencyLevel)
        {
            return loader.LoadFilesAsync(Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(predicate), concurrencyLevel);
        }

        public static Task<IEnumerable<RawSourceFile>> LoadFilesAsync(this AsyncFileLoader loader, int concurrencyLevel, params string[] filenames)
        {
            return loader.LoadFilesAsync(filenames, concurrencyLevel);
        }
    }
}