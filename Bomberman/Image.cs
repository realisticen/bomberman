using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman
{
    public class Image
    {
        private Texture2D image;
        public float Alpha = 1.0f;
        public Vector2 Postion = Vector2.Zero;

        public Image(Texture2D img)
        {
            image = img;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Postion, Color.White * Alpha);
        }

        public int GetWidth()
        {
            return image.Width;
        }

        public int GetHeight()
        {
            return image.Height;
        }

    }
}
