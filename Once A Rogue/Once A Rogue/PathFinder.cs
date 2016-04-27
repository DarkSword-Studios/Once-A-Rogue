using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

//Implemented by: Ian Moon & Stasha Blank
//Team: DarkSword Studios
//Purpose: Declares how the environment handles an interactable
//Date Modified: 4/27/16

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
            List<PathFinderNode> neighbors = new List<PathFinderNode>();

            //Loop through every item in the room annex to deal with assigning tiles
            PathFinderNode[,] nodes = new PathFinderNode[9, 16];

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    if(i == 0 || i == 8 || j == 0 || j == 15)
                    {
                        continue;
                    }
                    if ((room.finalRoomAnnex[i, j].Interactable == null || room.finalRoomAnnex[i, j].Interactable.Passable))
                    {
                        nodes[i, j] = new PathFinderNode(i, j);
                    }
                        
                }
            }

            PathFinderNode newNode = nodes[startYCoord, startXCoord];
            PathFinderNode endNode = nodes[finishYCoord, finishXCoord];

            open.Add(newNode);

            PathFinderNode currentNode;

            while (open.Count > 0)
            {
                currentNode = open[0];
                for (int i = 1; i < open.Count; i++)
                {
                    if (open[i].GetFCost() < currentNode.GetFCost() || open[i].GetFCost() == currentNode.GetFCost() && open[i].heuristic < currentNode.heuristic)
                    {
                        currentNode = open[i];
                    }
                }

                open.Remove(currentNode);
                closed.Add(currentNode);

                if(currentNode == endNode)
                {
                    return RetracePath(newNode, endNode);
                }

                if(nodes[currentNode.x + 1, currentNode.y] != null)
                {
                    neighbors.Add(nodes[currentNode.x + 1, currentNode.y]);
                }
                if (nodes[currentNode.x - 1, currentNode.y] != null)
                {
                    neighbors.Add(nodes[currentNode.x - 1, currentNode.y]);
                }
                if (nodes[currentNode.x, currentNode.y + 1] != null)
                {
                    neighbors.Add(nodes[currentNode.x, currentNode.y + 1]);
                }
                if (nodes[currentNode.x, currentNode.y - 1] != null)
                {
                    neighbors.Add(nodes[currentNode.x, currentNode.y - 1]);
                }

                foreach(PathFinderNode neighbor in neighbors)
                {
                    if (closed.Contains(neighbor))
                    {
                        continue;
                    }

                    int newDistance = currentNode.movementCost + GetDistance(currentNode, neighbor);
                    if (newDistance < neighbor.movementCost || !open.Contains(neighbor))
                    {
                        neighbor.movementCost = newDistance;
                        neighbor.heuristic = GetDistance(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if (!open.Contains(neighbor))
                        {
                            open.Add(neighbor);
                        }
                    }
                }


            }

            return closed;
        }

        private static int GetDistance(PathFinderNode nodeA, PathFinderNode nodeB)
        {
            int distX = Math.Abs(nodeA.x - nodeB.x);
            int distY = Math.Abs(nodeA.y - nodeB.y);

            return 10 * (distX + distY);
        }

        private static List<PathFinderNode> RetracePath(PathFinderNode startNode, PathFinderNode endNode)
        {
            List<PathFinderNode> path = new List<PathFinderNode>();
            PathFinderNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }

    }
}
