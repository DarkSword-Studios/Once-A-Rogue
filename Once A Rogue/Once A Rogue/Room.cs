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
                    spriteBatch.Draw(tilemap, finalRoomAnnex[row, col].RelativeLocation, finalRoomAnnex[row, col].RelativeImageLocal, finalRoomAnnex[row, col].DetermineTileColor());
                    finalRoomAnnex[row, col].InvalidTag = false;
                    finalRoomAnnex[row, col].ValidTag = false;
                    col++;
                }
                col = 0;
                row++;
            }
        }

        public String UpdateEvents(Player player, Camera camera, String playerMove)
        {

            if(player.PosX == 120 && player.PosY > 440 && player.PosY < 480 && playerMove == "left" && doorLocals.Contains("LEFT"))
            {
                camera.Move("left");
                return "left";
            }

            if (player.PosX == camera.screenWidth - player.PosRect.Width - 120 && player.PosY > 440 && player.PosY < 480 && playerMove == "right" && doorLocals.Contains("RIGHT"))
            {
                camera.Move("right");
                return "right";
            }

            if (player.PosY == 120 && player.PosX > 840 && player.PosX < 880 && playerMove == "up" && doorLocals.Contains("UP"))
            {
                camera.Move("up");
                return "up";
            }

            if (player.PosY == camera.screenHeight - player.PosRect.Height - 120 && player.PosX > 840 && player.PosX < 880 && playerMove == "down" && doorLocals.Contains("DOWN"))
            {
                camera.Move("down");
                return "down";
            }        

            return "NONE";

        }

        public void TagTiles(Point origin, int posXTiles, int negXTiles, int posYTiles, int negYTiles)
        {
            int tileX = origin.X / 120;
            int tileY = origin.Y / 120;

            int maxX = tileX + posXTiles + 1;
            int maxY = tileY + posYTiles + 1;

            tileX -= negXTiles;
            tileY -= negYTiles;
            
            for(int y = tileY; y < maxY; y++)
            {
                for(int x = tileX; x < maxX; x++)
                {
                    if (x > 0 && x < 15 && y > 0 && y < 8)
                    {
                        finalRoomAnnex[y, x].ValidTag = true;
                    }
                    else if ((x == 0 || x == 15 || y == 0 || y == 8) && y > -1 && x > -1 && y < 9 && x < 16)
                    {
                        finalRoomAnnex[y, x].InvalidTag = true;
                    }
                }
            }

            
        }

        public void TagTiles(Point origin, int posXTiles, int negXTiles, int posYTiles, int negYTiles, Rectangle player, int rangeX, int rangeY)
        {
            int playerX = (player.X + player.Width / 2) / 120;
            int playerY = (player.Y + player.Height) / 120;

            int tileX = origin.X / 120;
            int tileY = origin.Y / 120;

            if(tileX > (playerX + rangeX - posXTiles))
            {
                tileX = playerX + rangeX - posXTiles;
            }

            if (tileX < (playerX - rangeX + negXTiles))
            {
                tileX = playerX - rangeX + posXTiles;
            }

            if (tileY > (playerY + rangeY - posYTiles))
            {
                tileY = playerY + rangeY - posYTiles;
            }

            if (tileY < (playerY - rangeY + negYTiles))
            {
                tileY = playerY - rangeY + posYTiles;
            }

            int maxX = tileX + posXTiles + 1;
            int maxY = tileY + posYTiles + 1;

            tileX -= negXTiles;
            tileY -= negYTiles;

            for (int y = tileY; y < maxY; y++)
            {
                for (int x = tileX; x < maxX; x++)
                {
                    if (x > 0 && x < 15 && y > 0 && y < 8)
                    {
                        finalRoomAnnex[y, x].ValidTag = true;
                    }
                    else if ((x == 0 || x == 15 || y == 0 || y == 8) && y > -1 && x > -1 && y < 9 && x < 16)
                    {
                        finalRoomAnnex[y, x].InvalidTag = true;
                    }
                }
            }
        }

    }
}
