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
        //Attributes
        private int damage;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public MeleeAttack(Player player)
        {
            //Setting default values
            Cooldown = 0;
            RangeX = 1;
            RangeY = 0;
            BurstRadius = 0;
            
            //Setting the attacks damage and total cooldown based on the weapon currently equipped.
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

        //Method for when the skill is activated
        public override void OnActivated(Player player)
        {
            //Getting position of the mouse
            MouseState msState = Mouse.GetState();

            //Currently does not work but will eventually handle the player animation based on player position vs mouse position.
            if (msState.X <= player.PosX + player.PosRect.Width / 2)
            {
                player.PlayerStates = Player.PlayerState.AttackLeft;
                player.CurrentFrame = 0;
                player.framesElapsed = 0;
                player.timeElapsed = 0;
                Cooldown = CooldownTotal;
            }

            if (msState.X > player.PosX + player.PosRect.Width / 2)
            {
                player.PlayerStates = Player.PlayerState.AttackRight;
                player.CurrentFrame = 0;
                player.framesElapsed = 0;
                player.timeElapsed = 0;
                Cooldown = CooldownTotal;
            }
        }
    }
}
