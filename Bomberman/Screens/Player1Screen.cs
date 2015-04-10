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
        private Map map;

        public Player1Screen(ScreenManager owner, Map _map) : base(owner)
        {
            map = _map;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);
        }

        private KeyboardState state;
        public override void Update(GameTime gameTime)
        {
            state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))
                player.Move(Directons.UP);
            else if (state.IsKeyDown(Keys.Down))
                player.Move(Directons.DOWN);
            else if (state.IsKeyDown(Keys.Left))
                player.Move(Directons.LEFT);
            else if (state.IsKeyDown(Keys.Right))
                player.Move(Directons.RIGHT);

            player.Update();

            map.Colides(player);
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            player = new Player(content.Load<Texture2D>("Game/player"), Color.White);
            map.LoadTileSet(content);
        }
    }
}
