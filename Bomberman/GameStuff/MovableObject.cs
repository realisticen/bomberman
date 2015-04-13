using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bomberman.GameStuff
{
    enum Directons
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    class MovableObject
    {
        public Vector2 Position, Acceleration, OldPosition;
        protected Vector2 velocity;
        public Rectangle MapCollisionBox;

        public bool Teleported = false;

        public int Width;
        public int Height;

        private short counter = 0;

        public void UpdatePos()
        {
            if(OldPosition != Position)
                OldPosition = Position;

            if (Teleported)
                counter++;
            if (counter > 80)
            {
                counter = 0;
                Teleported = false;
            }

            velocity += Acceleration;
            Position += velocity;
            Acceleration.Y = 0;
            Acceleration.X = 0;
        }
    }
}
