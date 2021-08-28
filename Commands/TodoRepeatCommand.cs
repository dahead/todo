using System;
using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo.Commands
{
    [Description("Repeat a todo item.")]
    public sealed class TodoRepeatCommand : Command<TodoRepeatCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandOption("-n|--name <NAME>")]
            [Description("The name of the todo item to repeat.")]
            public string Name { get; set; }

            [CommandOption("-t|--type <TYPE>")]
            [Description("The type of the repetation.")]
            [DefaultValue(Repetation.RepeationType.EveryYear)]
            public Repetation.RepeationType Type { get; set; }

            [CommandOption("-m|--modifier <MODIFIER>")]
            [Description("The modifier. If the type is EveryWeek and the modifier is set to 2, then the repetation occurs every 2nd week.")]
            [DefaultValue(1)]
            public int Modifier { get; set; }

            [CommandOption("-a|--amount <AMOUNT>")]
            [Description("The amount of the repetations.")]
            [DefaultValue(1)]
            public int Amount { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            TodoManager tm = new TodoManager();
            TodoItem item = null;

            // check if we have at least the basics...
            if (string.IsNullOrWhiteSpace(settings.Name))
                item = tm.Items.ShowSelectItemDialog("What todo item do you want to add repetations for?");
            else
                item = tm.Items.FindItem(settings.Name);

            if (item != null)
            {
                AnsiConsole.MarkupLine($"Adding { settings.Amount } repetations { settings.Type } with { settings.Modifier } modidiers to Todo item [bold]{ item.Name }[/].");  
                item.RepetationList.AddIntervals(DateTime.Now, settings.Type, settings.Modifier, settings.Amount);     
                tm.SaveData();
            }
            else
                AnsiConsole.MarkupLine($"[red]Todo item { settings.Name } not found![/].");

            return 0;
        }
    }
}
