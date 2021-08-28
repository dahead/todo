# todo

### About

todo is a CLI application for taking notes. It makes use of [Spectre.Console](https://github.com/spectreconsole/spectre.console)

### Features

todo items can be displayed as a table.

Each todo item has

* due date
* reminder
* repetations
* attachments
* notes

### Demo

gif will follow...

## Add a todo item
    todo item-add "Remember the milk"
    todo ia "Remember the milk"

Creates a new todo item with the name "Remember the milk".

## Add a todo item as a one line
    todo ia "Remember the milk" -d 01/01/2021

Creates a new todo item with the name "Remember the milk" which is due at 01/01/2021.    

### Other commands
    todo item-list
    todo item-show
    todo item-add    
    todo item-remove

### Screenshots

![Todo commands](/Images/commands.png "Commands")

![item-show command](/Images/command_item_show.png "Item show command")