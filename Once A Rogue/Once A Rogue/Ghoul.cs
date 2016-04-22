using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection; //Needed for string to variable calls
using System.IO; //Needed for finding rooms
using System.Collections.Generic; //Needed for lists
using System; //Needed for random objects
using Microsoft.Xna.Framework.Media; //Needed for music
using System.Threading;

namespace Once_A_Rogue
{
    class Ghoul : Enemy
    {
        Random rand = new Random();
        int ghoulSouls = 0;

        public int GhoulSouls
        {
            get { return ghoulSouls; }
            set { ghoulSouls = value; }
        }
        public Ghoul(Player play, Camera camera, int ghSouls, int x, int y, int width, int height, Texture2D tex) : base(tex, play, camera, x, y, width, height)
        {

            SkillList = new List<Skills>();
            

            MoveSpeedTotal = 6;
            MoveSpeed = 6;
            ghoulSouls = ghSouls;
            Random randy = new Random();
            Level = randy.Next(-2, 2) + play.Level;
            ArmorLevel = 2 + Level;
            FearLevelTotal = 0;
            FearLevel = 0;
            TotalHealth = 30 + Level * 5;
            CurrHealth = TotalHealth;
        }

        public void SiphonSoul(Player pay)
        {
            int chance = rand.Next(0, 11);
            if (chance >= 7 || chance <= 9)
            {
                if (pay.CurrHealth <= pay.TotalHealth / 4)
                {
                    pay.Souls = pay.Souls - 4;
                    pay.CurrHealth = pay.CurrHealth - pay.TotalHealth / 10;
                    ghoulSouls++;
                }
            }

            if (chance == 10)
            {
                pay.CurrHealth = pay.CurrHealth - pay.TotalHealth / 5;
            }

        }

        protected override void OnDeath(Player pay)
        {
            base.OnDeath();

           
            pay.Souls = pay.Souls + ghoulSouls;
            pay.Souls++;
            
        }

        public void BasicAttack(Player play)
        {
            play.CurrHealth = play.CurrHealth - 20;
            //Add code for attack animation
        }

        public override void Update()
        {
            base.Update();
        }

        public void Update(Player play, GameTime gt)
        {
            base.Update();




        }
    }
}
