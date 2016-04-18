using System;
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
        List<Skills> mageSkillList;
        List<Skills> rogueSkillList;
        List<Skills> rangerSkillList;
        List<Skills> warriorSkillList;

        //String array of weapon choices
        string[] weaponArray;

        //Soul Count
        private int souls;

        public int Souls
        {
            get { return souls; }
            set { souls = value; }
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

        private int manaRegen;

        public int ManaRegen
        {
            get { return manaRegen; }
            set { manaRegen = value; }
        }

        //Keeps track of the current frame of animation for the player
        int timePerFrame = 50;
        int numFrames = 6;
        public int framesElapsed;
        public int timeElapsed;

        int timePassed;

        public enum PlayerState { IdleLeft, IdleRight, WalkingLeft, WalkingRight, AttackLeft, AttackRight };

        private PlayerState playerState;

        public PlayerState PlayerStates
        {
            get { return playerState; }
            set { playerState = value; }
        }

        //End of stuff added by Stasha

        public Player(int x, int y, int width, int height)
        {
            //Initializing the skill collections
            warriorSkillList = new List<Skills>();
            mageSkillList = new List<Skills>();
            rangerSkillList = new List<Skills>();
            rogueSkillList = new List<Skills>();

            //Adding skills to each
            warriorSkillList.Add(new MeleeAttack(this));
            mageSkillList.Add(new MeleeAttack(this));
            rogueSkillList.Add(new MeleeAttack(this));
            rangerSkillList.Add(new MeleeAttack(this));
            rangerSkillList.Add(new PiercingShot());


            mageSkillList.Add(new Fireball());

            //Initializing the weapon array
            weaponArray = new string[4];
            weaponArray[0] = "Sword";
            weaponArray[1] = "Daggers";
            weaponArray[2] = "Bow";
            weaponArray[3] = "Staff";

            //Setting default values
            playerState = PlayerState.IdleRight;
            MoveSpeedTotal = 5;
            MoveSpeed = 5;
            StunResist = 0;
            RootResist = 0;
            FireResist = 0;
            SnareResist = 0;
            PoisenResist = 0;
            TotalHealth = 100;
            TotalMana = 100;
            CurrMana = 100;
            CurrHealth = 100;
            ManaRegen = 2;
            timePassed = 0;
            PosRect = new Rectangle(x, y, width, height);
            currWeapon = weaponArray[0];
            currSkillList = warriorSkillList;
            CurrSkill = currSkillList[0];
            currentFrame = 0;
        }

        //Method for processing user input
        public void ProcessInput(int roomWidth, int roomHeight)
        {
            //Figuring out which key is down
            KeyboardState kbs = Keyboard.GetState();

            //Figuring out which mouse buttn is being pressed
            MouseState msState = Mouse.GetState();

            //Assume the player is moving, a special case exists to correct idle movement
            timePerFrame = 50;

            //Moving the player based on which key is down
            if (kbs.IsKeyDown(Keys.A))
            {
                PosX -= MoveSpeed;
                playerState = PlayerState.WalkingLeft;
                //direction = "left";
            }

            if (kbs.IsKeyDown(Keys.D))
            {
                PosX += MoveSpeed;
                playerState = PlayerState.WalkingRight;
                //direction = "right";
            }

            if (kbs.IsKeyDown(Keys.S))
            {
                PosY += MoveSpeed;

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

                if (playerState == PlayerState.IdleRight)
                {
                    playerState = PlayerState.WalkingRight;
                }
                else if (playerState == PlayerState.IdleLeft)
                {
                    playerState = PlayerState.WalkingLeft;
                }
            }

            if (kbs.IsKeyUp(Keys.A) && kbs.IsKeyUp(Keys.D) && kbs.IsKeyUp(Keys.S) && kbs.IsKeyUp(Keys.W) && playerState != PlayerState.AttackLeft && playerState != PlayerState.AttackRight)
            {
                //Correct speedy animation
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

            if (msState.LeftButton == ButtonState.Pressed)
            {
                CurrSkill.OnActivated(this);
            }

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

            if (kbs.IsKeyDown(Keys.Z))
            {
                CurrWeapon = weaponArray[0];
                currSkillList = warriorSkillList;
                CurrSkill = currSkillList[0];
            }

            if (kbs.IsKeyDown(Keys.X))
            {
                CurrWeapon = weaponArray[1];
                currSkillList = rogueSkillList;
                CurrSkill = currSkillList[0];
            }

            if (kbs.IsKeyDown(Keys.C))
            {
                CurrWeapon = weaponArray[2];
                currSkillList = rangerSkillList;
                CurrSkill = currSkillList[0];
            }

            if (kbs.IsKeyDown(Keys.V))
            {
                CurrWeapon = weaponArray[3];
                currSkillList = mageSkillList;
                CurrSkill = currSkillList[0];
            }

            if (kbs.IsKeyDown(Keys.D1))
            {
                CurrSkill = currSkillList[0];
            }

            if (kbs.IsKeyDown(Keys.D2))
            {
                Skills prevSkill = currSkill;

                try
                {
                    CurrSkill = currSkillList[1];
                }
                catch
                {
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
        }

        //Added by Stasha
        public void Draw(SpriteBatch spritebatch, Texture2D texture, int frameWidth, int frameHeight)
        {
            Rectangle frame;

            switch (playerState)
            {
                case PlayerState.IdleRight:

                    frame = new Rectangle(currentFrame * 140, 0, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White);
                    break;

                case PlayerState.IdleLeft:

                    frame = new Rectangle(currentFrame * 140, 0, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case PlayerState.WalkingRight:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White);
                    break;

                case PlayerState.WalkingLeft:

                    frame = new Rectangle(currentFrame * 140, 140, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case PlayerState.AttackLeft:

                    frame = new Rectangle(currentFrame * 140, 280, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                    break;

                case PlayerState.AttackRight:

                    frame = new Rectangle(currentFrame * 140, 280, frameWidth, frameHeight);
                    spritebatch.Draw(texture, PosRect, frame, Color.White);
                    break;
            }
        }

        public void UpdateFrame(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            framesElapsed = (int)(timeElapsed / timePerFrame);
            currentFrame = framesElapsed % numFrames + 1;

        }
        //End of things added by Stasha

        public void Update(int roomWidth, int roomHeight, Camera cam, GameTime gameTime)
        {
            base.Update();

            timePassed += gameTime.ElapsedGameTime.Milliseconds;

            if (CurrMana < TotalMana && timePassed >= 300)
            {
                CurrMana += ManaRegen;
                timePassed = 0;
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
                            PosX -= cam.panSpeed - 8;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX -= cam.panSpeed;
                            playerState = PlayerState.IdleRight;
                        }
                        break;

                    case "left":
                        if (cam.progress <= (1080 / 2))
                        {
                            PosX += cam.panSpeed - 8;
                        }

                        if (cam.progress > (1080 / 2))
                        {
                            PosX += cam.panSpeed;
                            playerState = PlayerState.IdleLeft;
                        }
                        break;

                    case "down":
                        if (cam.progress <= (1920 / 2))
                        {
                            PosY -= cam.panSpeed - 4;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY -= cam.panSpeed;
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
                            PosY += cam.panSpeed - 4;
                        }

                        if (cam.progress > (1920 / 2))
                        {
                            PosY += cam.panSpeed;
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
    }
}