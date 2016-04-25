using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Once_A_Rogue
{
    public class Character:GameObject
    //Ian Moon
    //03/9/2016
    //This is the base class for all interactable characters in the game
    {
        //Enumeration for movement animation
        enum MovementState { FacingRight,FacingLeft,MovingLeft,MovingRight};

        //Enumeration for specific animation triggers
        enum CharacterState { Normal, Stunned, Damaged, OnFire }

        //Gameplay Attributes

        private int level;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }


        //Health properties
        private int currHealth;

        public int CurrHealth
        {
            get { return currHealth; }
            set
            {
                //If the value is less than 0
                if(value < 0)
                {
                    //Set the health to equal 0
                    currHealth = 0;

                    //Run the dying method
                    OnDeath();
                }

                //If the current health would be greater than the max health
                else if(value > maxHealth)
                {
                    //Current health equals the max health
                    currHealth = maxHealth;
                }

                //Set the current health equal to the value passed in
                else
                {
                    currHealth = value;
                }
            }
        }

        //Int to handle maximum amount of health that the character can have
        private int maxHealth;

        public int TotalHealth
        {
            get { return maxHealth; }
            set
            {
                //If the max health is reduced below the current health
                if (value < currHealth)
                {
                    //Max health equals value passed in and current health gets adjusted to the new max health
                    maxHealth = value;
                    currHealth = maxHealth;
                }

                //Set the current health equal to the value passed in
                else
                {
                    maxHealth = value;
                }
            }
        }

        //Movement speed of the character

        private int moveSpeedTotal;

        public int MoveSpeedTotal
        {
            get { return moveSpeedTotal; }
            set { moveSpeedTotal = value; }
        }

        //Represents the current movement speed of the character
        private int moveSpeed;

        public int MoveSpeed
        {
            get { return moveSpeed; }
            set
            {
                //If value is less than 0, movespeed is 0 and the player is rooted
                if(value < 0)
                {
                    moveSpeed = 0;
                    isRooted = true;
                }

                //If the value is greater than the total move speed, set movespeed to moverSpeedTotal
                else if(value > moveSpeedTotal)
                {
                    moveSpeed = moveSpeedTotal;
                }
                
                else
                {
                    moveSpeed = value;
                }

                if((moveSpeed < moveSpeedTotal) && (moveSpeed != 0))
                {
                    isSnared = true;
                }
            }
        }

        //Bools to test if the character is afflicted by a status effect
        protected bool isStunned;

        public bool IsStunned
        {
            get { return isStunned; }
            set { isStunned = value; }
        }

        protected bool isRooted;
        public bool IsRooted
        {
            get { return isRooted; }
            set { isRooted = value; }
        }

        protected bool isSnared;
        public bool IsSnared
        {
            get { return isSnared; }
            set { isSnared = value; }
        }

        protected bool isOnFire;
        public bool IsOnFire
        {
            get { return isOnFire; }
            set { isOnFire = value; }
        }

        protected bool isPoisoned;
        public bool IsPoisoned
        {
            get { return isPoisoned; }
            set { isPoisoned = value; }
        }

        protected bool isExplosive;
        public bool IsExplosive
        {
            get { return isExplosive; }
            set { isExplosive = value; }
        }

        protected bool isFeared;
        public bool IsFeared
        {
            get { return isOnFire; }
            set { isOnFire = value; }
        }

        //Variables of the status effects
        private double explosiveDur;
        private double stunDur;
        private double rootDur;
        private double snareDur;
        private int snareAmt;
        private double poisonDur;
        private int poisonDmg;
        private double fireDur;
        private int fireDmg;

        //Properties for the above variables
        public double ExplosiveDur
        {
            get { return explosiveDur; }
            set
            {
                explosiveDur = value;

                if (explosiveDur <= 0)
                {
                    explosiveDur = 0;
                    IsExplosive = false;
                }

                if (explosiveDur != 0)
                {
                    IsExplosive = true;
                }
            }
        }

        public double StunDur
        {
            get { return stunDur; }

            //Duration of the status is reduced by a percentage based upon resistence of the character
            set
            {
                stunDur = value * (1-stunResist);

                if (stunDur <= 0)
                {
                    stunDur = 0;
                    IsStunned = false;
                }

                if (stunDur != 0)
                {
                    IsStunned = true;
                }
            }
        }

        public double RootDur
        {
            get { return rootDur; }
            set
            {
                rootDur = value * (1 - rootResist);

                if (rootDur <= 0)
                {
                    rootDur = 0;
                    IsRooted = false;
                }

                if (rootDur != 0)
                {
                    IsRooted = true;
                    MoveSpeed = 0;
                }
            }
        }
        public double SnareDur
        {
            get { return snareDur; }
            set
            {
                snareDur = value * (1 - rootResist);

                if (snareDur <= 0)
                {
                    snareDur = 0;
                    IsSnared = false;
                }

                if (snareDur != 0)
                {
                    IsSnared = true;
                }
            }
        }
        public int SnareAmount
        {
            get { return snareAmt; }
            set { MoveSpeed -= value; }
        }
        public double PoisonDur
        {
            get { return poisonDur; }
            set
            {
                poisonDur = value;

                if (poisonDur <= 0)
                {
                    poisonDur = 0;
                    IsPoisoned = false;
                }

                if (poisonDur != 0)
                {
                    IsPoisoned = true;
                }
            }
        }
        public int PoisonDmg
        {
            get { return poisonDmg; }

            //Damage is reduced by a percentage based on character resist
            set { poisonDmg = (int)(value * (1 - poisonResist)); }
        }

        public double FireDur
        {
            get { return fireDur; }
            set
            {
                fireDur = value;

                if(fireDur <= 0)
                {
                    fireDur = 0;
                    IsOnFire = false;
                }

                if(fireDur != 0)
                {
                    IsOnFire = true;
                }
            }
        }
        public int FireDmg
        {
            get { return fireDmg; }
            set { fireDmg = (int)(value * (1 - fireResist)); }
        }

        //Attributes for the characters resistence to the status effects

        private double stunResist;

        //Overrideable property
        virtual public double StunResist
        {
            get { return stunResist; }
            set
            {
                //Putting a resist cap
                if(value > 0.5)
                {
                    stunResist = 0.5;
                }
                
                //Note this allows the resistence to go negative, which means that characters can be weak to certain statuses
                else
                {
                    stunResist = value;
                }
            }
        }
        
        private double poisonResist;

        virtual public double PoisonResist
        {
            get { return poisonResist; }
            set
            {
                if (value > 1.0)
                {
                    poisonResist = 1.0;
                }

                else
                {
                    poisonResist = value;
                }
            }
        }

        private double rootResist;

        virtual public double RootResist
        {
            get { return rootResist; }
            set
            {
                if (value > 0.5)
                {
                    rootResist = 0.5;
                }

                else
                {
                    rootResist = value;
                }
            }
        }

        private double snareResist;

        virtual public double SnareResist
        {
            get { return snareResist; }
            set
            {
                if (value > 0.5)
                {
                    snareResist = 0.5;
                }

                else
                {
                    snareResist = value;
                }
            }
        }

        private double fireResist;

        virtual public double FireResist
        {
            get { return fireResist; }
            set
            {
                if (value > 0.5)
                {
                    fireResist = 0.5;
                }

                else
                {
                    fireResist = value;
                }
            }
        }

        double elapsedTime = 0;

        //Method to update the player
        virtual public void Update(GameTime gameTime)
        {
            //Checking for status effects
            if(IsOnFire == true)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if(elapsedTime >= 1000)
                {
                    elapsedTime = 0;
                    FireDur -= 1000;
                    CurrHealth -= FireDmg;
                }
            }

            if (IsPoisoned == true)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= 1000)
                {
                    elapsedTime = 0;
                    PoisonDur -= 1000;
                    CurrHealth -= PoisonDmg;
                }
            }

            if (IsSnared == true)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= 1000)
                {
                    elapsedTime = 0;
                    SnareDur -= 1000;
                }
            }

            if (IsRooted == true)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= 1000)
                {
                    elapsedTime = 0;
                    RootDur -= 1000;
                }
            }

            if (IsStunned == true)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= 1000)
                {
                    elapsedTime = 0;
                    StunDur -= 1000;
                }
            }

            if (IsExplosive == true)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= 1000)
                {
                    elapsedTime = 0;
                    ExplosiveDur -= 1000;
                }
            }

        }

        //Method for events that occur when the character dies
        virtual public void OnDeath()
        {
            //Resetting the status effects
            IsRooted = false;
            IsSnared = false;
            IsPoisoned = false;
            IsStunned = false;
            IsOnFire = false;
            IsExplosive = false;

            //TO DO: Determine what else needs to happen here.
        }

        //Method for the character to draw itself
        virtual public void Draw(SpriteBatch spritebatch)
        {
            //TO DO: Adjust drawing based on current states of the character
            spritebatch.Draw(Texture, PosRect, Color.White);
        }
    }
}
