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
        private Texture2D bombTexture;
        private List<Bomb> bombs = new List<Bomb>(); 

        public Player1Screen(ScreenManager owner, Map _map) : base(owner)
        {
            map = _map;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
            player.Draw(spriteBatch);
            foreach (var bomb in bombs)
            {
                bomb.Draw(spriteBatch);
            }
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

            if (state.IsKeyDown(Keys.Space))
            {
                bombs.Add(new Bomb(bombTexture, player.Position, player));
            }
            bombs.ForEach(bomb => bomb.Update());
            bombs.RemoveAll(bomb => bomb.IsAlive == false);
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            player = new Player(content.Load<Texture2D>("Game/player"), Color.White);
            bombTexture = content.Load<Texture2D>("Game/bomb");

            player.Position = map.GetSpawnLocation(0);
            player.Position.Y -= player.Height / 2;
            player.Position.X += player.Width / 4 - 4;

            map.LoadTileSet(content);
        }
    }
}
