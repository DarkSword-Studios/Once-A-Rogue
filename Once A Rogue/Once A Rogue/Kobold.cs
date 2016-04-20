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



        public Kobold(Player play, int x, int y, int width, int height, Texture2D tex) : base(tex, play, x, y, width, height)
        {
            Random randy = new Random();
            Level = randy.Next(-2, 2) + play.Level;
            ArmorLevel = 3 * Level;
            FearLevelTotal = 3 * Level;
            FearLevel = 0;
        }
        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
        }

        public void CandleBoost(Player pay)
        {
            if (pay.CurrHealth == pay.TotalHealth)
            {
                FearLevel = FearLevel / 2;
                ArmorLevel = ArmorLevel * 2;
                MoveSpeed = MoveSpeed * 2;
            }
            if (pay.CurrHealth <= pay.TotalHealth / 2)
            {
                FearLevel = FearLevel * 2;
                ArmorLevel = ArmorLevel / 2;

            }
        }
        public void PreyOnWeakness(Player pay)
        {
            if (pay.CurrHealth <= pay.TotalHealth / 5)
            {
                ArmorLevel = ArmorLevel * 2;
                SnareResist = SnareResist * 2;
                StunResist = StunResist * 2;
                FireResist = FireResist * 2;
                IsFeared = false;
                IsSnared = false;
                IsStunned = false;

            }
        }
        public void Frenzied(Player pay)
        {
            if (CurrHealth == TotalHealth / 6)
            {
                IsStunned = false;
                IsSnared = false;
                IsFeared = false;
                IsOnFire = false;
                IsPoisoned = false;
                IsExplosive = false;
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

