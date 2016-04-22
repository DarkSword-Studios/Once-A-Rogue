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

            SkillList = new List<Skills>();
            SkillList.Add(new Fireball());
            
            Level = randy.Next(-2, 2) + play.Level;
            ArmorLevel = 2 + Level * 2;
            FearLevelTotal = 2 + Level;
            FearLevel = 0;
            MoveSpeedTotal = 5;
            MoveSpeed = MoveSpeedTotal;
            TotalHealth = 25 + Level * 5;
            CurrHealth = TotalHealth;
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
        }

        //Default ranged attack for goblin
      
        public void Update(Player play, GameTime gt)
        {
            base.Update();

        }



    }
}
