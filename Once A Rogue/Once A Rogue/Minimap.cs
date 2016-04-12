using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Draws a minimap on screen if requested
//Date Modified: 4/12/16

namespace Once_A_Rogue
{
    class Minimap
    {
        public static void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Room[,] map, Boolean debug)
        {
            for(int y = 0; y < map.GetLength(1); y++)
            {
                for(int x = 0; x < map.GetLength(0); x++)
                {
                    if(map[x,y] != null && (map[x,y].Discovered == true || debug))
                    {
                        int xCoord = x * 100;
                        int yCoord = y * 100;

                        Rectangle location = new Rectangle(xCoord, yCoord, 100, 100);

                        //NOTE: Maybe draw using gold to indicate current room?
                        //spriteBatch.Draw(textures[map[x, y].DoorLocals + "map.png"], location, Color.White * 0.5f);
                        if (map[x, y].Active)
                        {
                            spriteBatch.Draw(textures["LEFTUPRIGHTDOWNmap.png"], location, Color.Gold * 0.5f);
                        }
                        else
                        {
                            spriteBatch.Draw(textures["LEFTUPRIGHTDOWNmap.png"], location, Color.White * 0.5f);
                        }
                        
                    }
                }
            }
        }
    }
}
