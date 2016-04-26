using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Kobold : Enemy
    {



        public Kobold(Player play, Camera camera, int x, int y, int width, int height, Texture2D tex) : base(tex, play, camera, x, y, width, height)
        {
            Random randy = new Random();
            Level = randy.Next(-2, 2) + play.Level;
            ArmorLevel = 3 + Level / 2;
            FearLevelTotal = 3 + Level;
            FearLevel = 0;
            TotalHealth = 20 + Level * 3;
            CurrHealth = TotalHealth;
        }
        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
        }

        
        public void PreyOnWeakness(Player pay)
        {
            if (pay.CurrHealth <= pay.TotalHealth / 5)
            {
                ArmorLevel = ArmorLevel * 2;
                SnareResist = SnareResist * 2;
                StunResist = StunResist * 2;
                FireResist = FireResist * 2;
                isFeared = false;
                isSnared = false;
                isStunned = false;

            }
        }
        public void Frenzied(Player pay)
        {
            if (CurrHealth == TotalHealth / 6)
            {
                isStunned = false;
                isSnared = false;
                isFeared = false;
                isOnFire = false;
                isPoisoned = false;
                isExplosive = false;
            }
        }
        public void Update(Player play, GameTime gt)
        {
            base.Update(gt);
        }
    }
}

