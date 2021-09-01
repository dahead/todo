using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

public class TodoItem
{

	public enum NoteEditMode
	{
		Add,
		Edit,
		Remove
	}

	public string Name { get; set; }
	// public string Description { get; set; }
	public List<string> Notes { get; set; }
	public DateTime CreatedAt { get; set; }


	public DateTime DueAt { get; set; }
	public Repetation.RepetationList RepetationList { get; set; }

	public bool IsDone { get => this.IsOverDue(); }

	public DateTime RemindMeAt { get; set; }

	public List<string> Attachments { get; set; }
	public List<TodoTask> Tasks { get; set; }

	private DateTime NoDate = new DateTime(1, 1, 1);

	private const string DefaultDateFormatString = "MMM dd yyyy hh:mm:ss";

	public TodoItem()
	{
		this.CreatedAt = DateTime.Now;
		this.DueAt = NoDate;
		this.Notes = new List<string>();
		this.Attachments = new List<string>();
		this.Tasks = new List<TodoTask>();
		this.RepetationList = new Repetation.RepetationList();
	}

	public string GetDueText()
	{
		if (this.DueAt != NoDate)
		{
			string duetext = this.DueAt.ToString(DefaultDateFormatString);
			TimeSpan ts = this.DueAt - DateTime.UtcNow;
			if (ts.Ticks < 0)
				return $":warning: [red]{ duetext }[/] { ts.ToReadableString() }";
			else
				return $":waning_crescent_moon: [green]{ duetext }[/] { ts.ToReadableString() }";
		}
		else
		{
			// no due date... we can relax...
			// return ":sleepy_face: No due date set.";
			return ":sleepy_face:";
		}
	}

	public double GetRemainingTimeValue()
	{
		if (this.DueAt == NoDate)
			return 0;
		return 100 - GetPassedTimeValue();
	}

	public double GetPassedTimeValue()
	{
		// returns the remaining time in percent
		if (this.DueAt == NoDate)
			return 0;

		// remaining time
		long ts = this.DueAt.Ticks - DateTime.Now.Ticks;
		TimeSpan tsa = new TimeSpan(ts);

		// passed time
		long ta = this.DueAt.Ticks - this.CreatedAt.Ticks;
		TimeSpan taa = new TimeSpan(ta);

		var to = tsa.Ticks + taa.Ticks;
		// var tst = new TimeSpan(to);

		var xx1 = ta * 100 / to;
		// var xx2 = ts * 100 / to;

		return xx1;

		// TimeSpan rest = this.DueAt - DateTime.Now;
		// TimeSpan passed = this.DueAt - this.CreatedAt;
		// var total = (passed.Ticks + rest.Ticks) / rest.Ticks;
		// return total;
	}







	public string GetRemainingTimePercentString()
	{
		return $"{ this.GetPassedTimeValue() } | 100 %";
	}

	private bool IsOverDue()
	{
		if (this.DueAt == NoDate)
			return false;
		return DateTime.Now.Ticks > this.DueAt.Ticks;
	}

	public void NoteEdit()
	{
		// if we dont have any notes yet, just show the add dialog.
		if (this.Notes.Count == 0)
		{
			this.ShowNoteAddDialog();
			return;
		}

		// show existing notes
		this.DisplayNotes();

		// ask what to do
		var input = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("What exactly do you want to [green]do[/]?")
				.AddChoices("Add", "Edit", "Remove", "Cancel"));

		switch (input.ToUpper())
		{
			case "ADD":
				this.ShowNoteAddDialog();
				break;
			case "EDIT":
				this.ShowNoteEditDialog();
				break;
			case "REMOVE":
				this.ShowNoteRemoveDialog();
				break;
			case "CANCEL":
				break;
		}

