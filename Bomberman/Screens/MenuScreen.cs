using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Screens
{
    class MenuScreen : Screen
    {
        private Texture2D image;

        public MenuScreen(ScreenManager owner) : base( owner)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Vector2.Zero, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                owner.ChangeScreen(new SplashScreen(owner));
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            image = content.Load<Texture2D>("MenuScreen/title");
        }
    }
}
