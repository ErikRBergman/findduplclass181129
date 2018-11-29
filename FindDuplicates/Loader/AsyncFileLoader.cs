namespace FindDuplicates.Loader
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class AsyncFileLoader
    {
        public async Task<IEnumerable<RawSourceFile>> LoadFilesAsync(IEnumerable<string> filenames, int concurrencyLevel)
        {
            var bag = new ConcurrentBag<RawSourceFile>();

            await this.LoadFilesAsync(
                filenames, 
                concurrencyLevel, 
                rsf =>
                {
                    bag.Add(rsf);
                    return Task.CompletedTask;
                });

            return bag;
        }

        public Task LoadFilesAsync(IEnumerable<string> filenames, int concurrencyLevel, Func<RawSourceFile, Task> loadedAction)
        {
            var queue = new ConcurrentQueue<string>(filenames);
            return Task.WhenAll(Enumerable.Range(0, concurrencyLevel).Select(_ => LoadFilesAsync(queue, loadedAction)));
        }

        private static async Task LoadFilesAsync(ConcurrentQueue<string> sourceQueue, Func<RawSourceFile, Task> loadedAction)
        {
            while (sourceQueue.TryDequeue(out var filename))
            {
                using (var sourceReader = File.OpenText(filename))
                {
                    await loadedAction(new RawSourceFile
                                   {
                                       Content = await sourceReader.ReadToEndAsync(),
                                       FullPath = filename
                                   });
                }
            }
        }
    }
}