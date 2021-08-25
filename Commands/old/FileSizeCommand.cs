using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace todo
{
    internal sealed class FileSizeCommand : Command<FileSizeCommand.Settings>
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
            .Start("Getting files sizes...", ctx =>
            {
                var searchPattern = settings.SearchPattern ?? "*.*";
                var searchPath = settings.Path ?? Directory.GetCurrentDirectory();
                var files = new DirectoryInfo(searchPath).GetFiles(searchPattern, searchOptions);      
                var totalFileSize = files.Sum(fileInfo => fileInfo.Length);
                AnsiConsole.MarkupLine($"Total file size for [green]{searchPattern}[/] files in [green]{searchPath}[/]: [blue]{totalFileSize:N0}[/] bytes");
            });

            return 0;
        }
    }
}
