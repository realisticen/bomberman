using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.GameStuff
{
    class Monster : MovableObject
    {
        public Rectangle HitBox;

        public static SoundEffect DeathSoundEffect;

        private Vector2 uPos;
        private Texture2D spriteSheet;
        private Rectangle sourceRectangle, destinationRectangle;
        private Directons lastdDirecton, direction;
        private int ticks, timeToChange;
        private float speed;


        public void Move(Directons direction)
        {
            Acceleration *= 0;
            switch (direction)
            {
                case Directons.UP:
                    if(this.direction == Directons.RIGHT)
                        Position.X -= 10;
                    Acceleration.Y -= speed;
                    break;
                case Directons.DOWN:
                    if (this.direction == Directons.RIGHT)
                        Position.X -= 10;
                    Acceleration.Y += speed;
                    break;
                case Directons.LEFT:
                    Acceleration.X -= speed;
                    break;
                case Directons.RIGHT:
                    Acceleration.X += speed;
                    break;
            }
            this.direction = direction;
        }

        public Monster(Texture2D sprites)
        {
            spriteSheet = sprites;
            speed = 2;
            Position = new Vector2(600,540);

            ticks = 0;
            timeToChange = MainGame.random.Next(30, 100);
            Width = 49;
            Height = 49;
            sourceRectangle = new Rectangle(0, 0, Width, Height);
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
            if(lastdDirecton == Directons.UP || lastdDirecton == Directons.DOWN)
            {
                if (sourceRectangle.X + Width >= spriteSheet.Width)
                    sourceRectangle.X = 0;
                sourceRectangle.X += 49;
            }
            else
            {
                if (sourceRectangle.X + Width > 274)
                    sourceRectangle.X = 0;
                sourceRectangle.X += 39;
            }
        }

        public void Update()
        {
            ticks ++;
            if (ticks == timeToChange)
            {
                ticks = 0;
                Move((Directons)MainGame.random.Next(1, 5));
                timeToChange = MainGame.random.Next(30, 100);
            }

            velocity *= 0;

            if (uPos != Position)
            {
                ticks = 0;
                Move((Directons)MainGame.random.Next(1, 5));
            }
            else
                Move(direction);

            UpdatePos();
            uPos = Position;
            if (velocity.Y < 0)
            {
                if (lastdDirecton == Directons.UP)
                {
                    NextFrame();
                }
                else
                {
                    sourceRectangle.X = 0;
                    sourceRectangle.Width = 49;
                    sourceRectangle.Y = 49;
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
                    sourceRectangle.Width = 49;
                    sourceRectangle.X = 0;
                    sourceRectangle.Y = 0;
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
                    sourceRectangle.Width = 39;
                    sourceRectangle.X = 0;
                    sourceRectangle.Y = 146;
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
                    sourceRectangle.Width = 39;
                    sourceRectangle.X = 0;
                    sourceRectangle.Y = 98;
                }

                lastdDirecton = Directons.RIGHT;
            }
            HitBox = destinationRectangle;
            //MapCollisionBox = new Rectangle((int)Position.X + 6, (int)Position.Y + 65, 36, 21);
            MapCollisionBox = new Rectangle((int)Position.X, (int)Position.Y, sourceRectangle.Width, Height);
            HitBox = MapCollisionBox;
            velocity *= 0;

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Rectangle((int)Position.X, (int)Position.Y, sourceRectangle.Width, Height), sourceRectangle, Color.White);

            //spriteBatch.Draw(spriteSheet, MapCollisionBox, Color.Red); // Začasno, da se vidi collisionBox...
        }

        internal void Kill()
        {
            DeathSoundEffect.Play(Settings.SoundEffectsVolume, 0, 0);
        }
    }
}
