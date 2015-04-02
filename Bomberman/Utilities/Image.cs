using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.Utilities
{
    public class Image
    {
        private Texture2D image;
        public float Alpha = 1.0f;
        public Vector2 Postion = Vector2.Zero;
        public Color Color;

        public Image(Texture2D img)
        {
            image = img;
            Color = Color.White;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Postion, Color * Alpha);
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
