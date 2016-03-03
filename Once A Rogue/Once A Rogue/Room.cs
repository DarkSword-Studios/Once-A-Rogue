using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; //Needed to read in room files
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Room
    {
        int[,] roomAnnex = new int[9,16];

        const int TILESIZE = 120;

        public Room(string file)
        {
            StreamReader reader = new StreamReader(file);

            string line = "";
            int row = 0;
            int col = 0;

            while((line = reader.ReadLine()) != null)
            {
                string[] tiles = line.Split(',');

                foreach(string tile in tiles)
                {
                    roomAnnex[row, col] = int.Parse(tile);
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

            while(row < roomAnnex.GetLength(0))
            {
                while(col < roomAnnex.GetLength(1))
                {
                    int tileY = roomAnnex[row, col] / 16;
                    int tileX = roomAnnex[row, col] % 16;

                    Rectangle tile = new Rectangle(tileX * TILESIZE, tileY * TILESIZE, TILESIZE, TILESIZE);
                    Vector2 location = new Vector2(xCoord + (120 * col), yCoord + (120 * row));

                    spriteBatch.Draw(tilemap, location, tile, Color.White);
                    col++;
                }
                col = 0;
                row++;
            }
        }
    }
}
