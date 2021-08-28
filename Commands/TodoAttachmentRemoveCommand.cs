using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Remove a file from a todo item.")]
    public sealed class TodoAttachmentRemoveCommand : Command<TodoAttachmentRemoveCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
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
                item = tm.Items.ShowSelectItemDialog("What todo item do you want to remove attachments from?");
            else
                item = tm.Items.FindItem(settings.Name);

            if (item != null)
            {
                AnsiConsole.MarkupLine($"File {settings.Filename} removed from Todo item [bold]{ item.Name }[/].");
                item.Attachments.Remove(settings.Filename);
                tm.SaveData();
            }
            return 0;
        }
    }
}
