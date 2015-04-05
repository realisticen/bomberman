using Bomberman.BaseClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.Utilities
{
    public class ScreenManager
    {
        private Screen currentScreen;
        private ContentManager content;
        //public Vector2 Dimensions;
        public readonly MainGame game;

        public void ChangeScreen(Screen screen)
        {
            if(currentScreen != null)
                currentScreen.UnloadContent();

            currentScreen = screen;
            screen.LoadContent(content);
        }

        public ScreenManager(MainGame gam)
        {
            game = gam;
            content = game.Content;
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
