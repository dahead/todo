using System;
using System.Diagnostics;
using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;
using todo.Commands;

namespace todo
{
       class Program
       {
              static void Main(string[] args)
              {
                     // create new app
                     var app = new CommandApp();

                     // configure
                     app.Configure(config =>{
                     #if DEBUG
                            config.PropagateExceptions();
                            config.ValidateExamples();
                     #endif

                     // add commands
                     config.AddCommand<TodoItemAddCommand>("item-add")
                            .WithAlias("add")
                            .WithAlias("ia")
                            .WithDescription("Adds a todo item.");

                     config.AddCommand<TodoItemEditCommand>("item-edit")
                            .WithAlias("edit")
                            .WithAlias("ie")
                            .WithDescription("Edit a todo item.");                    
                            
                     config.AddCommand<TodoItemRemoveCommand>("item-remove")
                            .WithAlias("remove")
                            .WithAlias("ir")
                            .WithDescription("Removes a todo item.");

                     config.AddCommand<TodoRepeatCommand>("item-repeat")
                            .WithAlias("repeat")
                            .WithAlias("rep")
                            .WithDescription("Repeats a todo item.");

                     config.AddCommand<TodoListCommand>("item-list")
                            .WithAlias("list")
                            .WithAlias("il")
                            .WithDescription("Displays all the todo items.");

                     config.AddCommand<TodoShowCommand>("item-show")
                            .WithAlias("show")
                            .WithAlias("is")
                            .WithDescription("Display the details of a todo items");

                     config.AddCommand<TodoAttachmentAddCommand>("item-addfile")
                            .WithAlias("addfile")
                            .WithAlias("af")
                            .WithDescription("Add a file to the todo item.");

                     config.AddCommand<TodoAttachmentRemoveCommand>("item-removefile")
                            .WithAlias("removefile")
                            .WithAlias("rf")
                            .WithDescription("Remove a file from the todo item.");

                     config.AddCommand<TodoItemNoteCommand>("item-note")
                            .WithAlias("note")
                            .WithAlias("in")
                            .WithDescription("Manage notes of a todo item.");

                     config.AddCommand<TodoNoteAddCommand>("item-note-add")
                            .WithAlias("na")
                            .WithDescription("Quickly add a note to a todo item.");
                     });


                     // display notes
                     if (args.Length == 0)
                     {
                            TodoManager tm = new TodoManager();
                            tm.Items.ShowList();
                     }
                     // advanced run with parameters
                     else
                     {
                            AnsiConsole.Render(new FigletText("todo").LeftAligned().Color(Color.SkyBlue1));                    
                            AnsiConsole.MarkupLine($"[grey]v.{ FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString() }[/]");
                            app.Run(args);
                     }
              }
       }
}
