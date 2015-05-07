using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.GameStuff
{
    class Explosion
    {
        private Texture2D sprteSheetFire;
        private int timeToLive = 40, offSet, verticalSize, horizontalSize; // 60 == 1 s
        private Rectangle verticalExplosion, horizontalExplosion, sourceRectangle, vHitBox, hHitbox;

        public bool isAlive = true;
        private const int fireSize = 48, frameTime = 40 / 5;

        public Explosion(Rectangle _verticalExplosion, Rectangle _horizontalExplosion, Texture2D sprite)
        {
            sprteSheetFire = sprite;
            verticalExplosion = _verticalExplosion;
            horizontalExplosion = _horizontalExplosion;
            sourceRectangle = new Rectangle(0,0, fireSize, fireSize);

            verticalSize = verticalExplosion.Height/verticalExplosion.Width;
            horizontalSize = horizontalExplosion.Width/horizontalExplosion.Height;
            offSet = (horizontalExplosion.Height - fireSize)/2;

            vHitBox = verticalExplosion;
            vHitBox.Y += 14;
            vHitBox.X += 14;
            vHitBox.Width -= 28;
            vHitBox.Height -= 28;

            hHitbox = horizontalExplosion;
            hHitbox.Y += 14;
            hHitbox.X += 14;
            hHitbox.Width -= 28;
            hHitbox.Height -= 28;
        }

        public bool Collides(Rectangle hitbox)
        {
            if (vHitBox.Intersects(hitbox) || hHitbox.Intersects(hitbox))
                return true;
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < horizontalSize; i++)
            {
                spriteBatch.Draw(sprteSheetFire, new Vector2(horizontalExplosion.X + (i * horizontalExplosion.Height) + offSet, horizontalExplosion.Y + offSet), sourceRectangle, Color.White);
            }
            for (int i = 0; i < verticalSize; i++)
            {
                spriteBatch.Draw(sprteSheetFire, new Vector2(verticalExplosion.X + offSet, verticalExplosion.Y + ( i * verticalExplosion.Width) + offSet), sourceRectangle, Color.White);
                
            }
            //spriteBatch.Draw(sprteSheetFire, verticalExplosion, sprteSheetFire.Bounds, Color.White);
            //spriteBatch.Draw(sprteSheetFire, horizontalExplosion, sprteSheetFire.Bounds, Color.White);
        }

        private int update = 0;
        public void Update()
        {
            if (timeToLive < 0)
                isAlive = false;
            timeToLive--;

            if (update > frameTime)
            {
                sourceRectangle.X += fireSize; // TODO: Ured, da bo šlo od leve prot desn..
                update = 0;
            }
            update++;
        }
    }
}
