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
        public Tile[,] finalRoomAnnex = new Tile[9,16];

        //The tilesize we are working with is 120 pixels
        const int TILESIZE = 120;

        List<Interactable> interactables;
        
        //This variables manages the room's activity state which is important (do we update the room? Do we draw it? Only if it's active)
        private Boolean active;

        //Keep track of the connections that the room supports
        private string doorLocals;

        private Boolean discovered;
        private Boolean aware;
        private Boolean boss;
        private Boolean levelTrigger;

        //Handles Locking and Unlocking animations
        public Boolean isLocking = false;
        public Boolean isUnlocking = false;

        //Handles room states
        public Boolean locked = false;

        List<Tile> doorTiles = new List<Tile>();
        public List<Tile> spawnTiles = new List<Tile>();
        public List<Tile> checkPoints = new List<Tile>();
        public List<Enemy> enemyList = new List<Enemy>();

        Boolean clear = false;

        public Boolean LevelTrigger
        {
            get
            {
                return levelTrigger;
            }
        }

        public Boolean Boss
        {
            get
            {
                return boss;
            }

            set
            {
                boss = value;
            }
        }

        public Boolean Aware
        {
            get
            {
                return aware;
            }

            set
            {
                if (!discovered)
                {
                    aware = value;
                }
                
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

            interactables = new List<Interactable>();

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
        public void BuildRoom(int xCoord, int yCoord, string roomCodeStr)
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

            if(roomCodeStr != "1234")
            {
                return;
            }

            //This list keeps track of all of the rooms in Content/Rooms/ that would be valid fits for the current room to build
            List<string> possibleRooms = new List<string>();

            //Read in each file in the Content/Rooms/ folder (where we keep the room.txt files)
            foreach (string file in Directory.GetFiles(@"..\..\..\Content\InteractableLayer"))
            {
                //The purpose of this code is to determine if the file is valid to put in the possible rooms list
                if (file.Contains(roomCodeStr))
                {
                    //Start of file processing by Ian
                    char[] letterArray = new char[file.Length];
                    List<int> numberArray = new List<int>();
                    List<int> roomCodeList = new List<int>();

                    //Store each character in the filename to a character array
                    for (int x = 0; x < file.Length; x++)
                    {
                        letterArray[x] = file[x];
                    }

                    //Pick out only the digits in the character array
                    for (int x = 0; x < roomCodeStr.Length; x++)
                    {
                        int roomCodeDigit;

                        int.TryParse(roomCodeStr[x].ToString(), out roomCodeDigit);

                        roomCodeList.Add(roomCodeDigit);
                    }

                    int fileNumber;

                    //Check the length of the room code
                    foreach (char c in letterArray)
                    {
                        string charString = c.ToString();
                        if (int.TryParse(charString, out fileNumber))
                        {
                            numberArray.Add(fileNumber);
                        }
                    }

                    //Verify the room code found in the file name against the room code needed
                    if (numberArray.Count == roomCodeList.Count)
                    {
                        int truthCount = 0;
                        for (int x = 0; x < numberArray.Count; x++)
                        {
                            if (numberArray[x] == roomCodeList[x])
                            {
                                truthCount += 1;
                            }
                        }

                        //If everything checks out, the room is added to the list of possible rooms
                        if (truthCount == roomCodeStr.Length)
                        {
                            possibleRooms.Add(file);
                        }
                    }
                }
            }
            //End of file processing by Ian

            Random random = new Random();

            //At this point a room file is chosen from the valid list
            string roomPath = possibleRooms[random.Next(0, possibleRooms.Count)];

            //Open the txt room
            StreamReader reader = new StreamReader(roomPath);

            //Keeps track of placement
            string line = "";
            row = 0;
            col = 0;

            int[,] interactableLayer = new int[9, 16];

            //Read the file until empty
            while ((line = reader.ReadLine()) != null)
            {
                //Separate all values into an array ( ',' is the delimiter)
                string[] tiles = line.Split(',');

                //For each tile in the array
                foreach (string tile in tiles)
                {
                    //Load the tile code into the room annex array
                    interactableLayer[row, col] = int.Parse(tile);
                    col++;
                }

                col = 0;
                row++;
            }

            row = 0;
            col = 0;

            //Loop through every item in the room annex to deal with assigning tiles
            while (row < interactableLayer.GetLength(0))
            {
                while (col < interactableLayer.GetLength(1))
                {
                    if(interactableLayer[row, col] != -1)
                    {
                        int tileY = interactableLayer[row, col] / 16;
                        int tileX = interactableLayer[row, col] % 16;

                        //Locate the specified tile texture, and give it a new location within the room
                        Rectangle imageLocal = new Rectangle(tileX * TILESIZE, tileY * TILESIZE, TILESIZE, TILESIZE);

                        int tileTag = interactableLayer[row, col];

                        if (tileTag == 82 || (tileTag >= 85 && tileTag < 89))
                        {
                            finalRoomAnnex[row, col].Interactable = new Interactable("Spawn", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, false, true);
                            finalRoomAnnex[row, col].Interactable.AssignSubType(tileTag);
                            spawnTiles.Add(finalRoomAnnex[row, col]);
                        }
                        if(tileTag >= 96 && tileTag < 104)
                        {
                            finalRoomAnnex[row, col].Interactable = new Interactable("Post", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, false, true);
                            finalRoomAnnex[row, col].Interactable.AssignSubType(tileTag);
                            spawnTiles.Add(finalRoomAnnex[row, col]);
                        }
                        //Load the interactable if there is one
                        else if(tileTag == 80)
                        {
                            finalRoomAnnex[row, col].Interactable = new Interactable("Box", finalRoomAnnex[row, col].RelativeLocation, imageLocal, false, false, true);
                        }
                        else if(tileTag == 84)
                        {
                            finalRoomAnnex[row, col].Interactable = new Interactable("CheckPoint", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, false, true);
                            checkPoints.Add(finalRoomAnnex[row, col]);
                            finalRoomAnnex[row, col].localX = col;
                            finalRoomAnnex[row, col].localY = row;


                        }
                        else
                        {
                            finalRoomAnnex[row, col].Interactable = new Interactable("Note", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, true, true);
                        }
                        interactables.Add(finalRoomAnnex[row, col].Interactable);
                        
                    }
                    col++;
                }
                col = 0;
                row++;
            }
        }

        //This method draws a room, given the sprite batch, the tilemap, and an x / y coordinate
        public void DrawRoom(SpriteBatch spriteBatch, Texture2D tilemap, int xCoord, int yCoord, Camera camera)
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

                    if(finalRoomAnnex[row, col].Interactable != null && finalRoomAnnex[row, col].Interactable.DoDraw)
                    {
                        spriteBatch.Draw(tilemap, finalRoomAnnex[row, col].RelativeLocation, finalRoomAnnex[row, col].Interactable.RelativeImageLocal, finalRoomAnnex[row, col].DetermineTileColor());
                    }

                    if(finalRoomAnnex[row, col].Door != null)
                    {
                        spriteBatch.Draw(tilemap, finalRoomAnnex[row, col].Door.RelativeLocation, finalRoomAnnex[row, col].Door.RelativeImageLocal, finalRoomAnnex[row, col].DetermineTileColor());
                    }

                    //Always reset a tile's tags after drawing; if the tile tags should still be true, they will be updated as such in the next update call
                    finalRoomAnnex[row, col].InvalidTag = false;
                    finalRoomAnnex[row, col].ValidTag = false;
                    col++;
                }
                col = 0;
                row++;
            }
            foreach(Enemy enemy in enemyList)
            {
                
                if(camera.xMod != enemy.relativeCamX)
                {
                    enemy.PosX += camera.xMod - enemy.relativeCamX;
                    enemy.relativeCamX = camera.xMod;
                }
                if (camera.yMod != enemy.relativeCamY)
                {
                    enemy.PosY += camera.yMod - enemy.relativeCamY;
                    enemy.relativeCamY = camera.yMod;
                }

                enemy.Draw(spriteBatch, 140, 140);

            }

            if (boss)
            {
                Atmosphere.BossFilter(spriteBatch, xCoord, yCoord);
            }

            
        }
        //This method handles whether or not the camera should be moved to an adjacent room
        public String UpdateEvents(Player player, Camera camera, String playerMove, GameTime gameTime)
        {
            foreach (Interactable interactable in interactables)
            {
                if (interactable.Activatable)
                {
                    interactable.Interact(player, camera);

                    if (interactable.LevelTrigger)
                    {
                        this.levelTrigger = true;
                    }
                }
                if (interactable.Passable)
                {
                    continue;
                }
                else
                {
                    interactable.HandleCollisions(player, camera);
                }


            }

            if (isLocking)
            {
                foreach (Tile door in doorTiles)
                {
                    switch (door.DoorLocal)
                    {
                        case 1:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X + 3, door.Door.RelativeLocation.Y, TILESIZE, TILESIZE);
                            break;

                        case 2:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X, door.Door.RelativeLocation.Y + 3, TILESIZE, TILESIZE);
                            break;

                        case 3:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X - 3, door.Door.RelativeLocation.Y, TILESIZE, TILESIZE);
                            break;

                        case 4:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X, door.Door.RelativeLocation.Y - 3, TILESIZE, TILESIZE);
                            break;
                    }

                }

                //If one door is done closing, the rest of them should be done as well
                if (doorTiles[0].RelativeLocation.Equals(doorTiles[0].Door.RelativeLocation))
                {
                    isLocking = false;
                }
            }

            if (isUnlocking)
            {
                foreach (Tile door in doorTiles)
                {
                    switch (door.DoorLocal)
                    {
                        case 1:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X - 3, door.Door.RelativeLocation.Y, TILESIZE, TILESIZE);
                            break;

                        case 2:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X, door.Door.RelativeLocation.Y - 3, TILESIZE, TILESIZE);
                            break;

                        case 3:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X + 3, door.Door.RelativeLocation.Y, TILESIZE, TILESIZE);
                            break;

                        case 4:

                            door.Door.RelativeLocation = new Rectangle(door.Door.RelativeLocation.X, door.Door.RelativeLocation.Y + 3, TILESIZE, TILESIZE);
                            break;
                    }

                }

                //If one door is done closing, the rest of them should be done as well
                if (!doorTiles[0].RelativeLocation.Intersects(doorTiles[0].Door.RelativeLocation))
                {
                    doorTiles.Clear();
                    isUnlocking = false;
                    locked = false;
                }
            }

            //Each of these if statements asks if the player is standing right in front of a door (not on the edges) and moving in the direction of that door
            if (player.PosX == 80 && player.PosY > 440 && player.PosY < 480 && playerMove == "left" && doorLocals.Contains("LEFT") && !locked)
            {
                //If the case is satisfied, it means that the player is trying to go through the door
                //In this case we call for a new camera move in the approrpriate direction, and then return a direction in string format
                camera.Move("left");
                return "left";
            }

            //Examine the previous if statement to understand how the rest of them work
            if (player.PosX == camera.screenWidth - player.PosRect.Width - 80 && player.PosY > 440 && player.PosY < 480 && playerMove == "right" && doorLocals.Contains("RIGHT") && !locked)
            {
                camera.Move("right");
                return "right";
            }

            if (player.PosY == 80 && player.PosX > 840 && player.PosX < 880 && playerMove == "up" && doorLocals.Contains("UP") && !locked)
            {
                camera.Move("up");
                return "up";
            }

            if (player.PosY == camera.screenHeight - player.PosRect.Height - 120 && player.PosX > 840 && player.PosX < 880 && playerMove == "down" && doorLocals.Contains("DOWN") && !locked)
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

        public void Lock()
        {
            if (locked || isLocking || isUnlocking)
            {
                return;
            }

            isLocking = true;
            locked = true;

            if (doorLocals.Contains("UP"))
            {
                finalRoomAnnex[0, 7].Door = new Tile(new Rectangle(14 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[0, 7].RelativeLocation.X, finalRoomAnnex[0, 7].RelativeLocation.Y - TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[0, 7].DoorLocal = 2;

                finalRoomAnnex[0, 6].Door = new Tile(new Rectangle(11 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[0, 6].RelativeLocation.X, finalRoomAnnex[0, 6].RelativeLocation.Y - TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[0, 6].DoorLocal = 2;

                finalRoomAnnex[0, 8].Door = new Tile(new Rectangle(12 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[0, 8].RelativeLocation.X, finalRoomAnnex[0, 8].RelativeLocation.Y - TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[0, 8].DoorLocal = 2;

                doorTiles.Add(finalRoomAnnex[0, 7]);
                doorTiles.Add(finalRoomAnnex[0, 6]);
                doorTiles.Add(finalRoomAnnex[0, 8]);

            }

            if (doorLocals.Contains("LEFT"))
            {
                finalRoomAnnex[3, 0].Door = new Tile(new Rectangle(8 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[3, 0].RelativeLocation.X - TILESIZE, finalRoomAnnex[3, 0].RelativeLocation.Y, TILESIZE, TILESIZE));
                finalRoomAnnex[3, 0].DoorLocal = 1;

                finalRoomAnnex[4, 0].Door = new Tile(new Rectangle(15 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[4, 0].RelativeLocation.X - TILESIZE, finalRoomAnnex[4, 0].RelativeLocation.Y, TILESIZE, TILESIZE));
                finalRoomAnnex[4, 0].DoorLocal = 1;

                finalRoomAnnex[5, 0].Door = new Tile(new Rectangle(7 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[5, 0].RelativeLocation.X - TILESIZE, finalRoomAnnex[5, 0].RelativeLocation.Y, TILESIZE, TILESIZE));
                finalRoomAnnex[5, 0].DoorLocal = 1;

                doorTiles.Add(finalRoomAnnex[3, 0]);
                doorTiles.Add(finalRoomAnnex[4, 0]);
                doorTiles.Add(finalRoomAnnex[5, 0]);

            }

            if (doorLocals.Contains("RIGHT"))
            {
                finalRoomAnnex[3, 15].Door = new Tile(new Rectangle(10 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[3, 15].RelativeLocation.X + TILESIZE, finalRoomAnnex[3, 15].RelativeLocation.Y, TILESIZE, TILESIZE));
                finalRoomAnnex[3, 15].DoorLocal = 3;

                finalRoomAnnex[4, 15].Door = new Tile(new Rectangle(0 * TILESIZE, 2 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[4, 15].RelativeLocation.X + TILESIZE, finalRoomAnnex[4, 15].RelativeLocation.Y, TILESIZE, TILESIZE));
                finalRoomAnnex[4, 15].DoorLocal = 3;

                finalRoomAnnex[5, 15].Door = new Tile(new Rectangle(9 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[5, 15].RelativeLocation.X + TILESIZE, finalRoomAnnex[5, 15].RelativeLocation.Y, TILESIZE, TILESIZE));
                finalRoomAnnex[5, 15].DoorLocal = 3;

                doorTiles.Add(finalRoomAnnex[3, 15]);
                doorTiles.Add(finalRoomAnnex[4, 15]);
                doorTiles.Add(finalRoomAnnex[5, 15]);

            }

            if (doorLocals.Contains("DOWN"))
            {
                finalRoomAnnex[8, 7].Door = new Tile(new Rectangle(13 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[8, 7].RelativeLocation.X, finalRoomAnnex[8, 7].RelativeLocation.Y + TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[8, 7].DoorLocal = 4;

                finalRoomAnnex[8, 6].Door = new Tile(new Rectangle(5 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[8, 6].RelativeLocation.X, finalRoomAnnex[8, 6].RelativeLocation.Y + TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[8, 6].DoorLocal = 4;

                finalRoomAnnex[8, 8].Door = new Tile(new Rectangle(6 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[8, 8].RelativeLocation.X, finalRoomAnnex[8, 8].RelativeLocation.Y + TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[8, 8].DoorLocal = 4;

                doorTiles.Add(finalRoomAnnex[8, 7]);
                doorTiles.Add(finalRoomAnnex[8, 6]);
                doorTiles.Add(finalRoomAnnex[8, 8]);

            }

        }

        public void RequestUnlock(Player player, Camera camera)
        {
            //Put room clearing code here... e.g. have all of the enemies been killed yet? 
            Unlock(player, camera);
        }

        private void Unlock(Player player, Camera camera)
        {
            if(isLocking || !locked || isUnlocking)
            {
                return;
            }

            isUnlocking = true;
            if (boss)
            {
                Notification.Alert("Boss cleared!", Color.Purple, 60, false);
                Notification.Alert("Next Level Unlocked!", Color.OrangeRed, 60, true);

                if(player.PosX <= camera.screenWidth / 2)
                {
                    Rectangle ladderImage = new Rectangle(3 * TILESIZE, 5 * TILESIZE, TILESIZE, TILESIZE);
                    finalRoomAnnex[2, 13].Interactable = new Interactable("Ladder", finalRoomAnnex[2, 13].RelativeLocation, ladderImage, true, true, true);
                    interactables.Add(finalRoomAnnex[2, 13].Interactable);
                }
                else
                {
                    Rectangle ladderImage = new Rectangle(3 * TILESIZE, 5 * TILESIZE, TILESIZE, TILESIZE);
                    finalRoomAnnex[2, 2].Interactable = new Interactable("Ladder", finalRoomAnnex[2, 2].RelativeLocation, ladderImage, true, true, true);
                    interactables.Add(finalRoomAnnex[2, 2].Interactable);
                }
            }
            else
            {
                Notification.Alert("Room Cleared!", Color.Green, 60, false);
            }
            
        }
        public void SpawnGoblin(Player play, Texture2D tex, Camera camera)
        {

            Random randy = new Random();
            Tile spawn = spawnTiles[randy.Next(0, spawnTiles.Count)];
            spawnTiles.Remove(spawn);
            Goblin goblin = new Goblin(play, camera, spawn.RelativeLocation.X, spawn.RelativeLocation.Y, 140, 140, tex);
            enemyList.Add(goblin);
            
         
        }

        public void SpawnGhoul(Player play, Texture2D tex, Camera camera)
        {
            Random randy = new Random();
            Tile spawn = spawnTiles[randy.Next(0, spawnTiles.Count)];
            spawnTiles.Remove(spawn);
            Ghoul ghoul = new Ghoul(play, camera, 0, spawn.RelativeLocation.X, spawn.RelativeLocation.Y, 140, 140, tex);
            enemyList.Add(ghoul);
        }
        
        public void SpawnKobold(Player play, Texture2D tex, Camera camera)
        {

            Random randy = new Random();
            Tile spawn = spawnTiles[randy.Next(0, spawnTiles.Count)];
            spawnTiles.Remove(spawn);
            Kobold kobold = new Kobold(play, camera, spawn.RelativeLocation.X, spawn.RelativeLocation.Y, 140, 140, tex);
            enemyList.Add(kobold);


        }
    }
}
