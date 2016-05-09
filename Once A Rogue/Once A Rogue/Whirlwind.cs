using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Timers;

namespace Once_A_Rogue
{
    class Whirlwind:Skills
    {
        public Vector2 target;
        public Vector2 origVector;
        public int totalNumSpins;
        public int numSpins;
        public int totalRotations;
        public int rotations;

        public Whirlwind(int dam, Character own) : base(dam, own)
        {
            Owner = own;
            Damage = dam;
            BurstRadius = 0;
            Cost = 0;
            Name = "Whirlwind";
            if (Owner.Level <= 30)
            {
                totalNumSpins = 1;
            }
            else
            {
                totalNumSpins = 2;
            }

            numSpins = totalNumSpins;
            rotations = 0;
            Cooldown = 0;
            CooldownTotal = 5000 + (100 * totalRotations);
        }

        public override void OnActivated()
        {
            if(Owner is Enemy)
            {
                Enemy enemy = (Enemy)Owner;

                if ((Owner.CurrHealth / Owner.TotalHealth) <= .70)
                {
                    numSpins++;

                    if ((Owner.CurrHealth / Owner.TotalHealth) <= .30)
                    {
                        numSpins++;
                    }
                }

                origVector = new Vector2(enemy.player.PosX + (enemy.player.PosRect.Width / 2), enemy.player.PosY + (enemy.player.PosRect.Height / 2)) - new Vector2(Owner.PosX + Owner.PosRect.Width / 2, Owner.PosY + Owner.PosRect.Height / 2);
                if (origVector != Vector2.Zero)
                {
                    //Normalize the vector to get the direction
                    origVector.Normalize();
                }

                target = new Vector2((float)(Math.Cos(0)) * (Owner.PosX + Owner.PosRect.Width / 2) - (float)(Math.Sin(0)) * (Owner.PosY + Owner.PosRect.Height / 2), (float)(Math.Sin(0)) * (Owner.PosX + Owner.PosRect.Width / 2) - (float)(Math.Cos(0)) * (Owner.PosY + Owner.PosRect.Height / 2));

                if (target != Vector2.Zero)
                {
                    //Normalize the vector to get the direction
                    target.Normalize();
                }

                //Game1.CurrProjectiles.Add(new Projectile(Damage, null, Owner, target, 1, 7, 10, 10, Owner.PosX + Owner.PosRect.Width / 2, Owner.PosY + Owner.PosRect.Height / 2, 5, false));

                enemy.IsSpinning = true;
            }
        }
    }
}
