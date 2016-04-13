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
        private const int TILE_WIDTH = 150;
        private const int TILE_HEIGHT = 90;

        private const int TILE_SEPARATION = 10;

        private static Boolean visible;

        public static Boolean Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        public static void UpdatePeripherals(Room[,] rooms, int xCoord, int yCoord)
        {
            string adjacentRooms = rooms[xCoord, yCoord].DoorLocals;

            if (adjacentRooms.Contains("LEFT"))
            {
                rooms[xCoord - 1, yCoord].Aware = true;
            }
            if (adjacentRooms.Contains("UP"))
            {
                rooms[xCoord, yCoord - 1].Aware = true;
            }
            if (adjacentRooms.Contains("RIGHT"))
            {
                rooms[xCoord + 1, yCoord].Aware = true;
            }
            if (adjacentRooms.Contains("DOWN"))
            {
                rooms[xCoord, yCoord + 1].Aware = true;
            }
        }

        public static void Draw(Camera camera, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Room[,] map, Boolean debug)
        {
            spriteBatch.Draw(textures["BlackSlate"], new Rectangle(0, 0, camera.screenWidth, camera.screenHeight), Color.Black * 0.3f);

            for(int y = 0; y < map.GetLength(1); y++)
            {
                for(int x = 0; x < map.GetLength(0); x++)
                {
                    if((map[x,y] != null && map[x, y].Boss && map[x, y].Discovered) || debug && (map[x, y] != null && map[x, y].Boss))
                    {
                        int xCoord = ((camera.screenWidth / 2) - (TILE_WIDTH * 3 / 4)) + ((x - (map.GetLength(0) / 2)) * (TILE_WIDTH + TILE_SEPARATION));
                        int yCoord = ((camera.screenHeight / 2) - (TILE_HEIGHT * 3 / 4)) + ((y - (map.GetLength(1) / 2)) * (TILE_HEIGHT + TILE_SEPARATION));

                        Rectangle location = new Rectangle(xCoord, yCoord, TILE_WIDTH, TILE_HEIGHT);

                        //if (map[x, y].Active && !map[x, y].Discovered)
                        //{
                        //    spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.Gold * 0.5f);
                        //}
                        if (map[x, y].Active)
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.Gold * 0.5f);
                        }
                        else
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.Purple * 0.5f);
                        }
                        //else
                        //{
                        //    spriteBatch.Draw(textures["whiteSlate"], location, Color.Purple * 0.5f);
                        //}
                    }
                    else if(map[x,y] != null && (map[x,y].Discovered || debug))
                    {
                        int xCoord = ((camera.screenWidth / 2) - (TILE_WIDTH * 3 / 4)) + ((x - (map.GetLength(0) / 2)) * (TILE_WIDTH + TILE_SEPARATION));
                        int yCoord = ((camera.screenHeight / 2) - (TILE_HEIGHT * 3 / 4)) + ((y - (map.GetLength(1) / 2)) * (TILE_HEIGHT + TILE_SEPARATION));

                        Rectangle location = new Rectangle(xCoord, yCoord, TILE_WIDTH, TILE_HEIGHT);

                        if (map[x, y].Active)
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.Gold * 0.5f);
                        }
                        else
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.White * 0.5f);
                        }
                        
                    }
                    else if(map[x, y] != null && map[x, y].Aware)
                    {
                        int xCoord = ((camera.screenWidth / 2) - (TILE_WIDTH * 3 / 4)) + ((x - (map.GetLength(0) / 2)) * (TILE_WIDTH + TILE_SEPARATION));
                        int yCoord = ((camera.screenHeight / 2) - (TILE_HEIGHT * 3 / 4)) + ((y - (map.GetLength(1) / 2)) * (TILE_HEIGHT + TILE_SEPARATION));

                        Rectangle location = new Rectangle(xCoord, yCoord, TILE_WIDTH, TILE_HEIGHT);

                        spriteBatch.Draw(textures["Unknown"], location, Color.White * 0.5f);

                    }
                }
            }
        }
    }
}
