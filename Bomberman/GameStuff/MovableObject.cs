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
        public Vector2 Position, Acceleration;
        protected Vector2 velocity;

        public void UpdatePos()
        {
            velocity += Acceleration;
            Position += velocity;
            Acceleration.Y = 0;
            Acceleration.X = 0;
        }
    }
}
