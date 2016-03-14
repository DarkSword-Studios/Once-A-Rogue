using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Tile
    {
        private Boolean invalidTag = false;
        private Boolean validTag = false;

        private Rectangle relativeImageLocal;
        private Rectangle relativeLocation;

        public Boolean InvalidTag
        {
            get
            {
                return invalidTag;
            }

            set
            {
                invalidTag = value;
            }
        }

        public Boolean ValidTag
        {
            get
            {
                return validTag;
            }

            set
            {
                validTag = value;
            }
        }

        public Rectangle RelativeImageLocal
        {
            get
            {
                return relativeImageLocal;
            }

            set
            {
                relativeImageLocal = value;
            }
        }

        public Rectangle RelativeLocation
        {
            get
            {
                return relativeLocation;
            }

            set
            {
                relativeLocation = value;
            }
        }

        public Tile(Rectangle imageLocal, Rectangle spacialLocation)
        {
            relativeImageLocal = imageLocal;
            relativeLocation = spacialLocation;
        }

        public Color DetermineTileColor()
        {
            if (validTag)
            {
                return Color.Green;
            }
            else if (invalidTag)
            {
                return Color.Red;
            }
            else
            {
                return Color.White;
            }
        }
    }
}
