using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class PiercingShot:Skills
    {
        public PiercingShot()
        {
            CooldownTotal = 3000;
            Cooldown = 0;
            RangeX = 15;
            RangeY = 15;
            BurstRadius = 0;
        }

        public override void OnActivated(Player player)
        {
            if (Cooldown == 0)
            {
                MouseState ms = Mouse.GetState();

                base.OnActivated(player);

                if (player.PlayerStates == Player.PlayerState.AttackLeft)
                {
                    Vector2 target = new Vector2(ms.X, ms.Y) - new Vector2(player.PosX - 10, player.PosY + player.PosRect.Height / 2);
                    if (target != Vector2.Zero)
                    {
                        target.Normalize();
                    }

                    Game1.CurrProjectiles.Add(new Projectile(target, 1, 7, 40, 40, player.PosX - 10, player.PosY + player.PosRect.Height / 2));
                }

                else
                {
                    Vector2 target = new Vector2(ms.X, ms.Y) - new Vector2(player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2);
                    if (target != Vector2.Zero)
                    {
                        target.Normalize();
                    }

                    Game1.CurrProjectiles.Add(new Projectile(target, 1, 7, 40, 40, player.PosX + player.PosRect.Width + 10, player.PosY + player.PosRect.Height / 2));
                }
            }
        }
    }
}