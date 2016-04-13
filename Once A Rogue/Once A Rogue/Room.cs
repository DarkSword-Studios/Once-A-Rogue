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
//Date Modified: 3/17/16

namespace Once_A_Rogue
{
    class Room
    {
        //This array stores all of the tile codes for the room
        int[,] unformattedRoomAnnex = new int[9,16];

        //This array stores all of the tiles which have been converted from codes
        Tile[,] finalRoomAnnex = new Tile[9,16];

        //The tilesize we are working with is 120 pixels
        const int TILESIZE = 120;
        
        //This variables manages the room's activity state which is important (do we update the room? Do we draw it? Only if it's active)
        private Boolean active;

        //Keep track of the connections that the room supports
        private string doorLocals;

        private Boolean discovered;
        private Boolean aware;

        public Boolean Aware
        {
            get
            {
                return aware;
            }

            set
            {
                aware = value;
            }
        }

        public Boolean Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
                if (value)
                {
                    discovered = true;
                    aware = false;
                }          
            }
        }

        public Boolean Discovered
        {
            get
            {
                return discovered;
            }
        }

        public string DoorLocals
        {
            get
            {
                return doorLocals;
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
            discovered = false;
            aware = false;

            //Fixes a slight consistency error
            if (doorLocals == "ALLDIRECTIONS")
            {
                doorLocals = "LEFTUPRIGHTDOWN";
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
        //This method takes care of the initial leg work for initializing a room so that it doesn't have to be done multiple times
        public void BuildRoom(int xCoord, int yCoord)
        {
            int row = 0;
            int col = 0;

            //Loop through every item in the room annex to deal with assigning tiles
            while(row < unformattedRoomAnnex.GetLength(0))
            {
                while(col < unformattedRoomAnnex.GetLength(1))
                {
                    //Get the relative coordinates of the tile within the room's space
                    int tileY = unformattedRoomAnnex[row, col] / 16;
                    int tileX = unformattedRoomAnnex[row, col] % 16;

                    //Locate the specified tile texture, and give it a new location within the room
                    Rectangle imageLocal = new Rectangle(tileX * TILESIZE, tileY * TILESIZE, TILESIZE, TILESIZE);
                    Rectangle location = new Rectangle(xCoord + (TILESIZE * col), yCoord + (TILESIZE * row), TILESIZE, TILESIZE);

                    //Create a tile object which will keep track of its texture and location
                    Tile tile = new Tile(imageLocal, location);

                    //Load the initialized tile into the tile array
                    finalRoomAnnex[row, col] = tile;

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
            while (row < unformattedRoomAnnex.GetLength(0))
            {
                while (col < unformattedRoomAnnex.GetLength(1))
                {
                    //Locate the specified tile, and give it a new location within the room
                    finalRoomAnnex[row, col].RelativeLocation = new Rectangle(xCoord + (TILESIZE * col), yCoord + (TILESIZE * row), TILESIZE, TILESIZE);

                    //Draw the tile
                    spriteBatch.Draw(tilemap, finalRoomAnnex[row, col].RelativeLocation, finalRoomAnnex[row, col].RelativeImageLocal, finalRoomAnnex[row, col].DetermineTileColor());

                    //Always reset a tile's tags after drawing; if the tile tags should still be true, they will be updated as such in the next update call
                    finalRoomAnnex[row, col].InvalidTag = false;
                    finalRoomAnnex[row, col].ValidTag = false;
                    col++;
                }
                col = 0;
                row++;
            }
        }
        //This method handles whether or not the camera should be moved to an adjacent room
        public String UpdateEvents(Player player, Camera camera, String playerMove)
        {
            //Each of these if statements asks if the player is standing right in front of a door (not on the edges) and moving in the direction of that door
            if(player.PosX == 120 && player.PosY > 440 && player.PosY < 480 && playerMove == "left" && doorLocals.Contains("LEFT"))
            {
                //If the case is satisfied, it means that the player is trying to go through the door
                //In this case we call for a new camera move in the approrpriate direction, and then return a direction in string format
                camera.Move("left");
                return "left";
            }

            //Examine the previous if statement to understand how the rest of them work
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

            //If the player has not indicated that they want to travel to an adjacent room, return none.
            return "NONE";

        }

        //This method tags tiles in the room to be colored differently (if they are withing range) in order to indicate whether or not the tiles are valid
        //Green is valid; Red is invalid
        //The purpose of tile tagging is to show the range / effectiveness of an equiped spell / ability before casting
        public void TagTiles(Point origin, int posXTiles, int negXTiles, int posYTiles, int negYTiles)
        {
            //The origin point (usually the mouse) is converted into grid coordinates
            int tileX = origin.X / 120;
            int tileY = origin.Y / 120;

            //A rectangle is formed from the information given (origin, # of positive tiles in the x and y, # of negative tiles in the x and y)
            int maxX = tileX + posXTiles + 1;
            int maxY = tileY + posYTiles + 1;

            tileX -= negXTiles;
            tileY -= negYTiles;
            
            //For each tile marked by the method to be evaluated
            for(int y = tileY; y < maxY; y++)
            {
                for(int x = tileX; x < maxX; x++)
                {
                    //If the tile is not on a wall
                    if (x > 0 && x < 15 && y > 0 && y < 8)
                    {
                        //It is valid and will be colored green
                        finalRoomAnnex[y, x].ValidTag = true;
                    }
                    //If the tile is on a wall and not in nether space (nether space tiles may be marked)
                    else if ((x == 0 || x == 15 || y == 0 || y == 8) && y > -1 && x > -1 && y < 9 && x < 16)
                    {
                        //It is invalid and will be colored red
                        finalRoomAnnex[y, x].InvalidTag = true;
                    }
                }
            }

            
        }
        //This method is an overload of the above method
        //This one in particular binds the tile tagging square to the player - to ensure spells with a limited range can't be cast outside of the range
        public void TagTiles(Point origin, int posXTiles, int negXTiles, int posYTiles, int negYTiles, Rectangle player, int rangeX, int rangeY)
        {
            //Examine the middle point of the player's feet to get their origin point (in tile coordinates)
            int playerX = (player.X + player.Width / 2) / 120;
            int playerY = (player.Y + player.Height) / 120;

            //Examine the mouse origin in terms of tile coordinates
            int tileX = origin.X / 120;
            int tileY = origin.Y / 120;

            //If the mouse reaches too far in any one direction away from the player (compare mouse tile, player tile, and allowed range), the origin of the tile tagging square will be restricted and readjusted
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

            //The rest of this method is the same as the method above
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
