using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class MeleeAttack:Skills
    //Ian Moon
    //3/15/2016
    //This class represents a melee attack by the player
    {
        public MeleeAttack(Character own): base(own)
        {
            //Setting default values
            Cooldown = 0;
            RangeX = 1;
            RangeY = 0;
            BurstRadius = 0;
        }

        //Method for when the skill is activated
        public override void OnActivated(Player player)
        {
            //Setting the attacks damage and total cooldown based on the weapon currently equipped.
            if (player.CurrWeapon == "Sword")
            {
                CooldownTotal = 1000;
                Damage = 10;
            }

            if (player.CurrWeapon == "Daggers")
            {
                CooldownTotal = 500;
                Damage = 5;
            }

            if (player.CurrWeapon == "Bow")
            {
                CooldownTotal = 1000;
                Damage = 7;
            }

            if (player.CurrWeapon == "Staff")
            {
                CooldownTotal = 1500;
                Damage = 5;
            }

            if (Cooldown == 0)
            {
                base.OnActivated(player);
            }
        }
    }
}