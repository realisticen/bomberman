using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Utilities.Examples.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.BaseClass
{
    
    public class Clickable
    {
        public Rectangle ClickArea;

        public delegate void MouseEventHandler(object obj);

        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseLeave;
        public event MouseEventHandler MouseOver;

        private bool entered = false;
        private MouseState old;

        public Clickable()
        {
            old = Mouse.GetState();
        }

        public void Update(MouseState mouseState)
        {
            if (mouseState.Y > ClickArea.Y && mouseState.Y < ClickArea.Y + ClickArea.Height
                && mouseState.X > ClickArea.X && mouseState.X < ClickArea.X + ClickArea.Width)
            {
                if (!entered && MouseEnter != null)
                    MouseEnter(this);
                if (MouseOver != null)
                    MouseOver(this);
                if (MouseClick != null && mouseState.LeftButton == ButtonState.Pressed && old.LeftButton == ButtonState.Released)
                    MouseClick(this);

                entered = true;

            }
            else
            {
                if (entered && MouseLeave != null)
                {
                    MouseLeave(this);
                    entered = false;
                }
            }

            old = mouseState;
        }

        public static MouseState ApplayCamera(ResolutionRenderer cam, MouseState mstate)
        {
            var point = new Point(mstate.X, mstate.Y);
            point = cam.ToVirtual(point);
            return new MouseState(point.X, point.Y, mstate.ScrollWheelValue, mstate.LeftButton, mstate.MiddleButton, mstate.RightButton, mstate.XButton1, mstate.XButton2);
        }
    }
}
