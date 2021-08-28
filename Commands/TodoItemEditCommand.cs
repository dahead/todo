using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Edit a todo item.")]
    public sealed class TodoItemEditCommand : Command<TodoItemEditCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            [Description("The name of the todo item to add.")]
            public string Name { get; set; }

            [CommandOption("-d|--due <DUE>")]
            [Description("The due date of the todo item to add.")]
            public DateTime? DueAt { get; set; }

            [CommandOption("-r|--remind <REMIND>")]
            [Description("The reminder date of the todo item to add.")]
            public DateTime? RemindAt { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            TodoItem item = null;

            // check if we have at least the basics...
            if (string.IsNullOrWhiteSpace(settings.Name))
                item = tm.Items.ShowSelectItemDialog("What todo item do you want to edit?");
            else
                item = tm.Items.FindItem(settings.Name);

            // change item...
            if (item == null)
            {
                AnsiConsole.MarkupLine($"[red]Todo item { settings.Name } not found![/].");
                return 0;
            }

            if (AnsiConsole.Confirm("Do you want to change the [green]name[/]?", false))
                item.Name = AnsiConsole.Ask<string>("Whats the todo items [green]name[/]?");

            if (AnsiConsole.Confirm("Do you want to set/change the due date?", false))
                item.DueAt = AnsiConsole.Ask<DateTime>("When is the item [green]due[/]?");

            if (AnsiConsole.Confirm("Do you want to set/change the reminder?", false))
                item.RemindMeAt = AnsiConsole.Ask<DateTime>("When do you want to be [green]reminded[/]?");

            // ask for some other stuff

            // RepetationType: daily, weekly
            if (AnsiConsole.Confirm("Do you want to repeat the item?", false))
            {
                var input = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How do you want the item to [green]repeat[/]?")
                    .AddChoices(Repetation.GetRepetationTypeNames()));
                Repetation.RepeationType rt = input.ToRepetationType();

                int modifier = AnsiConsole.Ask<int>("In which [green]intervals[/] do you want the repetation?");
                int amount = AnsiConsole.Ask<int>("How [green]many[/] repetations of this type do you want to add?");
                item.RepetationList.AddIntervals(item.DueAt, rt, modifier, amount);
            }

            // show the details of the item to add
            AnsiConsole.Render(item.GetDetailsTree());

            // Keep the changes?
            if (AnsiConsole.Confirm("Do you want to keep the changes?", true))
                tm.SaveData();

            return 0;
        }
    }
}
