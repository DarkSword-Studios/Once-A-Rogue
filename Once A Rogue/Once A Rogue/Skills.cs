﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Once_A_Rogue
{
    class Skills
    //Ian Moon
    //3/9/2016
    //This class is the foundation for the skills in the game
    {
        //Attributes
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        //Cooldown handling
        private double cooldown;

        public double Cooldown
        {
            get { return cooldown; }
            set { cooldown = value; }
        }

        private double cooldownTotal;

        public double CooldownTotal
        {
            get { return cooldownTotal; }
            set { cooldownTotal = value; }
        }

        //Range values of the skill
        private int rangeX;

        public int RangeX
        {
            get { return rangeX; }
            set { rangeX = value; }
        }

        private int rangeY;

        public int RangeY
        {
            get { return rangeY; }
            set { rangeY = value; }
        }

        //Determining whether the skill is direction based or area based.
        private bool isDirectional;

        public bool IsDirectional
        {
            get { return isDirectional; }
            set { isDirectional = value; }
        }


        //How far out the burst is of the skill is from a center point
        private int burstRadius;

        public int BurstRadius
        {
            get { return burstRadius; }
            set
            {
                if(burstRadius <= 0)
                {
                    burstRadius = 0;
                }
                else
                {
                    burstRadius = value;
                }
            }
        }

        private int cost;

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        private int damage;

        private Character owner;

        public Character Owner
        {
            get { return owner; }
            set { owner = value; }
        }


        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public Skills(int dam, Character own)
        {
            Damage = dam;
            owner = own;
        }

        public Skills(Character own)
        {
            owner = own;
            
        }

        //A method for when the skill is used
        virtual public void OnActivated(Player player)
        {
            //Getting position of the mouse
            MouseState msState = Mouse.GetState();

            //Handles the player animation based on player position vs mouse position.
            if (msState.X <= player.PosX + player.PosRect.Width / 2)
            {
                if(player.rightStickInput.Length() <= .25f && player.CurrSkill.Name != "Swing")
                {
                    player.PlayerStates = Player.PlayerState.AttackLeft;
                }
                else if(player.CurrSkill.Name == "Swing")
                {
                    player.PlayerStates = Player.PlayerState.AttackLeft;
                }
                player.CurrentFrame = 0;
                player.framesElapsed = 0;
                player.timeElapsed = 0;
                Cooldown = CooldownTotal;
            }

            else
            {
                if (player.rightStickInput.Length() <= .25f && player.CurrSkill.Name != "Swing")
                {
                    player.PlayerStates = Player.PlayerState.AttackRight;
                }
                else if (player.CurrSkill.Name == "Swing")
                {
                    player.PlayerStates = Player.PlayerState.AttackRight;
                }
                player.CurrentFrame = 0;
                player.framesElapsed = 0;
                player.timeElapsed = 0;
                Cooldown = CooldownTotal;
            }
        }

        virtual public void OnActivated()
        {

        }

        public override string ToString()
        {
            return name;
        }
    }
}
