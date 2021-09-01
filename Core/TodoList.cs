using System;
using System.Collections.Generic;
using System.Diagnostics;
using Spectre.Console;

public class TodoList : List<TodoItem>
{

	public new void Add(TodoItem item)
	{
		Debug.Print($"Adding TodoItem: { item.Name  } due at { item.DueAt.ToString() } ");
		base.Add(item);
	}

	internal TodoItem FindItem(string name)
	{
		foreach (TodoItem item in this)
		{
			// if (item.Name == name)
			if (item.Name.ToUpper() == name.ToUpper())
				return item;
		}
		return null;
	}

	internal void ShowList(bool detailstree = true, bool repetations = false, bool attachments = false, bool notes = false, bool calendar = false)
	{
		// Create a table
		var table = new Table()
			.Border(TableBorder.Rounded)
			.ShowHeaders()
			// .BorderStyle(Style.Plain)
			.Title("Todo Items")
			.AddColumn(new TableColumn("Done"))
			.AddColumn(new TableColumn("Name"))
			.AddColumn(new TableColumn("Due"));

		// add items
		foreach (TodoItem item in this)
		{
			// Add todo item to the main table
			table.AddRow(item.IsDone ? ":check_mark:" : ":white_medium_square:", $"[blue]{ item.Name }[/]", item.GetDueText());

			if (detailstree)
				table.AddRow(new Text(""), new Text(""), item.GetDetailsTree(repetations, attachments, notes, calendar));
			else
				table.AddRow(new Text(""), new Text(""), item.GetDetailsTable(repetations, attachments, notes, calendar));
		}

		// Render the table to the console
		AnsiConsole.Render(table);
	}

	internal TodoItem ShowSelectItemDialog(string title)
	{
		// Ask which note should be edited.
		var input = AnsiConsole.Prompt(
						new SelectionPrompt<string>()
							.Title(title)
							.AddChoices(GetItemsAsChocies()));

		var id = Convert.ToInt32(input.Substring(0, 1));
		return this[id];
	}

	private List<string> GetItemsAsChocies()
	{
		List<string> result = new List<string>();
		for (int i = 0; i < this.Count; i++)
			result.Add(i.ToString() + " " + this[i].Name);
		return result;
	}
}