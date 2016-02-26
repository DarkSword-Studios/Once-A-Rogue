using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Once_A_Rogue

//Implemented by: Stasha Blank
//Team: DarkSword Studios
//Purpose: Generates Level Structure
//Date Modified: 2/25/16

{
    class LevelBuilder
    {
        public LevelBuilder()
        {
            //Init
        }

        public string[,] BuildLevel(string[,] grid, int numRooms)
        {
            Dictionary<string, Boolean> roomUsed = new Dictionary<string, bool>();

            //Populate the center of the grid with a four way room
            grid[4, 4] = "allDirections";

            roomUsed["44"] = true;

            int roomCount = 1;

            Random random = new Random(); 

            string[] possibleRoomsMultiLeft = new string[] {"allDirections", "leftDown", "leftRight", "leftUp",
            "leftUpRight", "leftUpDown", "leftRightDown"};

            string[] possibleRoomsMultiRight = new string[] {"allDirections", "leftRight", "upRight",
            "rightDown", "leftUpRight", "leftRightDown", "upRightDown"};

            string[] possibleRoomsMultiDown = new string[] {"allDirections", "leftDown",
            "upDown", "rightDown", "leftUpDown", "leftRightDown", "upRightDown"};

            string[] possibleRoomsMultiUp = new string[] {"allDirections", "leftUp", "upRight",
            "upDown", "leftUpRight", "leftUpDown", "upRightDown"};

            string[] possibleRoomsSingle = new string[] {"left", "up", "right", "down"};

            Queue<string> nodeQueue = new Queue<string>();

            //Prepopulate Queue with correct nodes

            //1st digit: 1 = left connector, 2 = up connector, 3 = right connector, 4 = down connector
            //2nd digit: x coord
            //3rd digit: y coord

            nodeQueue.Enqueue("445");
            nodeQueue.Enqueue("243");
            nodeQueue.Enqueue("154");
            nodeQueue.Enqueue("334");

            while(nodeQueue.Count != 0)
            {
                string node = nodeQueue.Dequeue();

                string selectedRoom = "";

                if(roomCount <= numRooms && !roomUsed.ContainsKey(node.Substring(1)))
                {
                    switch (node[0])
                    {
                        case ('3'):
                            selectedRoom = possibleRoomsMultiLeft[random.Next(0, possibleRoomsMultiLeft.Length)];
                            break;

                        case ('4'):
                            selectedRoom = possibleRoomsMultiUp[random.Next(0, possibleRoomsMultiUp.Length)];
                            break;

                        case ('1'):
                            selectedRoom = possibleRoomsMultiRight[random.Next(0, possibleRoomsMultiRight.Length)];
                            break;

                        case ('2'):
                            selectedRoom = possibleRoomsMultiDown[random.Next(0, possibleRoomsMultiDown.Length)];
                            break;
                    }

                    int xCoord = int.Parse(node[1].ToString());
                    int yCoord = int.Parse(node[2].ToString());

                    //BAD CODE
                    if(xCoord == 9 || yCoord == 9)
                    {
                        xCoord = 8;
                        yCoord = 8;
                    }

                    grid[xCoord, yCoord] = selectedRoom;

                    string roomNum = xCoord.ToString();
                    roomNum += yCoord.ToString();

                    roomUsed[roomNum] = true;

                    selectedRoom = selectedRoom.ToUpper();

                    roomCount += 1;

                    if (selectedRoom.Contains("LEFT"))
                    {
                        roomNum = "3";
                        roomNum += (xCoord - 1).ToString();
                        roomNum += yCoord.ToString();
                        nodeQueue.Enqueue(roomNum);
                    }
                    if (selectedRoom.Contains("UP"))
                    {
                        roomNum = "4";
                        roomNum += xCoord.ToString();
                        roomNum += (yCoord - 1).ToString();
                        nodeQueue.Enqueue(roomNum);
                    }
                    if (selectedRoom.Contains("RIGHT"))
                    {
                        roomNum = "1";
                        roomNum += (xCoord + 1).ToString();
                        roomNum += yCoord.ToString();
                        nodeQueue.Enqueue(roomNum);
                    }
                    if (selectedRoom.Contains("DOWN"))
                    {
                        roomNum = "2";
                        roomNum += xCoord.ToString();
                        roomNum += (yCoord + 1).ToString();
                        nodeQueue.Enqueue(roomNum);
                    }
                }

                else if(roomCount > numRooms && !roomUsed.ContainsKey(node.Substring(1)))
                {
                    switch (node[0])
                    {
                        case ('3'):
                            selectedRoom = "left";
                            break;

                        case ('4'):
                            selectedRoom = "up";
                            break;

                        case ('1'):
                            selectedRoom = "right";
                            break;

                        case ('2'):
                            selectedRoom = "down";
                            break;
                    }

                    int xCoord = int.Parse(node[1].ToString());
                    int yCoord = int.Parse(node[2].ToString());

                    //BAD CODE
                    if (xCoord == 9 || yCoord == 9)
                    {
                        xCoord = 8;
                        yCoord = 8;
                    }

                    grid[xCoord, yCoord] = selectedRoom;

                    string roomNum = xCoord.ToString();
                    roomNum += yCoord.ToString();

                    roomUsed[roomNum] = true;

                    roomCount += 1;
                }
            }

            return grid;
        }   
    }
}
