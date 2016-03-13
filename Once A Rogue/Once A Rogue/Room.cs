﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; //Needed to read in room files
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Builds an individual room from a text file
//Date Modified: 3/3/16

namespace Once_A_Rogue
{
    class Room
    {
        //This array stores all of the tile codes for the room
        int[,] unformattedRoomAnnex = new int[9,16];
        Tile[,] finalRoomAnnex = new Tile[9,16];

        //The tilesize we are working with is 120 pixels
        const int TILESIZE = 120;

        private Boolean active;

        private string doorLocals; 

        public Boolean Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }

        //This constructor will read in a file and populate the room annex array
        public Room(string file, Boolean activityState, string doors)
        {
            //Open the txt room
            StreamReader reader = new StreamReader(file);

            //Keeps track of placement
            string line = "";
            int row = 0;
            int col = 0;

            active = activityState;
            doorLocals = doors.ToUpper();

            if (doorLocals == "ALLDIRECTIONS")
            {
                doorLocals = "RIGHTLEFTUPDOWN";
            }

            //Read the file until empty
            while((line = reader.ReadLine()) != null)
            {
                //Separate all values into an array ( ',' is the delimiter)
                string[] tiles = line.Split(',');

                //For each tile in the array
                foreach(string tile in tiles)
                {
                    //Load the tile code into the room annex array
                    unformattedRoomAnnex[row, col] = int.Parse(tile);
                    col++;
                }

                col = 0;
                row++;
            }
        }
        //This method draws a room, given the sprite batch, the tilemap, and an x / y coordinate
        public void BuildRoom(int xCoord, int yCoord)
        {
            int row = 0;
            int col = 0;

            //Loop through every item in the room annex to deal with drawing tiles
            while(row < unformattedRoomAnnex.GetLength(0))
            {
                while(col < unformattedRoomAnnex.GetLength(1))
                {
                    //Get the relative coordinates of the tile within the room's space
                    int tileY = unformattedRoomAnnex[row, col] / 16;
                    int tileX = unformattedRoomAnnex[row, col] % 16;

                    //Locate the specified tile, and give it a new location within the room
                    Rectangle imageLocal = new Rectangle(tileX * TILESIZE, tileY * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle location = new Rectangle(xCoord + (TILESIZE * col), yCoord + (TILESIZE * row), TILESIZE, TILESIZE);

                    Tile tile = new Tile(imageLocal, location);
                    finalRoomAnnex[row, col] = tile;

                    col++;
                }
                col = 0;
                row++;
            }
        }

        public void DrawRoom(SpriteBatch spriteBatch, Texture2D tilemap, int xCoord, int yCoord)
        {
            int row = 0;
            int col = 0;

            //Loop through every item in the room annex to deal with drawing tiles
            while (row < unformattedRoomAnnex.GetLength(0))
            {
                while (col < unformattedRoomAnnex.GetLength(1))
                {
                    //Locate the specified tile, and give it a new location within the room
                    finalRoomAnnex[row, col].RelativeLocation = new Rectangle(xCoord + (TILESIZE * col), yCoord + (TILESIZE * row), TILESIZE, TILESIZE);

                    //Draw the tile
                    spriteBatch.Draw(tilemap, finalRoomAnnex[row, col].RelativeLocation, finalRoomAnnex[row, col].RelativeImageLocal, Color.White);
                    col++;
                }
                col = 0;
                row++;
            }
        }

        public String CheckChangeRoom(Player player, Camera camera, String playerMove)
        {
            int row = 0;
            int col = 0;

            while (row < unformattedRoomAnnex.GetLength(0))
            {
                while (col < unformattedRoomAnnex.GetLength(1))
                {
                    if (unformattedRoomAnnex[row, col] == 79 && playerMove == "right")
                    {
                        if (player.PosRect.Intersects(finalRoomAnnex[row, col].RelativeLocation))
                        {
                            if (doorLocals.Contains("RIGHT"))
                            {
                                camera.Move("right");
                                return "right";
                            }
                        }
                    }

                    if (unformattedRoomAnnex[row, col] == 64)
                    {
                        if (player.PosRect.Intersects(finalRoomAnnex[row, col].RelativeLocation) && playerMove == "left")
                        {
                            if (doorLocals.Contains("LEFT"))
                            {
                                camera.Move("left");
                                return "left";
                            }
                        }
                    }

                    if (unformattedRoomAnnex[row, col] == 8)
                    {
                        if (player.PosRect.Intersects(finalRoomAnnex[row, col].RelativeLocation) && playerMove == "up")
                        {
                            if (doorLocals.Contains("UP"))
                            {
                                camera.Move("up");
                                return "up";
                            }
                        }
                    }

                    if (unformattedRoomAnnex[row, col] == 136)
                    {
                        if (player.PosRect.Intersects(finalRoomAnnex[row, col].RelativeLocation) && playerMove == "down")
                        {
                            if (doorLocals.Contains("DOWN"))
                            {
                                camera.Move("down");
                                return "down";
                            }
                        }
                    }
                    col++;
                }
                col = 0;
                row++;
              
            }

            return "NONE";

        }
    }
}
