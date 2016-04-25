using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for colors
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Stores data for specific notifications
//Date Modified: 4/17/16

namespace Once_A_Rogue
{
    class Alert
    {
        //Each alert can store a message, color, number of ticks to be on screen, and whether or not it is inverted
        public string message;
        public Color color;
        public int stationaryTicks;
        public Boolean invert;

        //This is the constructor for an alert, which accepts all of the items an alert can store
        public Alert(string message, Color color, int stationaryTicks, Boolean invert)
        {
            //All values are set when the object is initialized
            this.message = message;
            this.color = color;
            this.stationaryTicks = stationaryTicks;
            this.invert = invert;
        }
    }
}
