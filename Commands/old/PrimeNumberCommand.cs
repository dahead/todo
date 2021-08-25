using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todo
{
    internal sealed class PrimeNumberCommand : Command<PrimeNumberCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("Number range.")]
            [CommandArgument(0, "[LIMIT]")]            
            public int Limit { get; set; }
        }

         public override int Execute(CommandContext context, Settings settings)
        {
            AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots4)
            .Start("Calculating prime numbers...", ctx =>
            {
                var task = executeParallelAsync(settings);
                task.Wait();
            });            
            return 0;
        }

        private async Task executeParallelAsync(Settings settings)
        {
            Progress<int> progress = new Progress<int>();
            progress.ProgressChanged += ReportProgress;
            var limit = settings.Limit;
            var numbers = Enumerable.Range(0, limit).ToList();
            var results = await GetPrimeNumbers(numbers, progress);
            PrintResults(results);
        }
        
        public async Task<ConcurrentBag<int>> GetPrimeNumbers(IList<int> numbers, IProgress<int> progress)
        {
            var report = new ConcurrentBag<int>();
            await Task.Run(() =>
            {
                Parallel.ForEach<int>(numbers, (number) =>
                {
                    // Ping host, get result, add result to output
                    if (IsPrime(number))
                        report.Add(number);
                    // report progress
                    progress.Report(number);
                });

            });
            return report;
        }


        private void ReportProgress(object sender, int e)
        {
            // AnsiConsole.Write(e + ", ");
            // AnsiConsole.WriteLine(e);
        }

        private void PrintResults(ConcurrentBag<int> results)
        {          
            foreach (var item in results)
            {
                // AnsiConsole.Write(item);
                AnsiConsole.WriteLine(item);
            }
            AnsiConsole.MarkupLine($"Calculated a total of { results.Count } prime numbers.");
        }

        /// <summary>
        /// IsPrime returns true if number is Prime, else false.(https://en.wikipedia.org/wiki/Prime_number)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }

            for (var divisor = 2; divisor <= Math.Sqrt(number); divisor++)
            {
                if (number % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }
        
    }
}
