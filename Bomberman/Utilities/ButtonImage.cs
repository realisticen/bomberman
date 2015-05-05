using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.Utilities
{
    class ButtonImage : Clickable
    {
        public Image Image { get; private set; }
        public bool Enabled = true;

        public ButtonImage(Image img)
        {
            Image = img;
            UpdateClickArea();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(Enabled)
                Image.Draw(spriteBatch);
        }

        private void UpdateClickArea()
        {
            ClickArea = new Rectangle((int)Image.Postion.X, (int)Image.Postion.Y, Image.GetWidth(), Image.GetHeight());            
        }

        public void SetPosition(int x, int y)
        {
            Image.Postion.X = x;
            Image.Postion.Y = y;
            UpdateClickArea();
        }
    }
}
