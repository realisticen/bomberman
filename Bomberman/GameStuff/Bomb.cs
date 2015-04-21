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

        public Player Owner;
        public bool IsAlive = true, IsSolid = false;
        private int life = 120;
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

            Owner = _owner;
            Width = 48;
            Height = 48;
            Position.X -= Width / 2;
            Position.Y -= Height / 2;
            sourceRectangle = new Rectangle(0, 0, Width, Height);
            MapCollisionBox = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public void Update()
        {
            velocity *= 0;

            if (life < 0)
            {
                IsAlive = false;
                Owner.BombDestroyed();
                return;
            }
            life--;

            NextFrame();
            UpdatePos();
            MapCollisionBox = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            velocity *= 0;

        }

        private int update;
        private void NextFrame()
        {
            if (update != 20)
            {
                update++;
                return;
            }
            update = 0;

            if(BombColor != Color.Red)
                BombColor = Color.Red;
            else
                BombColor = Color.White;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), sourceRectangle, BombColor);
            //spriteBatch.Draw(spriteSheet, MapCollisionBox, Color.Red); // Začasno, da se vidi collisionBox...
            
        }
    }
}
