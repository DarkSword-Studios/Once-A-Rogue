using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{

    class Enemy : Character
    {   //Julian Januszka
        //3/15/16
        //Inherited class of Character to handle enemies 


        private List<Skills> skillList;

        public List<Skills> SkillList
        {
            get { return skillList; }
            set { skillList = value; }
        }

        //Value for fear level of enemy
        private int fearLevel;
        private int fearLevelTotal;

        bool isHostile;

        int timePerFrame = 100;
        int numFrames = 6;
        public int framesElapsed;
        public int timeElapsed;
        public int currentFrame;

        public int relativeCamX;
        public int relativeCamY;
        public Boolean justSpawned = false;

        public int pathSpeedX;
        public int pathSpeedY;

        enum enemyState { IdleRight, IdleLeft, WalkingRight, WalkingLeft, AttackRight, AttackLeft  };
        enemyState eState;

        public Player player;

        //Property for Fear Level
        public int FearLevel
        {
            get { return fearLevel; }

            set
            {
                //If FearLevel is over 100, Player IsFeared and fearLevel is set to 100 
                if (fearLevel > 100)
                {
                    
                    isFeared = true;
                    fearLevel = 100;
                }
                else
                {
                    fearLevel = value;
                }
            }
        }
        public int FearLevelTotal
        {
            get { return FearLevelTotal; }

            set { fearLevelTotal = value; }
        }
        //BRAINSTORM: Based on the armor level of enemies, the players will do reduced damage to them
        private int armorLevel;

        public int ArmorLevel
        {
            get { return armorLevel; }

            //Series of if loops to determine damage reduction based on enemy armor level
            set
            {
                if(armorLevel < 10)
                {
                    //If the armor level is less than 10, player will have no damage reduction against enemy
                }
                if(armorLevel > 10 && armorLevel <= 25)
                {
                    //If armor level is between 10 and 25, reduce damage player does by 10%
                }
                if(armorLevel > 25 && armorLevel <= 50)
                {
                    //If armor level is between 26 and 50, reduce damage player does by 15%
                }
                if(armorLevel > 50 && armorLevel <= 75)
                {
                    //If armor level is between 50 and 75, reduce damage player does by 20%
                }
                if(armorLevel > 75 && armorLevel <=100)
                {
                    //If armor level is between 75 and 100, reduce damage player does by 25%
                }
                if (armorLevel > 100)
                {
                    armorLevel = 100;
                }
                else
                {
                    armorLevel = value;
                }
            }
        }

        //BRAINSTORM: Attribute to control stealth of enemy
        private int stealthLevel;

        public int StealthLevel
        {
            get { return stealthLevel; }
            set
            {
                if(stealthLevel > 50)
                {
                    //Change opacity of particular enemy sprite here:

                    //Since in stealth, make enemy move slower
                    MoveSpeed = MoveSpeedTotal / 2;

                    //Add attribute to increase 'dodge' ability of enemy (IE more likely to not get hit by attacks)
                }
                else
                {
                    stealthLevel = value;
                }
            }
        }
        //Method to handle enemy attacks
        public void Attack()
        {
            //Add code to handle enemy attacking
        }

        //BRAINSTORM: Only call this method with certain types of enemies that would likely run away when faced with
        // certain death. IE: Kobolds, Goblins, Etc 

         
        //Non overloaded OnDeath method from base
        public override void OnDeath()
        {
            base.OnDeath();
        }

        //Overloaded OnDeath method that passes in a Player object
        public virtual void OnDeath(Player play)
        {
            base.OnDeath();

            //Add additional code here to handle animation of enemy dying

            //Increment the 'Soul Count' of the player'
            play.Souls++;
        }
        public void Retreat()
        {
            //If loop for if enemy is at 1/5th of its maximum health
            if(CurrHealth == TotalHealth / 5)
            {
                //Automatically give enemy "IsFeared" attribute
                isFeared = true;
                //Add code here to make enemy run in a random direction
            }

            //If loop for if enemy is at 1/7th of its maximum health. At this point it stops running away and goes
            // into "last stand" mode which increases it's attack but drastically lowers it's defense
            if (CurrHealth == TotalHealth / 7)
            {
                //Automatically set IsFeared to false since enemy is in "last stand" mode
                isFeared = false;
               
                //Automatically set armor level of enemy to 0 (they drop their sheilds and helmets in fear)
                armorLevel = 0;

                //BRAINSTORM: Make IsSnared variable true, enemy is expending all their energy trying
                // to survive and fight, thus they probably can't move at full speed
                //Also alter the movement speed, both this and the IsSnared = True may be redundant
                MoveSpeed = MoveSpeedTotal / 3;

                //Add code here to increase damage done by enemy

            }

          }

        //BRAINSTORM: If there are a certain amount of enemies of a certain type gathered together, boost attack of enemies
        public void Rally()
        {
            
        }

        //Do we even need this constructor?
        public Enemy(Texture2D tex, Player play, Camera camera, int x, int y, int width, int height, bool host) : base()//Add code here
        {
            relativeCamX = camera.xMod;
            relativeCamY = camera.yMod;
            Texture = tex;
            PosRect = new Rectangle(x, y, width, height);
            eState = enemyState.IdleLeft;
            Level = play.Level;
            fearLevel = 0;
            armorLevel = 5;
            isHostile = host;
            isHostile = false;
            FireResist = 0;
            StunResist = 0;
            PoisenResist = 0;
            SnareResist = 0;
            RootResist = 0;
            player = play;
        }

        public void Draw(SpriteBatch spritebatch, int frameWidth, int frameHeight)
        {
            Rectangle frame;

            switch (eState)
            {
                case enemyState.IdleRight:

                    frame = new Rectangle(currentFrame * 140, 0, frameWidth, frameHeight);
                    spritebatch.Draw(Texture, PosRect, frame, Color.White);
                    break;

                case enemyState.IdleLeft:

                    frame = new Rectangle(currentFrame * 140,  280, frameWidth, frameHeight);
                    spritebatch.Draw(Texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case enemyState.WalkingRight:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(Texture, PosRect, frame, Color.White);
                    break;

                case enemyState.WalkingLeft:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(Texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case enemyState.AttackLeft:

                    frame = new Rectangle(currentFrame * 140, 280, frameWidth, frameHeight);
                    spritebatch.Draw(Texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case enemyState.AttackRight:

                    frame = new Rectangle(currentFrame * 140, 280, frameWidth, frameHeight);
                    spritebatch.Draw(Texture, PosRect, frame, Color.White);
                    break;
            }
            

        }
        public void UpdateFrame(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            framesElapsed = (int)(timeElapsed / timePerFrame);
            currentFrame = framesElapsed % numFrames + 1;
        }

        public void UpdatePathDirection(string direction)
        {

            switch (direction)
            {
                case "up":

                    pathSpeedY = -MoveSpeed;
                    pathSpeedX = 0;
                    break;

                case "down":

                    pathSpeedY = MoveSpeed;
                    pathSpeedX = 0;
                    break;

                case "left":

                    pathSpeedY = 0;
                    pathSpeedX = -MoveSpeed;
                    break;

                case "right":

                    pathSpeedY = 0;
                    pathSpeedX = MoveSpeed;
                    break;

                case "upright":

                    pathSpeedY = -MoveSpeed;
                    pathSpeedX = MoveSpeed;
                    break;

                case "upleft":

                    pathSpeedY = -MoveSpeed;
                    pathSpeedX = -MoveSpeed;
                    break;

                case "downleft":

                    pathSpeedY = -MoveSpeed;
                    pathSpeedX = MoveSpeed;
                    break;

                case "downright":

                    pathSpeedY = MoveSpeed;
                    pathSpeedX = MoveSpeed;
                    break;
            }
        }

        public void UpdatePathPosition()
        {
            PosX += pathSpeedX;
            PosY += pathSpeedY;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
