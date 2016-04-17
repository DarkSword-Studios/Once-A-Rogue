using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for colors
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Allows for notifications to be put onscreen
//Date Modified: 4/17/16

namespace Once_A_Rogue
{
    class Alert
    {
        public string message;
        public Color color;

        public Alert(string message, Color color)
        {
            this.message = message;
            this.color = color;
        }
    }
}
