using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; //Needed for bringing in files

namespace Once_A_Rogue

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Generates Level Structure
//Date Modified: 3/17/16

{
    class LevelBuilder
    {
        public LevelBuilder()
        {
            //Init
        }

        public string[,] BuildLevel(string[,] grid, int numRooms)
        {
            //Monitors room usage to avoid overwrites + to aid garbage collection
            Dictionary<string, Boolean> roomUsed = new Dictionary<string, bool>();

            //Center of grid
            int centerX = (int)grid.GetLength(0) / 2;
            int centerY = (int)grid.GetLength(1) / 2;

            //Populate the center of the grid with a four way room
            grid[centerX, centerY] = "allDirections";

            //Convert starting coords to string
            string starterRoom = centerX.ToString();
            starterRoom += centerY;

            //Add first room to complete dictionary
            roomUsed[starterRoom] = true;

            //Keeps track of how many "important" (algorithmically) rooms have been generated
            int roomCount = 1;

            //Random object to handle arbitrary pulls
            Random random = new Random();

            //Arrays of assortments of rooms for each situation (multi w/ guranteed direction : single direction)
            string[] possibleRoomsMultiLeft = new string[] {"allDirections", "leftDown", "leftRight", "leftUp",
            "leftUpRight", "leftUpDown", "leftRightDown"};

            string[] possibleRoomsMultiRight = new string[] {"allDirections", "leftRight", "upRight",
            "rightDown", "leftUpRight", "leftRightDown", "upRightDown"};

            string[] possibleRoomsMultiDown = new string[] {"allDirections", "leftDown",
            "upDown", "rightDown", "leftUpDown", "leftRightDown", "upRightDown"};

            string[] possibleRoomsMultiUp = new string[] {"allDirections", "leftUp", "upRight",
            "upDown", "leftUpRight", "leftUpDown", "upRightDown"};

            string[] possibleRoomsSingle = new string[] { "left", "up", "right", "down" };

            //Grand master queue, praise this mighty control structure (Holds and dispenses unfulfilled nodes)
            Queue<string> nodeQueue = new Queue<string>();

            //MANUALLY Prepopulate Queue with correct nodes

            //1st digit: (MUST HAVE CONNECTOR OF TYPE;) 1 = left connector, 2 = up connector, 3 = right connector, 4 = down connector
            //2nd digit: x coord
            //3rd digit: y coord

            string temp = "2";
            temp += centerX;
            temp += centerY + 1;

            nodeQueue.Enqueue(temp);

            temp = "4";
            temp += centerX;
            temp += centerY - 1;

            nodeQueue.Enqueue(temp);

            temp = "1";
            temp += centerX + 1;
            temp += centerY;

            nodeQueue.Enqueue(temp);

            temp = "3";
            temp += centerX - 1;
            temp += centerY;

            nodeQueue.Enqueue(temp);

            //RUN UNTIL NODE QUEUE IS EMPTY OR THERE WILL BE SEVERE CONSEQUENCES
            while (nodeQueue.Count != 0)
            {
                //Ask the queue for a node
                string node = nodeQueue.Dequeue();

                //Start the room as unknown
                string selectedRoom = "";

                //This condition runs true if the room builder hasn't met the quota, and the current node has not been fulfilled
                if (roomCount <= numRooms && !roomUsed.ContainsKey(node.Substring(1)))
                {
                    //Examine the first digit to see which connection is a must-have, and select a room from the proper array
                    switch (node[0])
                    {
                        case ('1'):
                            selectedRoom = possibleRoomsMultiLeft[random.Next(0, possibleRoomsMultiLeft.Length)];
                            break;

                        case ('2'):
                            selectedRoom = possibleRoomsMultiUp[random.Next(0, possibleRoomsMultiUp.Length)];
                            break;

                        case ('3'):
                            selectedRoom = possibleRoomsMultiRight[random.Next(0, possibleRoomsMultiRight.Length)];
                            break;

                        case ('4'):
                            selectedRoom = possibleRoomsMultiDown[random.Next(0, possibleRoomsMultiDown.Length)];
                            break;
                    }

                    //Convert digits 2 and 3 into x and y coordinates
                    int xCoord = int.Parse(node[1].ToString());
                    int yCoord = int.Parse(node[2].ToString());

                    //give the correct node the selected room
                    grid[xCoord, yCoord] = selectedRoom;

                    //Build room num
                    string roomNum = xCoord.ToString();
                    roomNum += yCoord.ToString();

                    //Mark the node as fulfilled
                    roomUsed[roomNum] = true;

                    //Convert room to upper (avoid case detection errors)
                    selectedRoom = selectedRoom.ToUpper();

                    //We have a new room! Keep track of it
                    roomCount += 1;

                    //If the selected room contains a XXXX connection, Mark the node it points to
                    if (selectedRoom.Contains("LEFT") || selectedRoom == "ALLDIRECTIONS")
                    {
                        //Watch out for grid bounds!
                        if (xCoord != 1)
                        {
                            //Supply adjacent node with a marker to be handled by queue
                            roomNum = "3";
                            roomNum += (xCoord - 1).ToString();
                            roomNum += yCoord.ToString();
                            nodeQueue.Enqueue(roomNum);
                        }
                        //Grid bounds have been reached, cut off branch but placing end node
                        else
                        {
                            grid[0, yCoord] = "right";
                        }
                    }
                    if (selectedRoom.Contains("UP") || selectedRoom == "ALLDIRECTIONS")
                    {
                        if (yCoord != 1)
                        {
                            roomNum = "4";
                            roomNum += xCoord.ToString();
                            roomNum += (yCoord - 1).ToString();
                            nodeQueue.Enqueue(roomNum);
                        }
                        else
                        {
                            grid[xCoord, 0] = "down";
                        }
                    }
                    if (selectedRoom.Contains("RIGHT") || selectedRoom == "ALLDIRECTIONS")
                    {
                        if (xCoord < grid.GetLength(0) - 2)
                        {
                            roomNum = "1";
                            roomNum += (xCoord + 1).ToString();
                            roomNum += yCoord.ToString();
                            nodeQueue.Enqueue(roomNum);
                        }
                        else
                        {
                            grid[grid.GetLength(0) - 1, yCoord] = "left";
                        }

                    }
                    if (selectedRoom.Contains("DOWN") || selectedRoom == "ALLDIRECTIONS")
                    {
                        if (yCoord < grid.GetLength(1) - 2)
                        {
                            roomNum = "2";
                            roomNum += xCoord.ToString();
                            roomNum += (yCoord + 1).ToString();
                            nodeQueue.Enqueue(roomNum);
                        }
                        else
                        {
                            grid[xCoord, grid.GetLength(1) - 1] = "up";
                        }
                    }
                }
                //This condition runs true if the quota of rooms HAS been met and the node is unfulfilled
                else if (roomCount > numRooms && !roomUsed.ContainsKey(node.Substring(1)))
                {
                    //Once we've reached the room quota, it's time to end all open brances with end nodes
                    switch (node[0])
                    {
                        case ('1'):
                            selectedRoom = "left";
                            break;

                        case ('2'):
                            selectedRoom = "up";
                            break;

                        case ('3'):
                            selectedRoom = "right";
                            break;

                        case ('4'):
                            selectedRoom = "down";
                            break;
                    }

                    //Record the node, and mark it as fulfilled
                    int xCoord = int.Parse(node[1].ToString());
                    int yCoord = int.Parse(node[2].ToString());

                    grid[xCoord, yCoord] = selectedRoom;

                    string roomNum = xCoord.ToString();
                    roomNum += yCoord.ToString();

                    roomUsed[roomNum] = true;

                    roomCount += 1;
                }

                //Garbage collection (how to deal with conflicting nodes / queue mistakes!!!! IMPORTANT***)
                //This condition runs true if the current node HAS BEEN FULFILLED
                //Unfortunately if this is the case we have TWO OR MORE ASKING NODES, which results in a broken connection 50% of the time
                else if (roomUsed.ContainsKey(node.Substring(1)))
                {
                    int xCoord = int.Parse(node[1].ToString());
                    int yCoord = int.Parse(node[2].ToString());

                    //Format the all directions room for direction splicing
                    if (grid[xCoord, yCoord] == "allDirections")
                    {
                        grid[xCoord, yCoord] = "leftUpRightDown";
                    }

                    //Examine the asking node's origin (first come first served, second asking node is out of luck)
                    switch (node[0])
                    {
                        case ('1'):
                            //selectedRoom = "left";
                            //If the current node contains XXXX direction
                            if (!grid[xCoord, yCoord].ToUpper().Contains("LEFT"))
                            {
                                //Examine the asking node to see if it's null
                                string room = grid[xCoord - 1, yCoord];
                                if (room != null)
                                {
                                    //Format the all directions room for direction splicing
                                    if (grid[xCoord - 1, yCoord] == "allDirections")
                                    {
                                        grid[xCoord - 1, yCoord] = "leftUpRightDown";
                                    }
                                    //Connector Failure; Connection Denied (Remove asking node's -X-X-X-X vector)
                                    grid[xCoord - 1, yCoord] = grid[xCoord - 1, yCoord].Replace("right", "");
                                    grid[xCoord - 1, yCoord] = grid[xCoord - 1, yCoord].Replace("Right", "");
                                }

                            }
                            //Else - Connection approved; discrepency cleared
                            break;

                        case ('2'):
                            //selectedRoom = "up";
                            if (!grid[xCoord, yCoord].ToUpper().Contains("UP"))
                            {
                                string room = grid[xCoord, yCoord - 1];
                                if (room != null)
                                {
                                    if (grid[xCoord, yCoord - 1] == "allDirections")
                                    {
                                        grid[xCoord, yCoord - 1] = "leftUpRightDown";
                                    }
                                    //Connector Failure; Connection Denied (Remove asking node's vector)
                                    grid[xCoord, yCoord - 1] = grid[xCoord, yCoord - 1].Replace("down", "");
                                    grid[xCoord, yCoord - 1] = grid[xCoord, yCoord - 1].Replace("Down", "");
                                }

                            }
                            //Else - Connection approved ; node removed
                            break;

                        case ('3'):
                            //selectedRoom = "right";
                            if (!grid[xCoord, yCoord].ToUpper().Contains("RIGHT"))
                            {
                                string room = grid[xCoord + 1, yCoord];
                                if (room != null)
                                {
                                    if (grid[xCoord + 1, yCoord] == "allDirections")
                                    {
                                        grid[xCoord + 1, yCoord] = "leftUpRightDown";
                                    }
                                    //Connector Failure; Connection Denied (Remove asking node's vector)
                                    grid[xCoord + 1, yCoord] = grid[xCoord + 1, yCoord].Replace("left", "");
                                    grid[xCoord + 1, yCoord] = grid[xCoord + 1, yCoord].Replace("Left", "");
                                }

                            }
                            //Else - Connection approved ; node removed
                            break;

                        case ('4'):
                            //selectedRoom = "down";
                            if (!grid[xCoord, yCoord].ToUpper().Contains("DOWN"))
                            {
                                string room = grid[xCoord, yCoord + 1];
                                if (room != null)
                                {
                                    if (grid[xCoord, yCoord + 1] == "allDirections")
                                    {
                                        grid[xCoord, yCoord + 1] = "leftUpRightDown";
                                    }
                                    //Connector Failure; Connection Denied (Remove asking node's vector)
                                    grid[xCoord, yCoord + 1] = grid[xCoord, yCoord + 1].Replace("up", "");
                                    grid[xCoord, yCoord + 1] = grid[xCoord, yCoord + 1].Replace("Up", "");
                                }

                            }
                            //Else - Connection approved ; node removed
                            break;
                    }
                    if (grid[xCoord, yCoord] == "leftUpRightDown")
                    {
                        grid[xCoord, yCoord] = "allDirections";
                    }
                }

            }

            return grid;
        }

        public Room[,] BuildRoom(string[,] gridSystem, Room[,] levelAnnex, List<Room> bossRooms, Camera camera, int rowIndex, int columnIndex)
        {

            //If the grid node is NOT EMPTY it MUST BE FULFILLED
            if (gridSystem[columnIndex, rowIndex] != null)
            {
                //Build coordinates based on camera mods and grid placement
                int xCoord = ((camera.screenWidth / 2) + camera.xMod);
                int yCoord = ((camera.screenHeight / 2) + camera.yMod);

                xCoord += ((columnIndex - 4) * 1920);
                yCoord += ((rowIndex - 4) * 1080);

                //Based on the node's structure, use the appropriate room code
                int roomCode;
                Boolean addToBossList = false;

                //Room codes are transformed from text to numbers in an effort to shorten file names
                //1 = has left connector; 2 = has up connector; 3 = has right connector; 4 = has down connector
                switch (gridSystem[columnIndex, rowIndex].ToUpper())
                {
                    case ("ALLDIRECTIONS"):

                        roomCode = 1234;
                        break;

                    case ("LEFT"):

                        roomCode = 1;
                        addToBossList = true;
                        break;

                    case ("UP"):

                        roomCode = 2;
                        addToBossList = true;
                        break;

                    case ("RIGHT"):

                        roomCode = 3;
                        addToBossList = true;
                        break;

                    case ("DOWN"):

                        roomCode = 4;
                        addToBossList = true;
                        break;

                    case ("LEFTDOWN"):

                        roomCode = 14;
                        break;

                    case ("LEFTRIGHT"):

                        roomCode = 13;
                        break;

                    case ("LEFTUP"):

                        roomCode = 12;
                        break;

                    case ("UPRIGHT"):

                        roomCode = 23;
                        break;

                    case ("UPDOWN"):

                        roomCode = 24;
                        break;

                    case ("RIGHTDOWN"):

                        roomCode = 34;
                        break;

                    case ("LEFTUPRIGHT"):

                        roomCode = 123;
                        break;

                    case ("LEFTUPDOWN"):

                        roomCode = 124;
                        break;

                    case ("LEFTRIGHTDOWN"):

                        roomCode = 134;
                        break;

                    case ("UPRIGHTDOWN"):

                        roomCode = 234;
                        break;

                    default:
                        roomCode = 2;
                        break;
                }

                string roomCodeStr = roomCode.ToString();

                //This list keeps track of all of the rooms in Content/Rooms/ that would be valid fits for the current room to build
                List<string> possibleRooms = new List<string>();

                //Read in each file in the Content/Rooms/ folder (where we keep the room.txt files)
                foreach (string file in Directory.GetFiles(@"..\..\..\Content\Rooms"))
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

                //Assuming this node in the level annex hasn't been fulfilled;
                if (levelAnnex[columnIndex, rowIndex] == null)
                {
                    //Create a new room from the file picked in the specific position within the level annex
                    Room room = new Room(roomPath, false, gridSystem[columnIndex, rowIndex]);

                    //Build the level to take care of necessary initialization
                    room.BuildRoom(xCoord, yCoord);

                    //This is where we take potential boss room candidates and flag them for processing
                    if (addToBossList)
                    {
                        bossRooms.Add(room);
                    }

                    //Add the room to the level annex after it has been built
                    levelAnnex[columnIndex, rowIndex] = room;

                    //If the generated room is the starting room, it should be initially active (we need a place to start, every other room's activity can be determined algorithmically)
                    if (columnIndex == ((int)levelAnnex.GetLength(0) / 2) && rowIndex == ((int)levelAnnex.GetLength(1) / 2))
                    {
                        levelAnnex[columnIndex, rowIndex].Active = true;                        
                    }
                }


            }
            //Lastly, return the level annex so that changes can be saved
            return levelAnnex;

        }
    }
}
