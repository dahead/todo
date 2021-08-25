using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace todo
{
    public static class StringExtensions
    {
        public static Repetation.RepeationType ToRepetationType(this String input)
        {
            foreach (Repetation.RepeationType  item in Enum.GetValues(typeof(Repetation.RepeationType)))
            {
                if (item.ToString() == input)
                    return item;
            }
            return Repetation.RepeationType.EveryYear;
        }

        // public static TodoItem.NoteEditMode ToNoteEditMode(this String input)
        // {
        //     foreach (TodoItem.NoteEditMode item in Enum.GetValues(typeof(Repetation.RepeationType)))
        //     {
        //         if (item.ToString() == input)
        //             return item;
        //     }
        //     return TodoItem.NoteEditMode.Add;
        // }

    }
}