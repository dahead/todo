using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Add a file to a todo item.")]
    public sealed class TodoAttachmentAddCommand : Command<TodoAttachmentAddCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<NAME>")]
            [Description("The name of the todo item to add.")]
            public string Name { get; set; }

            [CommandOption("-f|--filename <FILENAME>")]
            [Description("The filename of the file to add.")]
            public string Filename { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            TodoItem item = null;

            // check if we have at least the basics...
            if (string.IsNullOrWhiteSpace(settings.Name))
                item = tm.Items.ShowSelectItemDialog("What todo item do you want to add attachments to?");
            else
                item = tm.Items.FindItem(settings.Name);

            if (item != null)
            {
                if (System.IO.File.Exists(settings.Filename))
                {
                    AnsiConsole.MarkupLine($"File {settings.Filename} add to Todo item [bold]{ item.Name }[/].");
                    item.Attachments.Add(settings.Filename);
                    tm.SaveData();                    
                }
                else
                {
                    AnsiConsole.MarkupLine($"File {settings.Filename} not found!");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"Todo item {settings.Name} not found!");
            }
            return 0;
        }
    }
}
