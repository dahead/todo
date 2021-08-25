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
    [Description("Setup some default values for new todo item, the presentation, ...")]
    internal sealed class SetupDefaultsCommand : Command<SetupDefaultsCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("Application wide datetime format string.")]
            [CommandOption("-d|--dateformat")]
            public string DateTimeFormat { get; set; }

            // default columns in: item-list

            // default details in: item-show

        }

        CancellationTokenSource cts = new CancellationTokenSource();

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
      

            return 0;
        }

    }
}
