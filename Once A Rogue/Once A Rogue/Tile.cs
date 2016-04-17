using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Holds tile specific information
//Date Modified: 3/14/16

namespace Once_A_Rogue
{
    class Tile
    {
        //These are tile tags which determine if the specific tile will be colored normally, with a green tint (valid) or a red tint (invalid)
        private Boolean invalidTag = false;
        private Boolean validTag = false;

        private Interactable interactable;

        //Each tile gets its own texture (relative location on the tilemap) and relative location in the room to which it belongs
        private Rectangle relativeImageLocal;
        private Rectangle relativeLocation;

        private Tile door;
        private int doorLocal;

        public int DoorLocal
        {
            get
            {
                return doorLocal;
            }

            set
            {
                doorLocal = value;
            }
        }

        public Tile Door
        {
            get
            {
                return door;
            }

            set
            {
                door = value;
            }
        }

        public Interactable Interactable
        {
            get
            {
                return interactable;
            }

            set
            {
                interactable = value;
            }
        }

        //Each private tile attribute has a property (just a general get and set)
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

        //When making a tile a texture (relative location on the tilemap) and a location within the room must be specified
        public Tile(Rectangle imageLocal, Rectangle spacialLocation)
        {
            relativeImageLocal = imageLocal;
            relativeLocation = spacialLocation;
        }

        //This method examines the tile's current tag (can only be one of the following: normal, valid, invalid)
        public Color DetermineTileColor()
        {
            //If the tile is valid, return green
            if (validTag)
            {
                return Color.Green;
            }
            //If the tile is invalid, return red
            else if (invalidTag)
            {
                return Color.Red;
            }
            //If the tile has no special tags, return white, which will not add color to the tile when drawn
            else
            {
                return Color.White;
            }
        }
    }
}
