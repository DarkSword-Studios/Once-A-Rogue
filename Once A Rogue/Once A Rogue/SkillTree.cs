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

        public Button RootButton
        {
            get { return rootButton; }
            set { rootButton = value; }
        }

        public SkillTree(Button butt)
        {
            layerDepth = new Dictionary<int, int>();
            rootButton = butt;
        }

        public Button Insert(string s, Button parent, Player player, Texture2D texture)
        {
            Button button = new Button(parent, player, s, texture);
            parent.Children.Add(button);
            layerDepth.Clear();
            CalcDepth(rootButton, 0);
            return button;
        }

        public Button Insert(string s, List<Button> parents, Player player, Texture2D texture)
        {
            Button button = new Button(parents, player, s, texture);
            foreach(Button parent in parents)
            {
                parent.Children.Add(button);
            }
            layerDepth.Clear();
            CalcDepth(rootButton, 0);
            return button;
        }

        public void UpdateButtons(Rectangle mouse)
        {
            UpdateButtons(mouse, rootButton);
        }

        private void UpdateButtons(Rectangle mouse, Button button)
        {
            if (mouse.Intersects(button.PosRect))
            {
                button.Purchase();
                return;
            }

            foreach (Button child in button.Children)
            {
                UpdateButtons(mouse, child);
            }
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
