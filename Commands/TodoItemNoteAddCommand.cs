using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Quickly add a note to a todo item.")]
    public sealed class TodoNoteAddCommand : Command<TodoNoteAddCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            [Description("The name of the todo item.")]
            public string Name { get; set; }

            [CommandOption("-t|--text <TEXT>")]
            [Description("The text of the note to add.")]
            public string Note { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            TodoItem item = null;

            // check if we have at least the basics...
            if (string.IsNullOrEmpty(settings.Name))
                item = tm.Items.ShowSelectItemDialog("In which todo item do you want to add notes?");
            else
                item = tm.Items.FindItem(settings.Name);

            if (item != null)
            {
                item.ShowNoteAddDialog();
                tm.SaveData();                    
            }
            else
            {
                AnsiConsole.MarkupLine($"Todo item {settings.Name} not found!");
            }
            return 0;
        }
    }
}
