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
    class SplashScreen : Screen
    {
        private Image image;

        public SplashScreen(ScreenManager owner) : base( owner)
        {
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) || image.Alpha > 1.6f)
                owner.ChangeScreen(new MenuScreen(owner));
            image.Alpha += 0.01f;
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            image = new Image(content.Load<Texture2D>("SplashScreen/solaLogo")) {Alpha = 0f};
            image.Postion = new Vector2((owner.Dimensions.X - image.GetWidth()) / 2, (owner.Dimensions.Y - image.GetHeight()) / 2);
        }
    }
}
