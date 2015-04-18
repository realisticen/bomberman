using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.GameStuff
{
    class Player : MovableObject
    {
        public Color PlayerColor;
        public Rectangle HitBox;

        private int maxBombs = 1;
        public int Bombs = 0;
        public int BombSize = 1;

        private Texture2D spriteSheet;
        private Rectangle sourceRectangle, destinationRectangle;
        private Directons lastdDirecton;
        private float speed;

        public void BombDestroyed()
        {
            Bombs--;
        }

        public void Move(Directons direction)
        {
            switch (direction)
            {
                case Directons.UP:
                    Acceleration.Y -= speed;
                    break;
                case Directons.DOWN:
                    Acceleration.Y += speed;
                    break;
                case Directons.LEFT:
                    Acceleration.X -= speed;
                    break;
                case Directons.RIGHT:
                    Acceleration.X += speed;
                    break;
            }
        }

        public bool CanPlaceBomb()
        {
            return true;
            // TODO: Tole uzgor je sm za testirane
            if (Bombs < maxBombs)
                return true;
            return false;
        }

        public Player(Texture2D sprites, Color color)
        {
            spriteSheet = sprites;
            speed = 5;
            PlayerColor = color;
            Position = new Vector2(600,540);

            Width = 48;
            Height = 86;
            sourceRectangle = new Rectangle(0,0,48,86);
            //MapCollisionBox = new Rectangle((int)Position.X, (int)Position.Y + 56, 30, 20);
        }

        private bool update;
        private void NextFrame()
        {
            if (!update)
            {
                update = true;
                return;
            }
            update = false;

            if (sourceRectangle.X + 48 >= spriteSheet.Width)
                sourceRectangle.X = 0;
            else
                sourceRectangle.X += 48;
        }

        public void Update()
        {
            velocity *= 0;
            UpdatePos();
            //MapCollisionBox = new Rectangle((int)Position.X + 12, (int)Position.Y + 65, 25, 21);
            if (velocity.Y < 0)
            {
                if (lastdDirecton == Directons.UP)
                {
                    NextFrame();
                }
                else
                {
                    sourceRectangle.X = 0;
                    sourceRectangle.Y = 0;
                }

                lastdDirecton = Directons.UP;
            }

            if (velocity.Y > 0)
            {
                if (lastdDirecton == Directons.DOWN)
                {
                    NextFrame();
                }
                else
                {
                    sourceRectangle.X = 0;
                    sourceRectangle.Y = 86;
                }

                lastdDirecton = Directons.DOWN;
            }

            if (velocity.X < 0)
            {
                if (lastdDirecton == Directons.LEFT)
                {
                    NextFrame();
                }
                else
                {
                    sourceRectangle.X = 0;
                    sourceRectangle.Y = 258;
                }

                lastdDirecton = Directons.LEFT;
            }

            if (velocity.X > 0)
            {
                if (lastdDirecton == Directons.RIGHT)
                {
                    NextFrame();
                }
                else
                {
                    sourceRectangle.X = 0;
                    sourceRectangle.Y = 172;
                }

                lastdDirecton = Directons.RIGHT;
            }
            HitBox = destinationRectangle;
            MapCollisionBox = new Rectangle((int)Position.X + 6, (int)Position.Y + 65, 36, 21);
            velocity *= 0;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Rectangle((int)Position.X, (int)Position.Y, 48, 86), sourceRectangle, PlayerColor);

            //spriteBatch.Draw(spriteSheet, MapCollisionBox, Color.Red); // Začasno, da se vidi collisionBox...
        }
    }
}
