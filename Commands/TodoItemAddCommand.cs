using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Add a todo item.")]
    public sealed class TodoItemAddCommand : Command<TodoItemAddCommand.Settings>
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
            TodoItem item = new TodoItem();

            // check if we have at least the basics...
            if (string.IsNullOrWhiteSpace(settings.Name))
                item.Name = AnsiConsole.Ask<string>("Whats the todo item's [green]name[/]?");
            else
                item.Name = settings.Name;

            if (settings.DueAt == null)
                if (AnsiConsole.Confirm("Do you want to set a due date?", false))
                    item.DueAt = AnsiConsole.Ask<DateTime>("When is the item [green]due[/]?");

            if (settings.RemindAt == null)
                if (AnsiConsole.Confirm("Do you want to set a reminder?", false))
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
                item.RepetationList.AddIntervals(item.DueAt.Value, rt, modifier, amount);
            }

            // show the details of the item to add
            AnsiConsole.Render(item.GetDetailsTree());

            // ask user if we want to add this item
            if (AnsiConsole.Confirm("Do you want to add this todo item?", true))
            {
                tm.Items.Add(item);
                tm.SaveData();
                AnsiConsole.MarkupLine($"Todo item [bold]{ item.Name }[/] added.");                
            }
            else
                AnsiConsole.MarkupLine($"[yellow]Adding Todo item canceled.[/].");

            return 0;
        }
    }
}
