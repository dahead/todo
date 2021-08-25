using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Manage notes in a todo item.")]
    public sealed class TodoItemNoteCommand : Command<TodoItemNoteCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<NAME>")]
            [Description("The name of the todo item to manage notes for.")]
            public string Name { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            TodoItem item = null;

            // check if we have at least the basics...
            if (string.IsNullOrEmpty(settings.Name))
                item = tm.Items.ShowSelectItemDialog("What todo item do you want to manage notes for?");
            else
                item = tm.Items.FindItem(settings.Name);

            if (item != null)
            {
                item.NoteEdit();
                // ask user if we want to keep the changes
                if (AnsiConsole.Confirm("Do you want to keep the changes?", true))
                    tm.SaveData();
                else
                    AnsiConsole.MarkupLine($"[red]Editing notes canceled.[/].");
            }
            else
                AnsiConsole.MarkupLine($"[red]Todo item { settings.Name } not found![/].");
                
            return 0;
        }
    }
}
