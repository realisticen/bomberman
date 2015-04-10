using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.Utilities.Examples.Classes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Utilities
{
    class ButtonManager
    {
        public List<ButtonImage> Buttons = new List<ButtonImage>(); 

        public void Update(ResolutionRenderer cam)
        {
            var state = Clickable.ApplayCamera(cam, Mouse.GetState());

            foreach (var button in Buttons)
            {
                button.Update(state);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in Buttons)
            {
                button.Draw(spriteBatch);
            }
        }
    }
}
