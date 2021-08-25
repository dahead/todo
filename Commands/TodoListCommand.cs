using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace todo.Commands
{
    [Description("Lists all the todo items.")]
    public sealed class TodoListCommand : Command<TodoListCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandOption("-t|--tree")]
            [Description("Display the details as a tree instead of a table.")]
            [DefaultValue(true)]
            public bool DetailsTree { get; set; }

            [CommandOption("-r|--repetations")]
            [Description("Display the repetations.")]
            [DefaultValue(false)]
            public bool Repetations { get; set; }

            [CommandOption("-a|--attachments")]
            [Description("Display the attachments.")]
            [DefaultValue(false)]
            public bool Attachments { get; set; }
            
            [CommandOption("-n|--notes")]
            [Description("Display the notes.")]
            [DefaultValue(false)]
            public bool Notes { get; set; }

            [CommandOption("-c|--calendar")]
            [Description("Display the calendar control.")]
            [DefaultValue(false)]
            public bool Calendar { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            tm.Items.ShowList(settings.DetailsTree, settings.Repetations, settings.Attachments, settings.Notes, settings.Calendar);
            return 0;
        }          
    }
}
