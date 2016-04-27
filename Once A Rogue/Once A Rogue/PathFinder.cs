using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Ian Moon & Stasha Blank
//Team: DarkSword Studios
//Purpose: Declares how the environment handles an interactable
//Date Modified: 4/20/16

namespace Once_A_Rogue
{
    class PathFinder
    {
        public static List<PathFinderNode> FindPath(Room room, Camera camera, GameObject start, GameObject finish)
        {
            int startXCoord = start.PosX;
            int startYCoord = start.PosY;
            startXCoord = ((startXCoord %= camera.screenWidth) < 0) ? startXCoord + camera.screenWidth : startXCoord;
            startYCoord = ((startYCoord %= camera.screenHeight) < 0) ? startYCoord + camera.screenHeight : startYCoord;

            startXCoord /= 120;
            startYCoord /= 120;

            int finishXCoord = finish.PosX + finish.PosRect.Width / 2;
            int finishYCoord = finish.PosY + finish.PosRect.Height / 2;
            finishXCoord = ((finishXCoord %= camera.screenWidth) < 0) ? finishXCoord + camera.screenWidth : finishXCoord;
            finishYCoord = ((finishYCoord %= camera.screenHeight) < 0) ? finishYCoord + camera.screenHeight : finishYCoord;

            finishXCoord = finishXCoord / 120;
            finishYCoord = finishYCoord / 120;


            List<PathFinderNode> open = new List<PathFinderNode>();
            List<PathFinderNode> closed = new List<PathFinderNode>();
            List<PathFinderNode> checkForOpen = new List<PathFinderNode>();

            //Loop through every item in the room annex to deal with assigning tiles

            PathFinderNode newNode = new PathFinderNode(null, (int) new Vector2(finishXCoord - startXCoord, finishYCoord - startYCoord).Length(), startXCoord, startYCoord);
            PathFinderNode endNode = new PathFinderNode(null, 0, finishXCoord, finishYCoord);

            open.Add(newNode);

            PathFinderNode currentNode = newNode;

            while(currentNode != endNode)
            {
                foreach (PathFinderNode node in open)
                {
                    if (node.heuristic < currentNode.heuristic)
                    {
                        currentNode = node;
                    }
                }

                open.Remove(currentNode);
                closed.Add(currentNode);

                if(currentNode == endNode)
                {
                    return closed;
                }

                if ((room.finalRoomAnnex[currentNode.y + 1, currentNode.x].Interactable == null || room.finalRoomAnnex[currentNode.y + 1, currentNode.x].Interactable.Passable) && currentNode.x + 1 != 15 && )
                {
                    Vector2 vector = new Vector2(finishXCoord - (currentNode.x + 1), finishYCoord - currentNode.y);
                    PathFinderNode node = new PathFinderNode(currentNode, (int)vector.Length(), currentNode.x + 1, currentNode.y);
                    if (!closed.Contains(node))
                    {
                        checkForOpen.Add(node);
                    }
                }

                if ((room.finalRoomAnnex[currentNode.y - 1, currentNode.x].Interactable == null || room.finalRoomAnnex[currentNode.y - 1, currentNode.x].Interactable.Passable) && currentNode.x - 1 != 0)
                {
                    Vector2 vector = new Vector2(finishXCoord - (currentNode.x - 1), finishYCoord - currentNode.y);
                    PathFinderNode node = new PathFinderNode(currentNode, (int)vector.Length(), currentNode.x - 1, currentNode.y);
                    if (!closed.Contains(node))
                    {
                        checkForOpen.Add(node);
                    }
                }

                if ((room.finalRoomAnnex[currentNode.y, currentNode.x + 1].Interactable == null || room.finalRoomAnnex[currentNode.y, currentNode.x + 1].Interactable.Passable) && currentNode.y + 1 != 8)
                {
                    Vector2 vector = new Vector2(finishXCoord - currentNode.x, finishYCoord - (currentNode.y + 1));
                    PathFinderNode node = new PathFinderNode(currentNode, (int)vector.Length(), currentNode.x, (currentNode.y + 1));
                    if (!closed.Contains(node))
                    {
                        checkForOpen.Add(node);
                    }
                }

                if ((room.finalRoomAnnex[currentNode.y, currentNode.x - 1].Interactable == null || room.finalRoomAnnex[currentNode.y, currentNode.x - 1].Interactable.Passable) && currentNode.y - 1 != 0)
                {
                    Vector2 vector = new Vector2(finishXCoord - currentNode.x, finishYCoord - (currentNode.y - 1));
                    PathFinderNode node = new PathFinderNode(currentNode, (int)vector.Length(), currentNode.x, currentNode.y - 1);
                    if (!closed.Contains(node))
                    {
                        checkForOpen.Add(node);
                    }
                }
                foreach(PathFinderNode node in checkForOpen)
                {
                    if (!open.Contains(node))
                    {
                        open.Add(node);
                    }
                }
                

                //checkForOpen.Clear();

                //return new PathFinderNode(null, 0, finishXCoord - currentNode.x, finishYCoord - currentNode.y);


            }
        }

    }
}
