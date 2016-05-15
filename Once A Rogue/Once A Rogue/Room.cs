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

        //Manage a list of all interactables (also a subset of just post interactables)
        public List<Interactable> interactables;
        public List<Interactable> posts = new List<Interactable>();

        //This variables manages the room's activity state which is important (do we update the room? Do we draw it? Only if it's active)
        private Boolean active;

        //Keep track of the connections that the room supports
        private string doorLocals;

        //Is this room the starting room
        public Boolean startingRoom = false;

        //Keep track of whether the room has been discovered, is aware, or is neither
        private Boolean discovered;
        private Boolean aware;

        //Keep track of whether or not the room is a boss room, and if the a new level should be generated (did the player climb a ladder?)
        private Boolean boss;
        private Boolean levelTrigger;

        //Handles Locking and Unlocking animations
        public Boolean isLocking = false;
        public Boolean isUnlocking = false;

        //Handles room states
        public Boolean locked = false;

        //Keep track of specific kinds of interactables
        List<Tile> doorTiles = new List<Tile>();
        public List<Tile> spawnTiles = new List<Tile>();
        public List<Tile> checkPoints = new List<Tile>();

        //Keep track of enemies, and enemies to remove (usually if they are killed)
        public List<Enemy> enemyList = new List<Enemy>();
        List<Enemy> enemiesToRemove = new List<Enemy>();

        Random rand = new Random();

        //Boolean clear = false;

        //Properties to access (sometimes just get) private room attributes
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
                interactables.Clear();
                posts.Clear();
                spawnTiles.Clear();
                checkPoints.Clear();
                int row = 0;
                int col = 0;

                //Loop through every item in the room annex to deal with assigning tiles
                while (row < unformattedRoomAnnex.GetLength(0))
                {
                    while (col < unformattedRoomAnnex.GetLength(1))
                    {
                        finalRoomAnnex[row, col].Interactable = null;

                        col++;
                    }
                    col = 0;
                    row++;
                }
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
                //A room can only become aware if it has not been discovered
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

                //If a room becomes active, it has been discovered and is no longer just aware
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

            //--- At this point an interactable layer must be chosen ---

            //To avoid errors, only load in interactable layers for rooms that we have files for
            //if(roomCodeStr != "1234" && roomCodeStr != "14" && roomCodeStr != "23" && roomCodeStr != "3" && roomCodeStr != "1")
            //{
            //    return;
            //}

            //This list keeps track of all of the interactable layers in Content/InteractableLayer/ that would be valid fits for the current room
            List<string> possibleRooms = new List<string>();

            //Read in each file in the Content/InteractableLayer/ folder (where we keep the room.txt files)
            foreach (string file in Directory.GetFiles(@"..\..\..\Content\InteractableLayer"))
            {
                if (file.Contains("INCLUDE_ALL"))
                {
                    possibleRooms.Add(file);
                }

                //The purpose of this code is to determine if the file is valid to put in the possible rooms list (interactable layer list)
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

            //At this point an interactable layer file is chosen from the valid list
            string roomPath = possibleRooms[rand.Next(0, possibleRooms.Count)];

            //Open the txt interactable layer
            StreamReader reader = new StreamReader(roomPath);

            if (startingRoom)
            {
                int tileY = 81 / 16;
                int tileX = 81 % 16;

                //Locate the specified interactable texture
                Rectangle imageLocal = new Rectangle(tileX * TILESIZE, tileY * TILESIZE, TILESIZE, TILESIZE);

                finalRoomAnnex[7, 11].Interactable = new Interactable("Note", finalRoomAnnex[7, 11].RelativeLocation, imageLocal, true, true, true);
                interactables.Add(finalRoomAnnex[7, 11].Interactable);

                return;
            }

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

                //For each interactable in the array
                foreach (string tile in tiles)
                {
                    //Load the interactable code into the interactable layer array
                    interactableLayer[row, col] = int.Parse(tile);
                    col++;
                }

                col = 0;
                row++;
            }

            row = 0;
            col = 0;

            //Loop through every item in the interactable layer to deal with assigning interactables to tiles
            while (row < interactableLayer.GetLength(0))
            {
                while (col < interactableLayer.GetLength(1))
                {
                    //If the interactable code is -1, there isn't an interactable there
                    if(interactableLayer[row, col] != -1)
                    {
                        int tileY = interactableLayer[row, col] / 16;
                        int tileX = interactableLayer[row, col] % 16;

                        if (interactableLayer[row, col] == 80 || interactableLayer[row, col] == 64)
                        {
                            
                            //System.Threading.Thread.Sleep(1);
                            int randomNum = rand.Next(0, 2);

                            if(randomNum == 1)
                            {
                                tileY = 64 / 16;
                                tileX = 64 % 16;
                            }
                            else
                            {
                                tileY = interactableLayer[row, col] / 16;
                                tileX = interactableLayer[row, col] % 16;
                            }
                        }
                        else
                        {
                            tileY = interactableLayer[row, col] / 16;
                            tileX = interactableLayer[row, col] % 16;
                        }

                        //Locate the specified interactable texture
                        Rectangle imageLocal = new Rectangle(tileX * TILESIZE, tileY * TILESIZE, TILESIZE, TILESIZE);

                        int tileTag = interactableLayer[row, col];

                        //If the tileTag of the interactable is one of these values
                        if (tileTag == 82 || (tileTag >= 85 && tileTag < 89))
                        {
                            //It is a spawn tile, so create a new spawn tile interactable
                            finalRoomAnnex[row, col].Interactable = new Interactable("Spawn", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, false, false);

                            //Spawn tile interactables need to be assigned a subtype
                            finalRoomAnnex[row, col].Interactable.AssignSubType(tileTag);
                            
                            //Add this spawn tile to the room's list of spawn tiles
                            spawnTiles.Add(finalRoomAnnex[row, col]);
                        }
                        else if(tileTag >= 96 && tileTag < 104)
                        {
                            //It is a post tile, so create a new post interactable
                            finalRoomAnnex[row, col].Interactable = new Interactable("Post", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, false, false);

                            //Post tiles need to be assigned a subtype
                            finalRoomAnnex[row, col].Interactable.AssignSubType(tileTag);
                            
                            //Add the post to the room's list of posts
                            posts.Add(finalRoomAnnex[row, col].Interactable);
                        }
                        else if(tileTag == 80)
                        {
                            //It is a box, so create a new box interactable
                            finalRoomAnnex[row, col].Interactable = new Interactable("Box", finalRoomAnnex[row, col].RelativeLocation, imageLocal, false, false, true);
                        }
                        else if(tileTag == 84)
                        {
                            //It is a checkpoint, so create a new checkpoint interactable
                            finalRoomAnnex[row, col].Interactable = new Interactable("CheckPoint", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, false, false);

                            //Add it to the room's list of checkpoints
                            checkPoints.Add(finalRoomAnnex[row, col]);

                            //Assign the tiles's localX and localY (it may be needed for future calculations)
                            finalRoomAnnex[row, col].localX = col;
                            finalRoomAnnex[row, col].localY = row;
                        }
                        else
                        {
                            //The only other type of interactable is a note, so create a note interactable
                            finalRoomAnnex[row, col].Interactable = new Interactable("Note", finalRoomAnnex[row, col].RelativeLocation, imageLocal, true, true, true);
                        }
                        //Add interactable - regardless of what type it is - to the room's interactables list
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

                    //If the current tile has an interactable associated with and the interactable is asking to be drawn
                    if(finalRoomAnnex[row, col].Interactable != null && finalRoomAnnex[row, col].Interactable.DoDraw)
                    {
                        //Draw the interactable
                        spriteBatch.Draw(tilemap, finalRoomAnnex[row, col].RelativeLocation, finalRoomAnnex[row, col].Interactable.RelativeImageLocal, finalRoomAnnex[row, col].DetermineTileColor());
                    }

                    //If the current tile has a door
                    if(finalRoomAnnex[row, col].Door != null)
                    {
                        //Draw the door on top of the current tile
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
            //Loop through the room's enemy list
            foreach(Enemy enemy in enemyList)
            {
                //Based on the camera's direction
                if (camera.direction == "right")
                {
                    //If the enemy has just spawned
                    if (enemy.justSpawned)
                    {
                        //Adjust their local coords to be offscreen
                        enemy.PosX += camera.screenWidth;

                        //The just spawned flag must be false now
                        enemy.justSpawned = false;
                    }
                    //If the enemy hasn't just spawned
                    else
                    {
                        //Update the enemy's position based on the direction of the camera and the difference between the camera's mod and the enemy's relative mod
                        enemy.PosX -= Math.Abs(Math.Abs(camera.xMod) - Math.Abs(enemy.relativeCamX));
                        enemy.relativeCamX = camera.xMod;
                    }
                }
                //The following cases are very similar to the first, just with different instructions for different camera directions
                else if (camera.direction == "left")
                {
                    if (enemy.justSpawned)
                    {
                        enemy.PosX -= camera.screenWidth;
                        enemy.justSpawned = false;
                    }

                    else
                    {
                        enemy.PosX += Math.Abs(Math.Abs(camera.xMod) - Math.Abs(enemy.relativeCamX));
                        enemy.relativeCamX = camera.xMod;
                    }
                }
                else if (camera.direction == "up")
                {
                    if (enemy.justSpawned)
                    {
                        enemy.PosY -= camera.screenHeight;
                        enemy.justSpawned = false;
                    }
                    else
                    {
                        enemy.PosY += Math.Abs(Math.Abs(camera.yMod) - Math.Abs(enemy.relativeCamY));
                        enemy.relativeCamY = camera.yMod;
                    }
                }
                else if (camera.direction == "down")
                {
                    if (enemy.justSpawned)
                    {
                        enemy.PosY += camera.screenHeight;
                        enemy.justSpawned = false;
                    }
                    else
                    {
                        enemy.PosY -= Math.Abs(Math.Abs(camera.yMod) - Math.Abs(enemy.relativeCamY));
                        enemy.relativeCamY = camera.yMod;
                    }
                }

                //If the camera isn't moving
                if (!camera.isMoving)
                {
                    //If the enemy somehow still has the just spawned tag (usually the case with the starting room)
                    if (enemy.justSpawned)
                    {
                        //Set the enemy's just spawned tag to be false
                        enemy.justSpawned = false;
                    }
                }

                //If the enemy's current health equals zero, they must be dead
                if (enemy.CurrHealth == 0)
                {
                    //Prepare the enemy to be removed outside of the loop
                    enemiesToRemove.Add(enemy);
                }

                //If the enemy has not just spawned, they may be drawn on screen
                if (!enemy.justSpawned)
                {
                    enemy.Draw(spriteBatch, 140, 140);
                }
            }
            
            //Loop through the enemies that need to be removed
            foreach(Enemy enemy in enemiesToRemove)
            {
                //Remove the enemy from the room's enemy list
                enemyList.Remove(enemy);

                //Call the enemy's on death method
                enemy.OnDeath(enemy.player);
            }
            
            //Clear the list that manages enemies that need to be removed (because they were just removed)
            enemiesToRemove.Clear();

            //If the current room is a boss room
            if (boss)
            {
                //Ask the atmosphere class to create a boss room filter
                Atmosphere.BossFilter(spriteBatch, xCoord, yCoord);
            }

            
        }
        //This method handles whether or not the camera should be moved to an adjacent room, as well as locking, unlocking, and updating interactables
        public String UpdateEvents(Player player, Camera camera, String playerMove, GameTime gameTime)
        {
            //Loop through every interactable in the room
            foreach (Interactable interactable in interactables)
            {
                //If the interactable can be activated (is interactable)
                if (interactable.Activatable)
                {
                    //Call the interactable's interact method and pass in the player and the camera
                    interactable.Interact(player, camera);

                    //If the interactable is calling for a new level, adjust the current room's flag to be true (pass it up the chain)
                    if (interactable.LevelTrigger)
                    {
                        this.levelTrigger = true;
                    }
                }
                //If the interactable is passable (no collision checks)
                if (interactable.Passable)
                {
                    //Exit the loop
                    continue;
                }
                //If the interactable is not passable
                else
                {
                    //Handle collisions betweenthe player and the interactable
                    interactable.HandleCollisions(player, camera);
                }


            }

            //If the room is currently going through the process of locking
            if (isLocking)
            {
                //Go through the tiles marked to hold doors
                foreach (Tile door in doorTiles)
                {
                    //Depending on which direction the door should be animating
                    switch (door.DoorLocal)
                    {
                        case 1:

                            //Update the position of the door based on the direction
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
                    //The room is not locking anymore
                    isLocking = false;
                }
            }

            //If the room is going through the process of unlocking
            if (isUnlocking)
            {
                //Just like with the locking process, examine all of the tiles marked to hold doors
                foreach (Tile door in doorTiles)
                {
                    //Update the animation of the door based on the direction it should be traveling
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
                    //Clear the door tiles
                    doorTiles.Clear();
                    
                    //Declare that the room is done unlocking, and is not locked
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

        //This method handles the locking of a room
        public void Lock()
        {
            //If the room is locked, is locking, or is unlocking do not allow a redundant / illegal call
            if (locked || isLocking || isUnlocking)
            {
                return;
            }

            //Make sure the room knows it is now in a locking state
            isLocking = true;
            locked = true;

            //For each door local contained within the room
            if (doorLocals.Contains("UP"))
            {
                //Each door requires the use of three tiles
                //Set the door at the specified tile and correctly update the door local
                finalRoomAnnex[0, 7].Door = new Tile(new Rectangle(14 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[0, 7].RelativeLocation.X, finalRoomAnnex[0, 7].RelativeLocation.Y - TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[0, 7].DoorLocal = 2;

                finalRoomAnnex[0, 6].Door = new Tile(new Rectangle(11 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[0, 6].RelativeLocation.X, finalRoomAnnex[0, 6].RelativeLocation.Y - TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[0, 6].DoorLocal = 2;

                finalRoomAnnex[0, 8].Door = new Tile(new Rectangle(12 * TILESIZE, 1 * TILESIZE, TILESIZE, TILESIZE), new Rectangle(finalRoomAnnex[0, 8].RelativeLocation.X, finalRoomAnnex[0, 8].RelativeLocation.Y - TILESIZE, TILESIZE, TILESIZE));
                finalRoomAnnex[0, 8].DoorLocal = 2;

                //Add the door tiles to the room's list of door tiles
                doorTiles.Add(finalRoomAnnex[0, 7]);
                doorTiles.Add(finalRoomAnnex[0, 6]);
                doorTiles.Add(finalRoomAnnex[0, 8]);

            }
            //Do the same for each door local (up to four)
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

        //This method will call the room's private unlock method if the conditions are met
        public Boolean RequestUnlock(Player player, Camera camera)
        {
            //Put room clearing code here... e.g. have all of the enemies been killed yet?
            
            //If there are no enemies in the room 
            if(enemyList.Count == 0)
            {
                //Call the room's private unlock method
                return Unlock(player, camera);
            }
            return false;
            
        }
        //This method handles actually unlocking the room
        private Boolean Unlock(Player player, Camera camera)
        {
            //If the room is currently locking, unlocking, or is unlocked, ignore redundant / illegal calls
            if(isLocking || !locked || isUnlocking)
            {
                return false;
            }
            //The room is now unlocking
            isUnlocking = true;

            //If the current room is the boss room
            if (boss)
            {
                //Create two new alerts, one to let the player know they have beaten the boss, and the other to tell them they have unlocked the next level
                Notification.Alert("Boss cleared!", Color.Purple, 60, false);
                Notification.Alert("Next Level Unlocked!", Color.OrangeRed, 60, true);

                //If the player is on the left half of the screen, spawn a ladder on the right side of the room
                if(player.PosX <= camera.screenWidth / 2)
                {
                    Rectangle ladderImage = new Rectangle(3 * TILESIZE, 5 * TILESIZE, TILESIZE, TILESIZE);
                    finalRoomAnnex[2, 13].Interactable = new Interactable("Ladder", finalRoomAnnex[2, 13].RelativeLocation, ladderImage, true, true, true);
                    interactables.Add(finalRoomAnnex[2, 13].Interactable);
                }
                //If the player is on the right half of the screen, spawn a ladder on the left side of the room
                else
                {
                    Rectangle ladderImage = new Rectangle(3 * TILESIZE, 5 * TILESIZE, TILESIZE, TILESIZE);
                    finalRoomAnnex[2, 2].Interactable = new Interactable("Ladder", finalRoomAnnex[2, 2].RelativeLocation, ladderImage, true, true, true);
                    interactables.Add(finalRoomAnnex[2, 2].Interactable);
                }
            }
            //If the current room is not the boss room
            else
            {
                //Let the player know they have cleared the room (by creating a new alert)
                Notification.Alert("Room Cleared!", Color.Green, 60, false);
            }

            return true;
            
        }

        //This method handles spawning a goblin in the current room
        public void SpawnGoblin(Player play, Texture2D tex, Camera camera)
        {
            //Select a random tile and remove it from the room's list
            Random randy = new Random();
            Tile spawn = spawnTiles[randy.Next(0, spawnTiles.Count)];
            spawnTiles.Remove(spawn);

            //Calculate the goblin's relative location
            int x = spawn.RelativeLocation.X;
            int y = spawn.RelativeLocation.Y;
            x = ((x %= camera.screenWidth) < 0) ? x + camera.screenWidth : x;
            y = ((y %= camera.screenHeight) < 0) ? y + camera.screenHeight : y;

            //Create a new goblin from the given information
            Goblin goblin = new Goblin(play, camera, x, y, 120, 120, tex);

            //The goblin has just spawned
            goblin.justSpawned = true;

            //Update the goblin with its initial direction
            goblin.UpdatePathDirection(spawn.Interactable.SubType);

            //Add the goblin to the enemy list
            enemyList.Add(goblin);
                 
        }

        //This method handles spawning a ghoul in the current room (refer to the above SpawnGoblin method to see how it works)
        public void SpawnGhoul(Player play, Texture2D tex, Camera camera)
        {
            Random randy = new Random();
            Tile spawn = spawnTiles[randy.Next(0, spawnTiles.Count)];
            int x = spawn.RelativeLocation.X;
            int y = spawn.RelativeLocation.Y;
            x = ((x %= camera.screenWidth) < 0) ? x + camera.screenWidth : x;
            y = ((y %= camera.screenHeight) < 0) ? y + camera.screenHeight : y;
            Ghoul ghoul = new Ghoul(play, camera, 0, x, y, 120, 120, tex);
            ghoul.UpdatePathDirection(spawn.Interactable.SubType);
            enemyList.Add(ghoul);
        }

        //This method handles spawning a kobold in the current room (refer to the above SpawnGoblin method to see how it works)
        public void SpawnKobold(Player play, Texture2D tex, Camera camera)
        {

            Random randy = new Random();
            Tile spawn = spawnTiles[randy.Next(0, spawnTiles.Count)];
            spawnTiles.Remove(spawn);
            int x = spawn.RelativeLocation.X;
            int y = spawn.RelativeLocation.Y;
            x = ((x %= camera.screenWidth) < 0) ? x + camera.screenWidth : x;
            y = ((y %= camera.screenHeight) < 0) ? y + camera.screenHeight : y;
            Kobold kobold = new Kobold(play, camera, x, y, 120, 120, tex);
            kobold.UpdatePathDirection(spawn.Interactable.SubType);
            enemyList.Add(kobold);
            
        }

        public void SpawnUlmog(Player play, Texture2D tex, Camera camera)
        {
            int x = camera.xMod;
            int y = camera.yMod;
            x = ((x %= camera.screenWidth) < 0) ? x + camera.screenWidth : x;
            y = ((y %= camera.screenHeight) < 0) ? y + camera.screenHeight : y;
            Ulmog ulmog = new Ulmog(play, camera, x, y, 120, 120, tex);
            enemyList.Add(ulmog);
            ulmog.IsHostile = true;
        }
    }
}
