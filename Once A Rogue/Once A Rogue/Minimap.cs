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
        //Dimensions for the representations for the rooms (symbols on the minimap)
        private const int TILE_WIDTH = 150;
        private const int TILE_HEIGHT = 90;
        private const int TILE_SEPARATION = 10;

        //Keep track of whether or not the minimap should be drawn
        private static Boolean visible;

        //Property to get and set the minimap visibility
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

        //This method handles updating the representation of rooms that are adjacent to a room that just became active
        public static void UpdatePeripherals(Room[,] rooms, int xCoord, int yCoord)
        {
            //Grab the string that represents which connections a room possesses 
            string adjacentRooms = rooms[xCoord, yCoord].DoorLocals;

            //If the room has a connection (can have multiple), update the corresponding adjacent room to be aware
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

        //This method handles drawing the minimap on screen
        public static void Draw(Camera camera, SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures, Room[,] map, Boolean debug)
        {
            //Draw a mostly transparent black cover over the screen to put the game in the background
            spriteBatch.Draw(textures["BlackSlate"], new Rectangle(0, 0, camera.screenWidth, camera.screenHeight), Color.Black * 0.3f);

            //Loop through the rooms in the map
            for(int y = 0; y < map.GetLength(1); y++)
            {
                for(int x = 0; x < map.GetLength(0); x++)
                {
                    //If the room at the current location is not null, and has been discovered (or is in debug mode), and is the boss room
                    if ((map[x,y] != null && map[x, y].Boss && map[x, y].Discovered) || debug && (map[x, y] != null && map[x, y].Boss))
                    {
                        //Create relative coordinates for the symbol of the room
                        int xCoord = ((camera.screenWidth / 2) - (TILE_WIDTH * 3 / 4)) + ((x - (map.GetLength(0) / 2)) * (TILE_WIDTH + TILE_SEPARATION));
                        int yCoord = ((camera.screenHeight / 2) - (TILE_HEIGHT * 3 / 4)) + ((y - (map.GetLength(1) / 2)) * (TILE_HEIGHT + TILE_SEPARATION));

                        Rectangle location = new Rectangle(xCoord, yCoord, TILE_WIDTH, TILE_HEIGHT);

                        //If the current room is active, draw it with gold (map symbols should be semi-transparent as well)
                        if (map[x, y].Active)
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.Gold * 0.5f);
                        }
                        //Otherwise the boss room should be drawn with purple
                        else
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.Purple * 0.5f);
                        }
                    }
                    //If the room is not a boss room, is not null, and is discovered (or is in debug mode)
                    else if(map[x,y] != null && (map[x,y].Discovered || debug))
                    {
                        int xCoord = ((camera.screenWidth / 2) - (TILE_WIDTH * 3 / 4)) + ((x - (map.GetLength(0) / 2)) * (TILE_WIDTH + TILE_SEPARATION));
                        int yCoord = ((camera.screenHeight / 2) - (TILE_HEIGHT * 3 / 4)) + ((y - (map.GetLength(1) / 2)) * (TILE_HEIGHT + TILE_SEPARATION));

                        Rectangle location = new Rectangle(xCoord, yCoord, TILE_WIDTH, TILE_HEIGHT);

                        //If the room is active, draw it with gold
                        if (map[x, y].Active)
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.Gold * 0.5f);
                        }
                        //If the room is not active, draw it with white
                        else
                        {
                            spriteBatch.Draw(textures[map[x, y].DoorLocals], location, Color.White * 0.5f);
                        }
                        
                    }
                    //If the room is not discovered, but is not null and is aware
                    else if(map[x, y] != null && map[x, y].Aware)
                    {
                        int xCoord = ((camera.screenWidth / 2) - (TILE_WIDTH * 3 / 4)) + ((x - (map.GetLength(0) / 2)) * (TILE_WIDTH + TILE_SEPARATION));
                        int yCoord = ((camera.screenHeight / 2) - (TILE_HEIGHT * 3 / 4)) + ((y - (map.GetLength(1) / 2)) * (TILE_HEIGHT + TILE_SEPARATION));

                        Rectangle location = new Rectangle(xCoord, yCoord, TILE_WIDTH, TILE_HEIGHT);

                        if (map[x, y].Boss)
                        {
                            spriteBatch.Draw(textures["Unknown"], location, Color.Purple * 0.5f);
                            return;
                        }

                        //Draw an unknown tile at the current location to indicate the player could discover a room at this location
                        spriteBatch.Draw(textures["Unknown"], location, Color.White * 0.5f);

                    }
                }
            }
        }
    }
}
