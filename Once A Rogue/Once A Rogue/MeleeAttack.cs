using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{

    //Class to handle Melee Attack skill
    class MeleeAttack:Skills
    {
        //Attributes

        //Int to store damage of melee attack
        private int damage;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        //Constructor passing in player object
        public MeleeAttack(Player player)
        {
            Cooldown = 0;
            RangeX = 1;
            RangeY = 0;
            BurstRadius = 0;
            
            //If loops to determine damage and cooldown of different weapons
            if(player.CurrWeapon == "Sword")
            {
                CooldownTotal = 1;
                damage = 10;
            }

            if(player.CurrWeapon == "Daggers")
            {
                CooldownTotal = .5;
                damage = 5;
            }

            if(player.CurrWeapon == "Bow")
            {
                CooldownTotal = 1;
                damage = 7;
            }

            if(player.CurrWeapon == "Staff")
            {
                CooldownTotal = 1.5;
                damage = 5;
            }
        }

        //Overriden OnActivated method
        public override void OnActivated(Player player)
        {
            //Getting position of the mouse
            MouseState msState = Mouse.GetState();

            if(msState.X >= player.PosX)
            {
                player.PlayerStates = Player.PlayerState.AttackLeft;
                Cooldown = CooldownTotal;
            }

            if ((player.PlayerStates == Player.PlayerState.IdleRight) || (player.PlayerStates == Player.PlayerState.WalkingRight))
            {
                player.PlayerStates = Player.PlayerState.AttackRight;
                Cooldown = CooldownTotal;
            }
        }
    }
}
