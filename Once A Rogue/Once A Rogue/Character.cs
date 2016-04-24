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
        public bool IsPoisened
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
        private int stunDur;
        private int rootDur;
        private int snareDur;
        private int snareAmt;
        private int poisenDur;
        private int poisenDmg;
        private int fireDur;
        private int fireDmg;

        //Properties for the above variables
        public int StunDur
        {
            get { return stunDur; }

            //Duration of the status is reduced by a percentage based upon resistence of the character
            set
            {
                stunDur = (int)(value * (1-stunResist));

                if (stunDur < 0)
                {
                    stunDur = 0;
                }

                if (stunDur != 0)
                {
                    isStunned = true;
                }
            }
        }

        public int RootDur
        {
            get { return rootDur; }
            set
            {
                rootDur = (int)(value * (1 - rootResist));

                if (rootDur < 0)
                {
                    rootDur = 0;
                }

                if (rootDur != 0)
                {
                    isRooted = true;
                }
            }
        }
        public int SnareDur
        {
            get { return snareDur; }
            set { snareDur = value; }
        }
        public int SnareAmount
        {
            get { return snareAmt; }
            set { snareAmt = (int)(value * (1 - snareResist)); }
        }
        public int PoisenDur
        {
            get { return poisenDur; }
            set
            {
                poisenDur = value;

                if (poisenDur < 0)
                {
                    poisenDur = 0;
                }

                if (poisenDur != 0)
                {
                    isPoisoned = true;
                }
            }
        }
        public int PoisenDmg
        {
            get { return poisenDmg; }

            //Damage is reduced by a percentage based on character resist
            set { poisenDmg = (int)(value * (1 - poisenResist)); }
        }
        public int FireDur
        {
            get { return fireDur; }
            set
            {
                fireDur = value;

                if(fireDur < 0)
                {
                    fireDur = 0;
                }

                if(fireDur != 0)
                {
                    isOnFire = true;
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
        
        private double poisenResist;

        virtual public double PoisenResist
        {
            get { return poisenResist; }
            set
            {
                if (value > 1.0)
                {
                    poisenResist = 1.0;
                }

                else
                {
                    poisenResist = value;
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

        //Timers to keep track of status effects
        Timer poisonTimer = new Timer(1000);
        Timer stunTimer = new Timer(1000);
        Timer snareTimer = new Timer(1000);
        Timer rootTimer = new Timer(1000);
        Timer fireTimer = new Timer(1000);

        //Method to update the player
        virtual public void Update()
        {
            //Checking for status effects

            StatusChecker(snareTimer, isSnared);

            StatusChecker(rootTimer, isRooted);

            StatusChecker(stunTimer, isStunned);

            StatusChecker(fireTimer, isOnFire);

            StatusChecker(poisonTimer, isPoisoned);
        }

        //Event for the poisen timer or fire timer triggering
        private void StatusTriggered(object sender, ElapsedEventArgs e)
        {
            //Adjusting all statuses timers if they are true

            StatusTimerAdjust(snareTimer, isSnared, SnareDur, MoveSpeedTotal, MoveSpeed);

            StatusTimerAdjust(rootTimer, isRooted, RootDur, MoveSpeedTotal, MoveSpeed);

            StatusTimerAdjust(stunTimer, isStunned, StunDur);

            StatusTimerAdjust(fireTimer, isOnFire, FireDur, FireDmg);

            StatusTimerAdjust(poisonTimer, isPoisoned, PoisenDur, PoisenDmg);
        }

        //Method for checking a status and setting up a timer for that status if one does not exist
        public void StatusChecker(Timer timer, bool status)
        {
            //If afflicted by status effect
            if (status == true)
            {
                //Create an event for when the interval goes off
                timer.Elapsed += StatusTriggered;

                //Start the timer
                timer.Start();
            }
        }

        //StatusTimer adjustment method
        public void StatusTimerAdjust(Timer timer, bool status, int dur)
        {
            if (status == true)
            {
                //Subtracting one from the duration of the timer
                dur -= 1;

                //Seeing if the stun is over
                if (dur <= 0)
                {
                    dur = 0;

                    status = false;

                    //Stopping the timer
                    timer.Stop();
                }
            }
        }

        //Overload for the Status timer adjustment used for movement speed
        public void StatusTimerAdjust(Timer timer, bool status, int dur, int totalStat, int affectedStat)
        {
            if (status == true)
            {
                //Subtracting one from the duration of the timer
                dur -= 1;

                //Seeing if the stun is over
                if (dur <= 0)
                {
                    dur = 0;

                    //Returning the stat to normal
                    affectedStat = totalStat;

                    status = false;

                    //Stopping the timer
                    timer.Stop();
                }
            }
        }

        //Overload for the status timer adjustment used for fire and poisen
        public void StatusTimerAdjust(Timer statTimer, bool status, int dur, int dmg)
        {
            if (status == true)
            {
                //Subtracting one from the duration of the timer
                dur -= 1;

                //Taking damage dependent on the characters resistence
                CurrHealth -= dmg;

                //Seeing if the stun is over
                if (dur <= 0)
                {
                    dur = 0;

                    status = false;

                    //Stopping the timer
                    statTimer.Stop();
                }
            }
        }

        //Method for events that occur when the character dies
        virtual public void OnDeath()
        {
            //Resetting the status effects
            isRooted = false;
            isSnared = false;
            isPoisoned = false;
            isStunned = false;
            isOnFire = false;
            isExplosive = false;

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