		// display the new changes
		this.DisplayNotes();
	}

	private void DisplayNotes()
	{
		// // Create a table
		// var table = new Table()
		//     .Title($"Notes of todo item [bold green]{ this.Name }[/]")
		//     .Border(TableBorder.HeavyHead)
		//     .AddColumn(new TableColumn("ID"))
		//     .AddColumn(new TableColumn("Note"));

		// // add items
		// for (int i = 0; i < this.Notes.Count; i++)
		// {
		//     table.AddRow(i.ToString(), this.Notes[i]);            
		// }

		// // Render the table to the console
		// AnsiConsole.Render(table);


		// Create the tree
		var root = new Tree($"todo item [bold green]{ this.Name }[/]");

		// Add some nodes
		var foo = root.AddNode("[yellow]Notes[/]");

		var it = new Table()
			.RoundedBorder()
			.AddColumn("ID")
			.AddColumn("Content");

		var table = foo.AddNode(it);
		for (int i = 0; i < this.Notes.Count; i++)
			it.AddRow(i.ToString(), this.Notes[i]);

		AnsiConsole.Render(root);
	}

	private void ShowNoteRemoveDialog()
	{
		// Ask which note should be removed.
		var input = AnsiConsole.Prompt(
						new SelectionPrompt<string>()
							.Title("Which note do you want to [green]remove[/]?")
							.AddChoices(GetNotesAsChocies()));

		// parse the id
		int id = Convert.ToInt32(input.Substring(0, 1));

		// remove the note
		this.Notes.RemoveAt(id);
	}

	private void ShowNoteEditDialog()
	{
		// Ask which note should be edited.
		var input = AnsiConsole.Prompt(
						new SelectionPrompt<string>()
							.Title("Which note do you want to [green]edit[/]?")
							.AddChoices(GetNotesAsChocies()));

		// parse the id
		int id = Convert.ToInt32(input.Substring(0, 1));

		// change the note
		// // problem: previous text goes missing
		// // todo: display already existing text...

		// Replace note
		this.Notes[id] = AnsiConsole.Prompt(new TextPrompt<string>("Content:"));
	}

	private List<string> GetNotesAsChocies()
	{
		var displayitems = new List<string>();
		for (int i = 0; i < this.Notes.Count; i++)
			displayitems.Add(i.ToString() + " " + this.Notes[i]);
		return displayitems;
	}

	public void ShowNoteAddDialog()
	{
		bool AddMode = true;
		while (AddMode)
		{
			this.Notes.Add(AnsiConsole.Prompt(new TextPrompt<string>("Content:")));
			if (!AnsiConsole.Confirm("Continue adding?", true))
				AddMode = false;
		}
	}

	public Table GetDetailsTable(bool repetations = false, bool attachments = false, bool notes = false, bool calendar = false)
	{
		// Create a table
		var table = new Table()
			.Border(TableBorder.Rounded)
			// .BorderStyle(Style.Plain)
			.Title("Todo Item")
			.AddColumn(new TableColumn("Done"))
			.AddColumn(new TableColumn("Name"))
			.AddColumn(new TableColumn("Due"));

		table.AddRow(new Text(this.GetDueText()), new Text(this.Name), new Text(this.GetDueText()));

		if (notes)
			if (this.Notes != null && this.Notes.Count > 0)
			{
				var subtable = new Table()
					.Border(TableBorder.Rounded)
					.BorderColor(Color.Grey)
					.AddColumn("Notes:");
				foreach (var subitem in this.Notes)
					subtable.AddRow(new Text(subitem));
				table.AddRow(new Text(""), new Text(""), subtable);
			}

		if (repetations)
			if (this.RepetationList != null && this.RepetationList.Count > 0)
			{
				var subtable = new Table()
					.Border(TableBorder.Rounded)
					.BorderColor(Color.Grey)
					.AddColumn("Repetations:");
				foreach (var subitem in this.RepetationList)
					subtable.AddRow(new Text(subitem.RepeatAt.ToShortDateString()));
				table.AddRow(new Text(""), new Text(""), subtable);
			}

		if (attachments)
			if (this.Attachments != null && this.Attachments.Count > 0)
			{
				var subtable = new Table()
					.Border(TableBorder.Rounded)
					.BorderColor(Color.Grey)
					.AddColumn("Attachments:");
				foreach (string filename in this.Attachments)
					subtable.AddRow(new Text(filename));
				table.AddRow(new Text(""), new Text(""), subtable);
			}

		// display due at calendar item
		if (calendar)
		{
			// todo: move this to the third column
			// var duenode = table.AddRow($"[yellow]Due at:[/] { this.DueAt.ToString(DefaultDateFormatString) }");
			var duenode = table.AddRow(new Calendar(this.DueAt.Year, this.DueAt.Month)
				.Border(TableBorder.Rounded)
				.BorderStyle(new Style(Color.Green3_1))
				.AddCalendarEvent(this.DueAt.Year, this.DueAt.Month, this.DueAt.Day)
				.HideHeader()
				);

			// display progression    
			var progressstatuschart = GetRemainingTimeProgressChart();
			duenode.AddRow(new Text(""), new Text(""), progressstatuschart);
		}

		return table;
	}

	public Tree GetDetailsTree(bool repetations = false, bool attachments = false, bool notes = false, bool calendar = false)
	{
		// Create the tree
		var tree =
			// new Tree($"[yellow]Details of:[/] { this.Name }")
			new Tree("") // no name looks better in showlist mode
			.Style(Style.Parse("red"))
			.Guide(TreeGuide.Ascii);

		// display due at calendar item
		if (calendar)
		{
			// var duenode = tree.AddNode($"[yellow]Due at:[/] { this.DueAt.ToString(DefaultDateFormatString) }");
			var duenode = tree.AddNode(new Calendar(this.DueAt.Year, this.DueAt.Month)
				.Border(TableBorder.Rounded)
				.BorderStyle(new Style(Color.Green3_1))
				.AddCalendarEvent(this.DueAt.Year, this.DueAt.Month, this.DueAt.Day)
				.HideHeader()
				);

			// display progression    
			var progressstatuschart = GetRemainingTimeProgressChart();
			duenode.AddNode(progressstatuschart);
		}


		// Add notes
		if (notes)
			if (this.Notes.Count > 0)
			{
				var foo = tree.AddNode("[yellow]Notes:[/]");
				foreach (var item in this.Notes)
					foo.AddNode(item);
			}

		// Add repetations
		if (repetations)
			if (this.RepetationList.Count > 0)
			{
				var foo = tree.AddNode("[yellow]Repetations:[/]");
				foreach (var item in this.RepetationList)
					foo.AddNode(item.RepeatAt.ToLongDateString());
			}

		// Add attachments
		if (attachments)
			if (this.Attachments.Count > 0)
			{
				var foo = tree.AddNode("[yellow]Attachments:[/]");
				foreach (var item in this.Attachments)
					foo.AddNode(item);
			}

		return tree;
	}

	public BreakdownChart GetRemainingTimeProgressChart()
	{
		return new BreakdownChart()
			.ShowPercentage()
			// .FullSize()
			.AddItem("Time passed", this.GetPassedTimeValue(), Color.Red)
			.AddItem("Time remaining until due", this.GetRemainingTimeValue(), Color.Green);
	}

}