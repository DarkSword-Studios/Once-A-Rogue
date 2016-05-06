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
    class Charge:Skills
    {
        int PosX;
        int PosY;

        public Charge(int dam, Character own) : base(dam, own)
        {
            CooldownTotal = 5000;
            Cooldown = CooldownTotal;
            Owner = own;
            Damage = dam;
        }

        public override void OnActivated()
        {
            if(Owner.CurrHealth/Owner.TotalHealth <= .30)
            {
                CooldownTotal = 2500;
            }

            PosX = Owner.PosX;
            PosY = Owner.PosY;

            if(Owner is Enemy)
            {
                Enemy enemy = (Enemy)Owner;

                if (enemy.CurrHealth / enemy.TotalHealth <= .30)
                {
                    enemy.MoveSpeed = 10;
                }

                else
                {
                    enemy.MoveSpeed = 5;
                }

                enemy.IsCharging = true;
            }
        }
    }
}
