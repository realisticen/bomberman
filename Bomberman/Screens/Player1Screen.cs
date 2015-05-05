using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.GameStuff;
using Bomberman.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;

namespace Bomberman.Screens
{
    class Player1Screen : Screen
    {
        private Player player;
        private Map map, backup_map;
        private Texture2D bombTexture, mobSheet, explosionSheet, endGameTexture2D;
        private List<Bomb> bombs = new List<Bomb>();
        private List<Monster> monsters;
        public List<Explosion> explosions = new List<Explosion>();

        private ButtonManager buttonManager;

        public Player1Screen(ScreenManager owner, Map _map)
            : base(owner)
        {
            map = _map;
            var json = JsonConvert.SerializeObject(map);
            backup_map = JsonConvert.DeserializeObject<Map>(json);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
            bombs.ForEach(bomb => bomb.Draw(spriteBatch));
            player.Draw(spriteBatch);
            monsters.ForEach(monster => monster.Draw(spriteBatch));
            explosions.ForEach(explosion => explosion.Draw(spriteBatch));

            if (endGame)
            {
                spriteBatch.Draw(endGameTexture2D, new Vector2(288,299), Color.White);
                buttonManager.Draw(spriteBatch);
            }
        }

        private KeyboardState state;
        public override void Update(GameTime gameTime)
        {
            if (endGame)
            {
                buttonManager.Update(owner.game.cam);
                return;
            }

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
                    player.onBomb = true;
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
                                player.Acceleration.X += (bomb.MapCollisionBox.Right - player.MapCollisionBox.X);
                                    //+ offSet;
                            else
                                player.Acceleration.X -= ((player.MapCollisionBox.Right) - bomb.MapCollisionBox.X);
                                    //+ offSet);
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
                {
                    if(bomb == bombs.Last())
                        player.onBomb = false;
                    bomb.IsSolid = true;
                }
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
                    monster.Kill();
                    monsters.RemoveAt(i);
                    i--;
                    if (monsters.Count == 0)
                    {
                        playerWin = endGame = true;
                        endGameTexture2D = content.Load<Texture2D>("Game/win");
                    }
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


        private bool playerWin = false, endGame = false;
        private void Gameover()
        {
            buttonManager = new ButtonManager();
            var image = content.Load<Texture2D>("Game/menu");
            var button = new ButtonImage(new Image(image));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.MouseClick += new Clickable.MouseEventHandler(GoToMenu);
            button.SetPosition(360, 500);
            buttonManager.Buttons.Add(button);

            image = content.Load<Texture2D>("Game/retry");
            button = new ButtonImage(new Image(image));
            button.MouseClick += new Clickable.MouseEventHandler(Retry);
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(620, 500);
            buttonManager.Buttons.Add(button);

            image = content.Load<Texture2D>("Game/map");
            button = new ButtonImage(new Image(image));
            button.MouseClick += new Clickable.MouseEventHandler(GoToMapPick);
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(800, 500);
            buttonManager.Buttons.Add(button);

            endGameTexture2D = content.Load<Texture2D>("Game/lost");
            endGame = true;
        }
        private void Retry(object button)
        {
            owner.ChangeScreen(new Player1Screen(owner, backup_map));
        }

        private void GoToMapPick(object button)
        {
            owner.ChangeScreen(new MapPickScreen(owner, MapPickScreen.GameType.Player_1));
        }

        private void GoToMenu(object button)
        {
            owner.ChangeScreen(new MenuScreen(owner));
        }


        private void Clear(object button)
        {
            var but = button as ButtonImage;
            but.Image.Color = Color.White;
        }

        private void Hover(object button)
        {
            var but = button as ButtonImage;
            but.Image.Color = Color.Turquoise;
        }

        private SoundEffectInstance backgroundMusic;
        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            player = new Player(content.Load<Texture2D>("Game/player"), Color.White);
            bombTexture = content.Load<Texture2D>("Game/bomb");
            explosionSheet = content.Load<Texture2D>("Game/explosion");
            mobSheet = content.Load<Texture2D>("Game/mob");

            Player.DeathSoundEffect = content.Load<SoundEffect>("Game/Sound/death");
            Monster.DeathSoundEffect = content.Load<SoundEffect>("Game/Sound/mob_death");
            Bomb.ExplosionSoundEffect = content.Load<SoundEffect>("Game/Sound/explosion");

            player.Position = map.GetSpawnLocation(0);
            player.Position.Y -= player.Height / 2;
            player.Position.X += player.Width / 4 - 4;
            player.Death += new Player.PlayerDeathEventHandler(Gameover);

            map.LoadTileSet(content);
            monsters = map.GetMonsters(mobSheet);

            backgroundMusic = content.Load<SoundEffect>("Game/Sound/player1music.wav").CreateInstance();
            backgroundMusic.Volume = 0.5f;
            backgroundMusic.IsLooped = true;
            backgroundMusic.Play();
        }
    }
}
