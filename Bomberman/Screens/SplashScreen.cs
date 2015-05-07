using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.Utilities;
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
            owner.game.cam.BackgroundColor = Color.White;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            image.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) || image.Alpha > 1.6f || Mouse.GetState().RightButton == ButtonState.Pressed)
                owner.ChangeScreen(new MenuScreen(owner));
            image.Alpha += 0.01f;
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            image = new Image(content.Load<Texture2D>("SplashScreen/solaLogo")) {Alpha = 0f};
            image.Postion = new Vector2((MainGame.VIRTUAL_RESOLUTION_WIDTH - image.GetWidth()) / 2, (MainGame.VIRTUAL_RESOLUTION_HEIGHT - image.GetHeight()) / 2);
        }
    }
}
