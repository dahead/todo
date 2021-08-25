using System;
using System.IO;
using Newtonsoft.Json;
using Spectre.Console;

public class TodoManager
{

    public const string default_items_filename = "items.json";

    public TodoList Items { get; set; } = new TodoList();

    public TodoManager()
    {     
        this.LoadData();
    }

    public void LoadData()
    {
        this.LoadItemsFromFile(default_items_filename, true);
    }

    private bool LoadItemsFromFile(string filename, bool createIfNotExistant = false)
    {
        if (!System.IO.File.Exists(filename))
        {
            if (createIfNotExistant)
                this.SaveData();
            return false;    
        }

        try
        {
            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                this.Items = (TodoList)serializer.Deserialize(file, typeof(TodoList));
            }
            return true;
        }
        catch (System.Exception)
        {
        }
        return false;
    }

    public void SaveData()
    {
        this.SaveItemsToFile(default_items_filename);
    }

    private bool SaveItemsToFile(string filename)
    {
        try
        {
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Newtonsoft.Json.Formatting.Indented };
                serializer.Serialize(file, this.Items);
            }
            AnsiConsole.MarkupLine($"[green]{ this.Items.Count }[/] saved to file { filename }.");
            return true;
        }
        catch (System.Exception)
        {            
        }
        return false;
    }
    
}