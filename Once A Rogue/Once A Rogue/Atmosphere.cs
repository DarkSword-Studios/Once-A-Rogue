using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Puts a tint of color on every object in the game (in a certain area)
//Date Modified: 4/17/16

namespace Once_A_Rogue
{
    class Atmosphere
    {
        //The Atmosphere class needs a filter texture and a camera
        private static Texture2D filter = null;
        private static Camera camera = null;

        //This is a public property to set the Atmosphere's filter
        public static Texture2D Filter
        {
            set
            {
                filter = value;
            }          
        }

        //This is a public property to set the Atmosphere's camera
        public static Camera Camera
        {
            set
            {
                camera = value;
            }
        }

        //This method handles drawing an atmosphere for the boss room
        public static void BossFilter(SpriteBatch spriteBatch, int xCoord, int yCoord)
        {
            //Put a purple tint on everything in the room
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.Purple * 0.2f);
        }

    }
}
