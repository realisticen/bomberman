using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.GameStuff
{
    class Bomb : MovableObject
    {
        public Color BombColor;

        private Texture2D spriteSheet;
        private Rectangle sourceRectangle, destinationRectangle;

        private Player owner;
        public bool IsAlive = true;
        private int life = 60;
        private float speed;
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

        public Bomb(Texture2D sprites, Vector2 position, Player _owner)
        {
            spriteSheet = sprites;
            speed = 5;
            BombColor = Color.White;
            Position = position;

            owner = _owner;
            Width = 48;
            Height = 48;
            sourceRectangle = new Rectangle(0,0, Height, Width);
        }

        public void Update()
        {
            if (life < -100)
            {
                IsAlive = false;
                owner.BombDestroyed();
                return;
            }
            life--;
            NextFrame();
            UpdatePos();
            MapCollisionBox = new Rectangle((int)Position.X, (int)Position.Y, Height, Width);
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

            if(BombColor != Color.Red)
                BombColor = Color.Red;
            else
                BombColor = Color.White;
            //if (sourceRectangle.X + Width >= spriteSheet.Width)
            //    sourceRectangle.X = 0;
            //else
            //    sourceRectangle.X += Width;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), sourceRectangle, BombColor);
        }
    }
}
