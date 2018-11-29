namespace FindDuplicates.Loader
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AsyncFileLoader
    {
        public async Task<IEnumerable<RawSourceFile>> LoadFilesAsync(IEnumerable<string> filenames, int concurrencyLevel)
        {
            var queue = new ConcurrentQueue<string>(filenames);
            var bag = new ConcurrentBag<RawSourceFile>();
            await Task.WhenAll(Enumerable.Range(0, concurrencyLevel).Select(_ => LoadFilesAsync(queue, bag)));
            return bag;
        }

        private static async Task LoadFilesAsync(ConcurrentQueue<string> sourceQueue, ConcurrentBag<RawSourceFile> result)
        {
            while (sourceQueue.TryDequeue(out var filename))
            {
                using (var sourceReader = File.OpenText(filename))
                {
                    result.Add(new RawSourceFile
                                   {
                                       Content = await sourceReader.ReadToEndAsync(),
                                       FullPath = filename
                                   });
                }
            }
        }
    }
}