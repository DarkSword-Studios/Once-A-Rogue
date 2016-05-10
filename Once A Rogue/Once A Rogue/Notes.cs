using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Once_A_Rogue
{
    class Notes
    {
        public static List<Note> gatheredNotes = new List<Note>();
        public static List<Note> entriesToDiscover = new List<Note>();

        public static void GatherNotes()
        {
            foreach (string file in Directory.GetFiles(@"..\..\..\Content\Lore"))
            {
                int nameStart = file.LastIndexOf('\\') + 1;

                string name = file.Substring(nameStart, file.Length - nameStart - 4);

                StreamReader reader = new StreamReader(file);

                Note note = new Note(name);

                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    note.message.Add(line);
                }

                entriesToDiscover.Add(note);
            }
        }

        public static string DiscoverNote()
        {
            Random random = new Random();

            int index = random.Next(0, entriesToDiscover.Count);

            Note newNote = entriesToDiscover[index];

            entriesToDiscover.Remove(newNote);

            gatheredNotes.Add(newNote);

            return newNote.title;
        }
    }
}
