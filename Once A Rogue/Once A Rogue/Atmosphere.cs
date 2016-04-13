using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Atmosphere
    {
        private static Texture2D filter = null;
        private static Camera camera = null;

        public static Texture2D Filter
        {
            set
            {
                if(filter == null)
                {
                    filter = value;
                }
            }          
        }

        public static Camera Camera
        {
            set
            {
                if (camera == null)
                {
                    camera = value;
                }
            }
        }

        public static void BossFilter(SpriteBatch spriteBatch, int xCoord, int yCoord)
        {
            spriteBatch.Draw(filter, new Rectangle(xCoord, yCoord, camera.screenWidth, camera.screenHeight), Color.Purple * 0.2f);
        }

    }
}
