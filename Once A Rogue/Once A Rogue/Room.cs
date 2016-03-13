using System;
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
        int[,] roomAnnex = new int[9,16];

        //The tilesize we are working with is 120 pixels
        const int TILESIZE = 120;

        //This constructor will read in a file and populate the room annex array
        public Room(string file)
        {
            //Open the txt room
            StreamReader reader = new StreamReader(file);

            //Keeps track of placement
            string line = "";
            int row = 0;
            int col = 0;

            //Read the file until empty
            while((line = reader.ReadLine()) != null)
            {
                //Separate all values into an array ( ',' is the delimiter)
                string[] tiles = line.Split(',');

                //For each tile in the array
                foreach(string tile in tiles)
                {
                    //Load the tile code into the room annex array
                    roomAnnex[row, col] = int.Parse(tile);
                    col++;
                }

                col = 0;
                row++;
            }
        }
        //This method draws a room, given the sprite batch, the tilemap, and an x / y coordinate
        public void DrawRoom(SpriteBatch spriteBatch, Texture2D tilemap, int xCoord, int yCoord)
        {
            int row = 0;
            int col = 0;

            //Loop through every item in the room annex to deal with drawing tiles
            while(row < roomAnnex.GetLength(0))
            {
                while(col < roomAnnex.GetLength(1))
                {
                    //Get the relative coordinates of the tile within the room's space
                    int tileY = roomAnnex[row, col] / 16;
                    int tileX = roomAnnex[row, col] % 16;

                    //Locate the specified tile, and give it a new location within the room
                    Rectangle tile = new Rectangle(tileX * TILESIZE, tileY * TILESIZE, TILESIZE, TILESIZE);
                    Vector2 location = new Vector2(xCoord + (TILESIZE * col), yCoord + (TILESIZE * row));

                    //Draw the tile
                    spriteBatch.Draw(tilemap, location, tile, Color.White);
                    col++;
                }
                col = 0;
                row++;
            }
        }
    }
}
