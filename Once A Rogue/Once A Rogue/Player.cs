﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Player : Character
    //Ian Moon
    //3/13/2016
    //Class that represents the player character
    {
        //Attributes

        //Lists containing known skills
        public List<Skills> mageSkillList;
        public List<Skills> rogueSkillList;
        public List<Skills> rangerSkillList;
        public List<Skills> warriorSkillList;

        //Dictionaries for looking up skills
        public List<Skills> mageSkillDict;
        public List<Skills> rogueSkillDict;
        public List<Skills> rangerSkillDict;
        public List<Skills> warriorSkillDict;

        //String array of weapon choices
        string[] weaponArray;

        //Soul Count
        private int souls;

        public int Souls
        {
            get { return souls; }
            set { souls = value; }
        }

        private int soulsNeeded;

        public int SoulsNeeded
        {
            get { return soulsNeeded; }
            set { soulsNeeded = value; }
        }

        private int skillPoints;

        public int SkillPoints
        {
            get { return skillPoints; }
            set { skillPoints = value; }
        }

        //Currently selected weapon
        private string currWeapon;

        //Keeping track of the direction the player is facing
        //string direction;

        public string CurrWeapon
        {
            get { return currWeapon; }
            set { currWeapon = value; }
        }

        //Currently selected skill
        private Skills currSkill;

        //Current skillList
        List<Skills> currSkillList;

        public Skills CurrSkill
        {
            get { return currSkill; }
            set { currSkill = value; }
        }

        //Stuff added by Stasha
        private int currentFrame;

        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        int skillIndex;

        //MouseStates
        MouseState prevMS;
        MouseState msState;

        //GamePadStates
        GamePadState prevGPadState;
        GamePadState gPadState;

        public Color color = Color.White;
        public float colorPhase = 1;

        //Mana properties
        private int currMana;

        public int CurrMana
        {
            get { return currMana; }
            set { currMana = value; }
        }

        private int totalMana;

        public int TotalMana
        {
            get { return totalMana; }
            set { totalMana = value; }
        }

        //Mana Regen Property
        private int manaRegen;

        public int ManaRegen
        {
            get { return manaRegen; }
            set { manaRegen = value; }
        }

        //Health Regen Property
        private int healthRegen;

        public int HealthRegen
        {
            get { return healthRegen; }
            set { healthRegen = value; }
        }

        //Health Regen Rate Property
        private int healthRegenRate;

        public int HealthRegenRate
        {
            get { return healthRegenRate; }
            set { healthRegenRate = value; }
        }

        //Mana Regen Rate Property
        private int manaRegenRate;

        public int ManaRegenRate
        {
            get { return manaRegenRate; }
            set { manaRegenRate = value; }
        }


        //Percentage of HP remaining property
        private float percentHP;

        public float PercentHP
        {
            get { return percentHP; }
        }

        private float percentMP;

        public float PercentMP
        {
            get { return percentMP; }
        }

        //Keeps track of the current frame of animation for the player
        int timePerFrame = 50;
        int numFrames = 6;
        public int framesElapsed;
        public int timeElapsed;

        int manaTimer;
        int healthTimer;

        public enum PlayerState { IdleLeft, IdleRight, WalkingLeft, WalkingRight, AttackLeft, AttackRight };

        private PlayerState playerState;

        public PlayerState PlayerStates
        {
            get { return playerState; }
            set { playerState = value; }
        }

        //Gamepad variables
        public Vector2 rightStickInput;
        public Vector2 leftStickInput;
        public float deadZone;


        //Creating a hitbox rectangle
        private Rectangle hitBox;

        public Rectangle HitBox
        {
            get { return hitBox; }
            set { hitBox = value; }
        }

        public int HitBoxX
        {
            get { return hitBox.X; }
            set { hitBox.X = value; }
        }

        public int HitBoxY
        {
            get { return hitBox.Y; }
            set { hitBox.Y = value; }
        }

        //Block variables
        private bool isBlocking;

        public bool IsBlocking
        {
            get { return isBlocking; }
            set { isBlocking = value; }
        }

        private int blockAmt;

        public int BlockAmount
        {
            get { return blockAmt; }
            set { blockAmt = value; }
        }

        int costTimer;

        private string direction;

        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }


        public Player(int x, int y, int width, int height)
        {
            Level = 1;
            SoulsNeeded = 1000;
            SkillPoints = 1;

            //Initializing the skill collections
            warriorSkillList = new List<Skills>();
            mageSkillList = new List<Skills>();
            rangerSkillList = new List<Skills>();
            rogueSkillList = new List<Skills>();

            //Adding skills to each
            warriorSkillList.Add(new MeleeAttack(this));
            mageSkillList.Add(new MeleeAttack(this));
            rogueSkillList.Add(new MeleeAttack(this));
            rangerSkillList.Add(new StandardShot(4, this));

            //warriorSkillList.Add(new Block(this));
            //rangerSkillList.Add(new PiercingShot(6, this));
            //rogueSkillList.Add(new FanOfKnives(6, this));
            //mageSkillList.Add(new Fireball(4, this));
            //mageSkillList.Add(new OilThrow(this));

            //Initializing the weapon array
            weaponArray = new string[4];
            weaponArray[0] = "Sword";
            weaponArray[1] = "Daggers";
            weaponArray[2] = "Bow";
            weaponArray[3] = "Staff";

            //Setting default values
            playerState = PlayerState.IdleRight;
            MoveSpeedTotal = 6;
            MoveSpeed = 6;
            StunResist = 0;
            RootResist = 0;
            FireResist = 0;
            SnareResist = 0;
            PoisonResist = 0;
            TotalHealth = 75;
            TotalMana = 100;
            CurrMana = 100;
            CurrHealth = 75;
            HealthRegen = 0;
            ManaRegen = 1;
            BlockAmount = 3;
            manaTimer = 0;
            HealthRegenRate = 1000;
            ManaRegenRate = 1000;
            costTimer = 0;
            PosRect = new Rectangle(x, y, width, height);
            HitBox = new Rectangle(x + 20, y + 30, width - 60, height - 40);
            currWeapon = weaponArray[0];
            currSkillList = warriorSkillList;
            CurrSkill = currSkillList[0];
            currentFrame = 0;
            skillIndex = 0;
            Direction = "right";
            IsBlocking = false;
        }

        //Method for processing user input
        public void ProcessInput(int roomWidth, int roomHeight)
        {
            //Figuring out which key is down
            prevMS = msState;
            msState = Mouse.GetState();
            KeyboardState kbs = Keyboard.GetState();
            prevGPadState = gPadState;
            gPadState = GamePad.GetState(PlayerIndex.One);

            if(gPadState.IsConnected)
            {
                deadZone = 0.25f;

                leftStickInput = new Vector2(gPadState.ThumbSticks.Left.X, gPadState.ThumbSticks.Left.Y);
                rightStickInput = new Vector2(gPadState.ThumbSticks.Right.X, gPadState.ThumbSticks.Right.Y);

                if (leftStickInput.Length() > deadZone)
                {
                    if(leftStickInput.X < 0)
                    {
                        playerState = PlayerState.WalkingLeft;
                    }

                    if(leftStickInput.X > 0)
                    {
                        playerState = PlayerState.WalkingRight;
                    }

                    if(leftStickInput.Y != 0)
                    {
                        if (playerState == PlayerState.IdleRight)
                        {
                            playerState = PlayerState.WalkingRight;
                        }

                        else if (playerState == PlayerState.IdleLeft)
                        {
                            playerState = PlayerState.WalkingLeft;
                        }
                    }

                    if(kbs.IsKeyUp(Keys.W) && kbs.IsKeyUp(Keys.A) && kbs.IsKeyUp(Keys.S) && kbs.IsKeyUp(Keys.D))
                    {
                        leftStickInput.Normalize();
                        if(leftStickInput.X > deadZone)
                        {
                            PosX += MoveSpeed;
                            HitBoxX += MoveSpeed;
                        }
                        if(leftStickInput.X <= -deadZone)
                        {
                            PosX -= MoveSpeed;
                            HitBoxX -= MoveSpeed;
                        }

                        if (leftStickInput.Y > deadZone)
                        {
                            PosY += MoveSpeed;
                            HitBoxY += MoveSpeed;
                        }

                        if (leftStickInput.Y <= -deadZone)
                        {
                            PosY -= MoveSpeed;
                            HitBoxY -= MoveSpeed;
                        }
                    }
                }
            }

            //Assume the player is moving, a special case exists to correct idle movement
            timePerFrame = 50;

            //Moving the player based on which key is down
            if (kbs.IsKeyDown(Keys.A))
            {
                PosX -= MoveSpeed;
                HitBoxX -= MoveSpeed;

                playerState = PlayerState.WalkingLeft;
            }

            if (kbs.IsKeyDown(Keys.D))
            {
                PosX += MoveSpeed;
                HitBoxX += MoveSpeed;

                playerState = PlayerState.WalkingRight;
            }

            if (kbs.IsKeyDown(Keys.S))
            {
                PosY += MoveSpeed;
                HitBoxY += MoveSpeed;

                if (playerState == PlayerState.IdleRight)
                {
                    playerState = PlayerState.WalkingRight;
                }
                else if (playerState == PlayerState.IdleLeft)
                {
                    playerState = PlayerState.WalkingLeft;
                }
            }

            if (kbs.IsKeyDown(Keys.W))
            {
                PosY -= MoveSpeed;
                HitBoxY -= MoveSpeed;

                if (playerState == PlayerState.IdleRight)
                {
                    playerState = PlayerState.WalkingRight;
                }
                else if (playerState == PlayerState.IdleLeft)
                {
                    playerState = PlayerState.WalkingLeft;
                }
            }

            if (kbs.IsKeyUp(Keys.A) && kbs.IsKeyUp(Keys.D) && kbs.IsKeyUp(Keys.S) && kbs.IsKeyUp(Keys.W) && playerState != PlayerState.AttackLeft && playerState != PlayerState.AttackRight && (leftStickInput == null || leftStickInput.Length() < deadZone))
            {
                //Correct the speedy animation to be slower if the player is idle
                timePerFrame = 100;

                if (playerState == PlayerState.WalkingLeft)
                {
                    playerState = PlayerState.IdleLeft;
                }
                else if (playerState == PlayerState.WalkingRight)
                {
                    playerState = PlayerState.IdleRight;
                }
            }

            if (currentFrame == 6 && playerState == PlayerState.AttackLeft)
            {
                timePerFrame = 100;
                playerState = PlayerState.IdleLeft;
            }

            if (currentFrame == 6 && playerState == PlayerState.AttackRight)
            {
                timePerFrame = 100;
                playerState = PlayerState.IdleRight;
            }

            if ((msState.LeftButton == ButtonState.Pressed || rightStickInput.Length() > deadZone))
            {
                CurrSkill.OnActivated();
            }

            //else if(CurrSkill.Name == "Swing" && gPadState.IsButtonDown(Buttons.A))
            //{
            //    CurrSkill.OnActivated();
            //}

            if (PosY > (roomHeight - PosRect.Height - 120))
            {
                PosY = roomHeight - PosRect.Height - 120;
            }

            if (PosY < 80)
            {
                PosY = 80;
            }

            if (PosX > (roomWidth - PosRect.Width - 80))
            {
                PosX = roomWidth - PosRect.Width - 80;
            }

            if (PosX < 80)
            {
                PosX = 80;
            }

            if (playerState == PlayerState.AttackLeft || playerState == PlayerState.IdleLeft || playerState == PlayerState.WalkingLeft)
            {
                HitBox = new Rectangle(PosX + 40, PosY + 30, PosRect.Width - 60, PosRect.Height - 40);
                Direction = "left";
            }

            else
            {
                HitBox = new Rectangle(PosX + 20, PosY + 30, PosRect.Width - 60, PosRect.Height - 40);
                Direction = "right";
            }

            //Weapon switching
            if((msState.RightButton == ButtonState.Pressed && prevMS.RightButton == ButtonState.Released) || (gPadState.IsButtonDown(Buttons.RightShoulder) && prevGPadState.IsButtonUp(Buttons.RightShoulder)))
            {
                if(CurrWeapon == "Sword")
                {
                    //Switch to the Daggers
                    CurrWeapon = weaponArray[1];
                    currSkillList = rogueSkillList;
                    CurrSkill = currSkillList[0];
                }

                else if(CurrWeapon == "Daggers")
                {
                    //Switch to the Bow
                    CurrWeapon = weaponArray[2];
                    currSkillList = rangerSkillList;
                    CurrSkill = currSkillList[0];
                }

                else if (CurrWeapon == "Bow")
                {
                    //Switch to the staff
                    CurrWeapon = weaponArray[3];
                    currSkillList = mageSkillList;
                    CurrSkill = currSkillList[0];
                }

                else if (CurrWeapon == "Staff")
                {
                    //Switch to the sword and restore defaults
                    CurrWeapon = weaponArray[0];
                    currSkillList = warriorSkillList;
                    CurrSkill = currSkillList[0];
                }
            }

            if(gPadState.IsButtonDown(Buttons.LeftShoulder) && prevGPadState.IsButtonUp(Buttons.LeftShoulder))
            {
                if(CurrWeapon == "Sword")
                {
                    //Switch to the staff
                    CurrWeapon = weaponArray[3];
                    currSkillList = mageSkillList;
                    CurrSkill = currSkillList[0];
                }

                else if(CurrWeapon == "Daggers")
                {
                    //Switch to the sword and restore defaults
                    CurrWeapon = weaponArray[0];
                    currSkillList = warriorSkillList;
                    CurrSkill = currSkillList[0];
                }

                else if (CurrWeapon == "Bow")
                {
                    //Switch to the Daggers
                    CurrWeapon = weaponArray[1];
                    currSkillList = rogueSkillList;
                    CurrSkill = currSkillList[0];
                }

                else if (CurrWeapon == "Staff")
                {
                    //Switch to the Bow
                    CurrWeapon = weaponArray[2];
                    currSkillList = rangerSkillList;
                    CurrSkill = currSkillList[0];
                }
            }

            //Switch to sword
            if (kbs.IsKeyDown(Keys.Z))
            {
                //If the weapon is not the one you are trying to switch to
                if(CurrWeapon != "Sword")
                {
                    //Switch to the weapon and restore defaults
                    CurrWeapon = weaponArray[0];
                    currSkillList = warriorSkillList;
                    CurrSkill = currSkillList[0];
                }
            }

            //Switch to Daggers
            if (kbs.IsKeyDown(Keys.X))
            {
                if(CurrWeapon != "Daggers")
                {
                    CurrWeapon = weaponArray[1];
                    currSkillList = rogueSkillList;
                    CurrSkill = currSkillList[0];
                }
            }

            //Switch to Bow
            if (kbs.IsKeyDown(Keys.C))
            {
                if(CurrWeapon != "Bow")
                {
                    CurrWeapon = weaponArray[2];
                    currSkillList = rangerSkillList;
                    CurrSkill = currSkillList[0];
                }
            }

            //Switch to Staff
            if (kbs.IsKeyDown(Keys.V))
            {
                if(CurrWeapon != "Staff")
                {
                    CurrWeapon = weaponArray[3];
                    currSkillList = mageSkillList;
                    CurrSkill = currSkillList[0];
                }
            }


            //Handling skill switching
            if (kbs.IsKeyDown(Keys.D1))
            {
                CurrSkill = currSkillList[0];
            }

            if (kbs.IsKeyDown(Keys.D2))
            {
                Skills prevSkill = currSkill;

                //Try to switch to the skill
                try
                {
                    CurrSkill = currSkillList[1];
                }

                //If there is no skill to switch to
                catch
                {
                    //Don't switch
                    currSkill = prevSkill;
                }
            }

            if (kbs.IsKeyDown(Keys.D3))
            {
                Skills prevSkill = currSkill;

                try
                {
                    CurrSkill = currSkillList[2];
                }
                catch
                {
                    currSkill = prevSkill;
                }
            }

            if (kbs.IsKeyDown(Keys.D4))
            {
                Skills prevSkill = currSkill;

                try
                {
                    CurrSkill = currSkillList[3];
                }
                catch
                {
                    currSkill = prevSkill;
                }
            }

            if (kbs.IsKeyDown(Keys.D5))
            {
                Skills prevSkill = currSkill;

                try
                {
                    CurrSkill = currSkillList[4];
                }
                catch
                {
                    currSkill = prevSkill;
                }
            }

            if (kbs.IsKeyDown(Keys.D6))
            {
                Skills prevSkill = currSkill;

                try
                {
                    CurrSkill = currSkillList[5];
                }
                catch
                {
                    currSkill = prevSkill;
                }
            }

            if (gPadState.Triggers.Right > .20f && prevGPadState.Triggers.Right <= .20f)
            {
                Skills prevSkill = currSkill;

                skillIndex += 1;

                try
                {
                    CurrSkill = currSkillList[skillIndex];
                }
                catch
                {
                    CurrSkill = currSkillList[0];
                    skillIndex = 0;
                }
            }

            if (gPadState.Triggers.Left > .20f && prevGPadState.Triggers.Left <= .20f)
            {
                Skills prevSkill = currSkill;

                skillIndex -= 1;

                try
                {
                    CurrSkill = currSkillList[skillIndex];
                }
                catch
                {
                    CurrSkill = currSkillList[currSkillList.Count - 1];
                    skillIndex = currSkillList.Count - 1;
                }
            }
        }

        //Added by Stasha
        //This method handles drawing the player using the correct animation; based on the player's current state
        public void Draw(SpriteBatch spritebatch, Texture2D texture, int frameWidth, int frameHeight)
        {
            Rectangle frame;

            //Look at the player's current state and choose the correct animation to play
            switch (playerState)
            {
                case PlayerState.IdleRight:

                    //Calculate the source rectangle for the frame
                    frame = new Rectangle(currentFrame * 140, 0, frameWidth, frameHeight);

                    //Draw the player using the frame and the position rectangle
                    spritebatch.Draw(texture, PosRect, frame, color);
                    break;

                case PlayerState.IdleLeft:

                    frame = new Rectangle(currentFrame * 140, 0, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case PlayerState.WalkingRight:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, color);
                    break;

                case PlayerState.WalkingLeft:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case PlayerState.AttackLeft:

                    frame = new Rectangle(currentFrame * 140, 280, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case PlayerState.AttackRight:

                    frame = new Rectangle(currentFrame * 140, 280, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, color);
                    break;
            }

            if(IsBlocking)
            {
                spritebatch.Draw(texture, HitBox, color);
            }
        }
        //This method handles updating the player's frame based on gametime
        public void UpdateFrame(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            framesElapsed = (int)(timeElapsed / timePerFrame);
            currentFrame = framesElapsed % numFrames + 1;
        }
        //End of things added by Stasha

        public void Update(int roomWidth, int roomHeight, Camera cam, GameTime gameTime)
        {
            base.Update(gameTime);

            manaTimer += gameTime.ElapsedGameTime.Milliseconds;
            healthTimer += gameTime.ElapsedGameTime.Milliseconds;
            costTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (CurrMana < TotalMana && manaTimer >= ManaRegenRate)
            {
                CurrMana += ManaRegen;
                manaTimer = 0;
            }

            if (CurrHealth < TotalHealth && healthTimer >= HealthRegenRate)
            {
                CurrHealth += HealthRegen;
                healthTimer = 0;
            }


            if(IsBlocking && CurrMana < 3)
            {
                IsBlocking = false;
            }

            if (IsBlocking && costTimer >= 300)
            {
                CurrMana -= 2;
                costTimer = 0;
            }

            if (Souls > SoulsNeeded)
            {
                LevelUp();
            }

            else if (CurrMana > TotalMana)
            {
                CurrMana = TotalMana;
            }

            percentMP = (float)CurrMana / (float)TotalMana;
            percentHP = (float)CurrHealth / (float)TotalHealth;

            //If the camera is not moving, process the player input
            if (!cam.isMoving)
            {
                ProcessInput(roomWidth, roomHeight);
            }

            //If the camera is moving, player input is not processed
            else if (cam.isMoving)
            {
                //Adjusting the player motion based on the camera movement direction
                switch (cam.direction)
                {
                    case "right":
                        if (cam.progress <= (1080 / 2))
                        {
                            PosX -= cam.panSpeed - 5;
                            HitBoxX -= cam.panSpeed - 5;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX -= cam.panSpeed;
                            HitBoxX -= cam.panSpeed;

                            playerState = PlayerState.IdleRight;
                        }
                        break;

                    case "left":
                        if (cam.progress <= (1080 / 2))
                        {
                            PosX += cam.panSpeed - 5;
                            HitBoxX += cam.panSpeed - 5;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX += cam.panSpeed;
                            HitBoxX += cam.panSpeed;
                            playerState = PlayerState.IdleLeft;
                        }
                        break;

                    case "down":
                        if (cam.progress <= (1920 / 2))
                        {
                            PosY -= cam.panSpeed - 3;
                            HitBoxY -= cam.panSpeed - 3;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY -= cam.panSpeed;
                            HitBoxY -= cam.panSpeed;

                            if (playerState == PlayerState.WalkingLeft)
                            {
                                playerState = PlayerState.IdleLeft;
                            }
                            else if (playerState == PlayerState.WalkingRight)
                            {
                                playerState = PlayerState.IdleRight;
                            }
                        }
                        break;

                    case "up":
                        if (cam.progress <= (1920 / 2))
                        {
                            PosY += cam.panSpeed - 3;
                            HitBoxY += cam.panSpeed - 3;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY += cam.panSpeed;
                            HitBoxY += cam.panSpeed;

                            if (playerState == PlayerState.WalkingLeft)
                            {
                                playerState = PlayerState.IdleLeft;
                            }
                            else if (playerState == PlayerState.WalkingRight)
                            {
                                playerState = PlayerState.IdleRight;
                            }
                        }
                        break;
                }
            }

            //Updating cooldowns
            foreach (Skills skill in warriorSkillList)
            {
                if (skill.Cooldown > 0)
                {
                    skill.Cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                }

                if (skill.Cooldown < 0)
                {
                    skill.Cooldown = 0;
                }
            }

            foreach (Skills skill in mageSkillList)
            {
                if (skill.Cooldown > 0)
                {
                    skill.Cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                }

                if (skill.Cooldown < 0)
                {
                    skill.Cooldown = 0;
                }
            }

            foreach (Skills skill in rogueSkillList)
            {
                if (skill.Cooldown > 0)
                {
                    skill.Cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                }

                if (skill.Cooldown < 0)
                {
                    skill.Cooldown = 0;
                }
            }

            foreach (Skills skill in rangerSkillList)
            {
                if (skill.Cooldown > 0)
                {
                    skill.Cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                }

                if (skill.Cooldown < 0)
                {
                    skill.Cooldown = 0;
                }
            }
        }

        private void LevelUp()
        {
            Souls -= SoulsNeeded;
            SoulsNeeded += Level^2 * 256;
            Level += 1;
            TotalHealth += 10;
            CurrHealth = TotalHealth;
            TotalMana += 10;
            CurrMana = TotalMana;
            SkillPoints += 1;
            Notification.Alert("Level Up!", Color.Purple, 60, false);
        }
    }
}