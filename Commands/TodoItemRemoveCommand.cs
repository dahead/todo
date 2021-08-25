using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Remove a todo item.")]
    public sealed class TodoItemRemoveCommand : Command<TodoItemRemoveCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            [Description("The name of the todo item to remove.")]
            public string Name { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            TodoItem item = null;

            // check if we have at least the basics...
            if (string.IsNullOrEmpty(settings.Name))
                item = tm.Items.ShowSelectItemDialog("What todo item do you want to remove?");
            else
                item = tm.Items.FindItem(settings.Name);

            if (item != null)
            {
                if (AnsiConsole.Confirm($"Do you really want to remove the todo item { item.Name }?", false))
                {
                    tm.Items.Remove(item);
                    tm.SaveData();
                    AnsiConsole.MarkupLine($"Todo item [bold]{ item.Name }[/] removed.");
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
