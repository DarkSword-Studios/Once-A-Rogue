using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class Ulmog:Enemy
    {
        public Ulmog(Player play, Camera camera, int x, int y, int width, int height, Texture2D tex) : base(tex, play, camera, x, y, width, height)
        {
            if(player.Level <= 10)
            {
                Level = 15;
            }

            else
            {
                Level = player.Level + 5;
            }

            ArmorLevel = 4 + Level * 4;
            StunResist = 100;
            MoveSpeedTotal = 0;
            MoveSpeed = MoveSpeedTotal;
            TotalHealth = 100 + Level * 10;
            CurrHealth = TotalHealth;
            Cooldown = 0;

            SkillList = new List<Skills>();

            SkillList.Add(new Whirlwind(2 * Level, this));
            SkillList.Add(new Charge(30 + 5 * Level, this));
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            base.Draw(spritebatch);
        }

        public void Update(Player play, GameTime gt)
        {
            base.Update(gt, play);
        }

        public override void OnDeath(Player play)
        {
            base.OnDeath(play);
        }
    }
}
