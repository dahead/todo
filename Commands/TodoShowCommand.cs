using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace todo.Commands
{
    [Description("Displays the details of a todo item.")]
    public sealed class TodoShowCommand : Command<TodoShowCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "[NAME]")]     
            [Description("The name of the todo item to display.")]
            public string Name { get; set; }

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
            [Description("Display the calendar.")]
            [DefaultValue(false)]
            public bool Calendar { get; set; }        
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            TodoItem item = null;

            // check if we have at least the basics...
            if (string.IsNullOrWhiteSpace(settings.Name))
                item = tm.Items.ShowSelectItemDialog("What todo item do you want to display?");
            else
                item = tm.Items.FindItem(settings.Name);

            // show details
            if (item != null)
                AnsiConsole.Render(item.GetDetailsTree(settings.Repetations, settings.Attachments, settings.Notes, settings.Calendar)); 
            else
                AnsiConsole.MarkupLine($"[red]Todo item { settings.Name } not found![/].");

            return 0;
        }
          
    }
}
