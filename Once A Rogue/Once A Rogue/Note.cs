using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue
{
    class Note
    {


        public string title;
        public List<String> message;
        public Boolean read;

        public Note(string title)
        {
            this.title = title;
            this.message = new List<string>();
            read = false;
        }
    }
}
