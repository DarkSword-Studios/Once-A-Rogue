using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class PathFinderNode
    {
        public PathFinderNode parent;
        public int heuristic;
        public int x;
        public int y;
        public int movementCost;
        public int weight;

        public PathFinderNode(int x, int y)
        {
            this.x = x;
            this.y = y;
            //movementCost = (int)(new Vector2(this.x - parent.x, this.y - parent.y).Length() * 10);
            //weight = heuristic + movementCost;
        }

        public int GetFCost()
        {
            return heuristic + movementCost;
        }
    }
}
