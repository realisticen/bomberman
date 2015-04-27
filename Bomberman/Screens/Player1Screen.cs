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
        private Texture2D mobSheet;
        private Texture2D explosionSheet;
        private List<Bomb> bombs = new List<Bomb>();
        private List<Monster> monsters;
        public List<Explosion> explosions = new List<Explosion>();

        public Player1Screen(ScreenManager owner, Map _map)
            : base(owner)
        {
            map = _map;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
            bombs.ForEach(bomb => bomb.Draw(spriteBatch));
            player.Draw(spriteBatch);
            monsters.ForEach(monster => monster.Draw(spriteBatch));
            explosions.ForEach(explosion => explosion.Draw(spriteBatch));
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
                if (player.CanPlaceBomb())
                {
                    bombs.Add(new Bomb(bombTexture, map.GetTileCenter(player.MapCollisionBox.Center.ToVector2()), player));
                    player.Bombs++;
                }
            }

            bombs.ForEach(bomb => bomb.Update());
            explosions.ForEach(explosion => explosion.Update());

            bombs.ForEach(delegate(Bomb bomb)
            {
                if (bomb.IsAlive) return;

                var rexts = map.MakeExplosion(bomb);
                explosions.Add(new Explosion(rexts.Item1, rexts.Item2, explosionSheet));
            });

            bombs.RemoveAll(bomb => bomb.IsAlive == false);
            explosions.RemoveAll(explosion => explosion.isAlive == false);

            bombs.ForEach(bomb => map.Colides(bomb));
            foreach (var bomb in bombs)
            {
                if (bomb.MapCollisionBox.Intersects(player.MapCollisionBox))
                {
                    if (bomb.IsSolid && !player.Teleported)
                    {
                        if (player.OldPosition.X != player.Position.X) // Če je pršu z leve/ desne
                        {
                            if (player.OldPosition.X > player.Position.X) // Če pride iz leve
                                player.Acceleration.X += (bomb.MapCollisionBox.Right - player.MapCollisionBox.X); //+ offSet;
                            else
                                player.Acceleration.X -= ((player.MapCollisionBox.Right) - bomb.MapCollisionBox.X); //+ offSet);
                        }
                        else if (player.OldPosition.Y != player.Position.Y)
                        {
                            if (player.OldPosition.Y > player.Position.Y) // Če pride iz uspod
                                player.Acceleration.Y += (bomb.MapCollisionBox.Bottom - player.MapCollisionBox.Y);
                            else
                                player.Acceleration.Y -= (player.MapCollisionBox.Bottom - bomb.MapCollisionBox.Y);
                        }
                        player.UpdatePos();
                        break;
                    }
                }
                else
                    bomb.IsSolid = true;
            }

            foreach (var explosion in explosions)
            {
                if (explosion.Collides(player.HitBox))
                    player.Kill();
            }

            for (int i = 0; i < monsters.Count; i++)
            {
                var monster = monsters[i];

                monster.Update();

                if (explosions.Exists(explosion => explosion.Collides(monster.HitBox)))
                {
                    monsters.RemoveAt(i);
                    i--;
                    // TODO: Če ni vč mobu si zmagu
                }

                map.Colides(monster);
                if (monster.HitBox.Intersects(player.HitBox))
                {
                    player.Kill();
                }

                foreach (var bomb in bombs)
                {
                    if (bomb.IsSolid && bomb.MapCollisionBox.Intersects(monster.MapCollisionBox))
                    {
                        if (monster.OldPosition.X != monster.Position.X) // Če je pršu z leve/ desne
                        {
                            if (monster.OldPosition.X > monster.Position.X) // Če pride iz leve
                                monster.Acceleration.X += (bomb.MapCollisionBox.Right - monster.MapCollisionBox.X); //+ offSet;
                            else
                                monster.Acceleration.X -= ((monster.MapCollisionBox.Right) - bomb.MapCollisionBox.X); //+ offSet);
                        }
                        else if (monster.OldPosition.Y != monster.Position.Y)
                        {
                            if (monster.OldPosition.Y > monster.Position.Y) // Če pride iz uspod
                                monster.Acceleration.Y += (bomb.MapCollisionBox.Bottom - monster.MapCollisionBox.Y);
                            else
                                monster.Acceleration.Y -= (monster.MapCollisionBox.Bottom - bomb.MapCollisionBox.Y);
                        }
                        monster.UpdatePos();
                        break;
                    }

                }
            }

            map.CheckPowers(player);
        }


        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            player = new Player(content.Load<Texture2D>("Game/player"), Color.White);
            bombTexture = content.Load<Texture2D>("Game/bomb");
            explosionSheet = content.Load<Texture2D>("Game/explosion");
            mobSheet = content.Load<Texture2D>("Game/mob");


            player.Position = map.GetSpawnLocation(0);
            player.Position.Y -= player.Height / 2;
            player.Position.X += player.Width / 4 - 4;

            map.LoadTileSet(content);
            monsters = map.GetMonsters(mobSheet);
        }
    }
}
