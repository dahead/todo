using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace todo
{
    internal sealed class FileChecksumCommand : Command<FileChecksumCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("Path to search. Defaults to current directory.")]
            [CommandArgument(0, "[PATH]")]
            public string Path { get; set; }

            [Description("Search pattern. Defaults to *.* (every file).")]
            [CommandOption("-p|--pattern")]
            public string SearchPattern { get; set; }

            [Description("Include hidden files. Defaults to false.")]
            [CommandOption("--hidden")]
            [DefaultValue(false)]
            public bool IncludeHidden { get; set; }

            [Description("Include all files in all sub directories. Defaults to false.")]
            [CommandOption("--recursive")]
            [DefaultValue(false)]
            public bool Recursive {  get; set; }
        }

        CancellationTokenSource cts = new CancellationTokenSource();

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var searchOptions = new EnumerationOptions
            {
                AttributesToSkip = settings.IncludeHidden
                    ? FileAttributes.Hidden | FileAttributes.System
                    : FileAttributes.System,
                RecurseSubdirectories = settings.Recursive
            };

            AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .Start($"Calculting SHA256 hashes...", ctx =>
            {
                var searchPattern = settings.SearchPattern ?? "*.*";
                var searchPath = settings.Path ?? Directory.GetCurrentDirectory();
                var files = new DirectoryInfo(searchPath).GetFiles(searchPattern, searchOptions);      
                var task = executeParallelAsync(files);
                task.Wait();
            });          

            return 0;
        }

        private async Task executeParallelAsync(FileInfo[] files)
        {
            Progress<CheckSumDataModel> progress = new Progress<CheckSumDataModel>();
            progress.ProgressChanged += ReportProgress;
            var results = await GetHashParallelAsync(files, progress, cts.Token);
            PrintResults(results);
        }
        
        public async Task<List<CheckSumDataModel>> GetHashParallelAsync(FileInfo[] files, IProgress<CheckSumDataModel> progress, CancellationToken cancellationToken)
        {
            var report = new List<CheckSumDataModel>();
            await Task.Run(() =>
            {
                Parallel.ForEach<FileInfo>(files, (file) =>
                {
                    // Ping host, get result, add result to output
                    string checksum = CalculateSHA256(file.FullName);
                    CheckSumDataModel result = new CheckSumDataModel() { File = file, Checksum = checksum };
                    report.Add(result);

                    cancellationToken.ThrowIfCancellationRequested();

                    // report progress
                    progress.Report(result);
                });
            });

            return report;
        }
        
        private void ReportProgress(object sender, CheckSumDataModel e)
        {
            // try
            // {
            //     AnsiConsole.MarkupLine($"{ e.File.Name } - [green]{ e.Checksum }[/]");
            //     // AnsiConsole.Write(".");

            // }
            // catch (System.Exception)
            // {
            // }
        }

        private void PrintResults(List<CheckSumDataModel> results)
        {
            AnsiConsole.MarkupLine($"{ Environment.NewLine }Calculated a total of [green]{results.Count}[/] hashes.");

            // Create a table
            var table = new Table();
            table.Border(TableBorder.Ascii);

            // Add some columns
            table.AddColumn("Directory");
            table.AddColumn("Filename");
            table.AddColumn(new TableColumn("Hash").RightAligned());

            var groupedresults = results.GroupBy(f => f.File.DirectoryName, f => f);          
            foreach (var group in groupedresults)
            {
                foreach (var item in group)
                {
                    try
                    {
                        table.AddRow(":check_mark: " + item.File.DirectoryName, item.File.Name, $"[green]{item.Checksum}[/]");               
                    }
                    catch (System.Exception)
                    {
                        // Unhandled exception. System.InvalidOperationException: Could not find color or style 'MV'.
                        // some checksums cause parsing errors in Spectre.Console
                    }
                }
            }

            // foreach (var item in results)
            // {
            //     try
            //     {
            //         table.AddRow(item.File.DirectoryName, item.File.Name, $"[green]{item.Checksum}[/]");               
            //     }
            //     catch (System.Exception)
            //     {
            //         // Unhandled exception. System.InvalidOperationException: Could not find color or style 'MV'.
            //         // some checksums cause parsing errors in Spectre.Console
            //     }
            // }

            // Render the table to the console
            AnsiConsole.Render(table);
        }

        private string CalculateSHA256(string filename)
        {
            try
            {
                using (var sha = SHA256.Create())
                {
                    using (var stream = new BufferedStream(File.OpenRead(filename), 1200000))
                    {
                        var hash = sha.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }                    
            }
            catch (System.Exception)
            {
                return string.Empty;
            }                   
        }
    }

    public class CheckSumDataModel
    {
        public FileInfo File {get; set; }
        public string Checksum { get; set; }
    }
}
