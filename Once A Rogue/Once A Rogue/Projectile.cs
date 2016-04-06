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
        int velocityX;
        int velocityY;
        int duration;
        int rowY;
        int numFrames;
        int currentFrame;
        const int FRAMEWIDTH = 40;
        const int FRAMEHEIGHT = 40;

        //Parameterized Constructor
        public Projectile(int vectorX, int vectorY, int rowY, int numFrames)
        {
            vectorX = velocityX;
            vectorY = velocityY;
            this.rowY = rowY;
            this.numFrames = numFrames;

            //Adding the projectile to a projectile list in the game class
            //game.CurrProjectiles.Add(this);
        }

        //Overload Constructor
        public Projectile(int vectorX, int vectorY, int dur, int rowY, int numFrames)
        {
            velocityX = vectorX;
            velocityY = vectorY;
            duration = dur;
            this.rowY = rowY;
            this.numFrames = numFrames;

            //Adding the projectile to a projectile list in the game class
            //game.CurrProjectiles.Add(this);
        }

        public void Update()
        {
            PosX = PosX + velocityX;

            PosY = PosY + velocityY;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            //spriteBatch.Draw(Texture, PosRect, Color.White);

            Rectangle frame = new Rectangle(currentFrame * FRAMEWIDTH, FRAMEHEIGHT * rowY, FRAMEWIDTH, FRAMEHEIGHT);
            spriteBatch.Draw(texture, PosRect, frame, Color.White);
        }
    }
}
