using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman
{
    public class ScreenManager
    {
        private Screen currentScreen;
        private ContentManager content;
        public Vector2 Dimensions;

        public void ChangeScreen(Screen screen)
        {
            if(currentScreen != null)
                currentScreen.UnloadContent();

            currentScreen = screen;
            screen.LoadContent(content);
        }

        public ScreenManager(ContentManager Content, Vector2 dimension)
        {
            content = Content;
            Dimensions = dimension;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }
    }
}
