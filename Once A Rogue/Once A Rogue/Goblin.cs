using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Goblin : Enemy
    {
        public Goblin(Player play, Camera camera, int x, int y, int width, int height, Texture2D tex) : base(tex, play, camera, x, y, width, height)
        {
            Random randy = new Random();   
            Level = randy.Next(-2, 2) + play.Level;
            ArmorLevel = 2 * Level;
            FearLevelTotal = 2 * Level;
            FearLevel = 0;
            MoveSpeedTotal = 5;
            MoveSpeed = MoveSpeedTotal;
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
        }

        //Default ranged attack for goblin
        public int RockToss()
        {
            int cooldown = 2000;
            int rangeX = 8;
            int rangeY = 8;

            if (cooldown == 0)
            {
                Vector2 rock = new Vector2(rangeX, rangeY);
                //Add code to see if it hits target
                return 10;

            }
            else
            {
                return 0;
            }
        }
        public void Update(Player play, GameTime gt)
        {
            base.Update();

            if (PosX > play.PosX + play.PosRect.Width + 40)
            {
                PosX -= MoveSpeed;
            }
            if (PosY > play.PosY + play.PosRect.Height + 40)
            {
                PosY -= MoveSpeed;
            }
            if (PosX < play.PosX + play.PosRect.Width + 40)
            {
                PosX += MoveSpeed;
            }
            if (PosY < play.PosY + play.PosRect.Height + 40)
            {
                PosY = MoveSpeed;
            }
        }



    }
}
