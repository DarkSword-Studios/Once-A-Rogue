using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //Needed for drawing tiles
using Microsoft.Xna.Framework;

namespace Once_A_Rogue
{
    class SkillTree
    {
        private Button rootButton;

        private Dictionary<int, int> layerDepth;

        List<Button> buyableButtons;

        public Button RootButton
        {
            get { return rootButton; }
            set { rootButton = value; }
        }

        public SkillTree(Button butt)
        {
            buyableButtons = new List<Button>();
            layerDepth = new Dictionary<int, int>();
            rootButton = butt;
        }

        public Button Insert(string description, string tag, int mod, Button parent, Player player, Texture2D texture)
        {
            Button button = new Button(parent, player, description, tag, mod, texture);
            parent.Children.Add(button);
            CompileList();
            return button;
        }

        public Boolean UpdateButtons(Player player, Rectangle mouse)
        {
            return UpdateButtons(player, mouse, rootButton);
        }

        private Boolean UpdateButtons(Player player, Rectangle mouse, Button button)
        {
            if (mouse.Intersects(button.PosRect) && !button.isBought)
            {
                if(player.SkillPoints > 0)
                {
                    player.SkillPoints--;
                    button.Purchase();
                    CompileList();
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

            foreach (Button child in button.Children)
            {
                if(UpdateButtons(player, mouse, child))
                {
                    return true;
                }
            }
            return false;
        }

        public void DrawTree(SpriteBatch spriteBatch)
        {
            DrawButton(spriteBatch, rootButton, 0, 0);
        }

        private void DrawButton(SpriteBatch spriteBatch, Button button, int depth, int xOffset)
        {
            button.PosRect = new Rectangle(900 + xOffset, 900 - depth * (button.Texture.Height + 20), button.Texture.Width, button.Texture.Height);


            if (button.isBought)
            {
                spriteBatch.Draw(button.Texture, button.PosRect, Color.Green);
            }
            else
            {
                spriteBatch.Draw(button.Texture, button.PosRect, Color.Red);
            }          

            foreach(Button child in button.Children)
            {
                DrawButton(spriteBatch, child, depth + 1, xOffset);
                xOffset += button.Texture.Width + 20;
            }
        }

        private void CompileList()
        {
            buyableButtons.Clear();
            CompileList(rootButton);
        }

        private void CompileList(Button button)
        {
            Boolean allBought = true;
            foreach (Button parent in button.Parents)
            {
                if (parent != null && !parent.isBought)
                {
                    allBought = false;
                }
            }

            if (allBought && !button.isBought)
            {
                buyableButtons.Add(button);
            }

            foreach (Button child in button.Children)
            {
                CompileList(child);
            }
        }

        public void DrawButtons(SpriteBatch spriteBatch, SpriteFont font, int buttonsPerRow, int startX, int startY)
        {
            int rowEntry = 0;
            int row = 0;

            for(int i = 0; i < buyableButtons.Count; i++)
            {
                buyableButtons[i].PosRect = new Rectangle(startX + rowEntry * (buyableButtons[i].Texture.Width + 20), startY + row * (buyableButtons[i].Texture.Height + 20), buyableButtons[i].Texture.Width, buyableButtons[i].Texture.Height);
                spriteBatch.Draw(buyableButtons[i].Texture, buyableButtons[i].PosRect, Color.White);
                Vector2 stringPos = new Vector2(buyableButtons[i].PosRect.X, buyableButtons[i].PosRect.Y + (buyableButtons[i].Texture.Height / 2));
                spriteBatch.DrawString(font, buyableButtons[i].description, stringPos, Color.Black, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 1);

                if(rowEntry == buttonsPerRow - 1)
                {
                    row++;
                    rowEntry = 0;
                }
                else
                {
                    rowEntry++;
                }
            }
        }

        private void CalcDepth(Button button, int depth)
        {
            if (layerDepth.ContainsKey(depth))
            {
                layerDepth[depth]++;
            }
            else
            {
                layerDepth[depth] = 1;
            }
                    

            foreach (Button child in button.Children)
            {
                CalcDepth(child, depth + 1);
            }
        }
    }
}
