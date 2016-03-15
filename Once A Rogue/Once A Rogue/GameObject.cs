using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Once_A_Rogue
{
    class GameObject
    {
        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        //Rectangle for the characters position
        private Rectangle posRect;

        public Rectangle PosRect
        {
            get { return posRect; }
            set { posRect = value; }
        }

        public int PosX
        {
            get { return posRect.X; }
            set { posRect.X = value; }
        }

        public int PosY
        {
            get { return posRect.Y; }
            set { posRect.Y = value; }
        }
    }
}
