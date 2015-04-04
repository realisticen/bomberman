using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.GameStuff;
using Bomberman.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Screens
{
    class Player1Screen : Screen
    {
        private Player player;

        public Player1Screen(ScreenManager owner) : base(owner)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch);
        }

        private KeyboardState state;
        private const int speed = 2;
        public override void Update(GameTime gameTime)
        {
            state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))
                player.Move(Directons.UP);
            if (state.IsKeyDown(Keys.Down))
                player.Move(Directons.DOWN);
            if (state.IsKeyDown(Keys.Left))
                player.Move(Directons.LEFT);
            if (state.IsKeyDown(Keys.Right))
                player.Move(Directons.RIGHT);
            player.UpdatePos();
            player.Update();
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            player = new Player(content.Load<Texture2D>("Game/player"), Color.White);
        }
    }
}
