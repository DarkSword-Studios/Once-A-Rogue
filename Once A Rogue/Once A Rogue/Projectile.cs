using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    public class Projectile:GameObject
    //Ian Moon
    //3/9/2016
    //This class is for any projectile that needs to be produced
    {
        //Attributes
        Vector2 projPos;
        Vector2 vector;
        MouseState ms;
        int duration;
        int rowY;
        int numFrames;
        int currentFrame;
        const int FRAMEWIDTH = 40;
        const int FRAMEHEIGHT = 40;

        //Parameterized Constructor
        public Projectile(Vector2 vec, int rowY, int numFrames, int height, int width, int x, int y)
        {
            projPos = new Vector2(x, y);
            vector = vec;
            this.rowY = rowY;
            this.numFrames = numFrames;
            PosRect = new Rectangle(x, y, width, height);

            //Adding the projectile to a projectile list in the game class
            Game1.CurrProjectiles.Add(this);
        }

        //Overload Constructor
        public Projectile(Vector2 vec, MouseState mouseState, int rowY, int numFrames, int height, int width, int x, int y)
        {
            projPos = new Vector2(x, y);
            vector = vec;
            ms = mouseState;
            this.rowY = rowY;
            this.numFrames = numFrames;
            PosRect = new Rectangle(x, y, width, height);

            //Adding the projectile to a projectile list in the game class
            Game1.CurrProjectiles.Add(this);
        }

        public void Update()
        {
            projPos += vector * 10;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            //spriteBatch.Draw(Texture, PosRect, Color.White);

            Rectangle frame = new Rectangle(currentFrame * FRAMEWIDTH, FRAMEHEIGHT * rowY, FRAMEWIDTH, FRAMEHEIGHT);
            spriteBatch.Draw(texture, projPos, frame, Color.White);
        }
    }
}
