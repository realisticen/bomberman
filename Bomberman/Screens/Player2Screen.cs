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
using Newtonsoft.Json;

namespace Bomberman.Screens
{
    class Player2Screen : Screen
    {

        
        private Player player, player2;
        private Map map, backup_map;
        private Texture2D bombTexture, explosionSheet, endGameTexture2D;
        private List<Bomb> bombs = new List<Bomb>();
        public List<Explosion> explosions = new List<Explosion>();

        private ButtonManager buttonManager;

        public Player2Screen(ScreenManager owner, Map _map)
            : base(owner)
        {
            owner.game.cam.BackgroundColor = new Color(19, 20, 38);
            map = _map;
            owner.game.IsMouseVisible = false;
            var json = JsonConvert.SerializeObject(map);
            backup_map = JsonConvert.DeserializeObject<Map>(json);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
            bombs.ForEach(bomb => bomb.Draw(spriteBatch));
            player.Draw(spriteBatch);
            player2.Draw(spriteBatch);
            explosions.ForEach(explosion => explosion.Draw(spriteBatch));

            if (endGame)
            {
                spriteBatch.Draw(endGameTexture2D, new Vector2(288,299), Color.White);
                buttonManager.Draw(spriteBatch);
            }
        }

        private KeyboardState state;
        private bool p1win = true;
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

            if (state.IsKeyDown(Keys.W))
                player2.Move(Directons.UP);
            else if (state.IsKeyDown(Keys.S))
                player2.Move(Directons.DOWN);
            else if (state.IsKeyDown(Keys.A))
                player2.Move(Directons.LEFT);
            else if (state.IsKeyDown(Keys.D))
                player2.Move(Directons.RIGHT);

            player.Update();
            player2.Update();
            map.Colides(player);
            map.Colides(player2);

            if (state.IsKeyDown(Keys.RightControl))
            {
                if (player.CanPlaceBomb())
                {
                    bombs.Add(new Bomb(bombTexture, map.GetTileCenter(player.MapCollisionBox.Center.ToVector2()), player));
                    player.Bombs++;
                    player.onBomb = true;
                }
            }

            if (state.IsKeyDown(Keys.LeftShift))
            {
                if (player2.CanPlaceBomb())
                {
                    bombs.Add(new Bomb(bombTexture, map.GetTileCenter(player2.MapCollisionBox.Center.ToVector2()), player2));
                    player2.Bombs++;
                    player2.onBomb = true;
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
                if (bomb.MapCollisionBox.Intersects(bomb.Owner.MapCollisionBox))
                {
                    if (bomb.IsSolid && !bomb.Owner.Teleported)
                    {
                        if (bomb.Owner.OldPosition.X != bomb.Owner.Position.X) // Če je pršu z leve/ desne
                        {
                            if (bomb.Owner.OldPosition.X > bomb.Owner.Position.X) // Če pride iz leve
                                bomb.Owner.Acceleration.X += (bomb.MapCollisionBox.Right - bomb.Owner.MapCollisionBox.X);
                                    //+ offSet;
                            else
                                bomb.Owner.Acceleration.X -= ((bomb.Owner.MapCollisionBox.Right) - bomb.MapCollisionBox.X);
                                    //+ offSet);
                        }
                        else if (bomb.Owner.OldPosition.Y != bomb.Owner.Position.Y)
                        {
                            if (bomb.Owner.OldPosition.Y > bomb.Owner.Position.Y) // Če pride iz uspod
                                bomb.Owner.Acceleration.Y += (bomb.MapCollisionBox.Bottom - bomb.Owner.MapCollisionBox.Y);
                            else
                                bomb.Owner.Acceleration.Y -= (bomb.Owner.MapCollisionBox.Bottom - bomb.MapCollisionBox.Y);
                        }
                        player.UpdatePos();
                        break;
                    }
                }
                else
                {
                    if(bomb == bombs.FindAll(bomb1 => bomb1.Owner == bomb.Owner).Last())
                        bomb.Owner.onBomb = false;
                    bomb.IsSolid = true;
                }
            }


            foreach (var explosion in explosions)
            {
                if (explosion.Collides(player.HitBox))
                {
                    p1win = false;
                    player.Kill();
                }

                if (explosion.Collides(player2.HitBox))
                    player2.Kill();

                foreach (var bomb in bombs)
                {
                    if (explosion.Collides(bomb.MapCollisionBox))
                        bomb.Kill();
                }

            }

            map.CheckPowers(player);
            map.CheckPowers(player2);
        }


        private bool playerWin = false, endGame = false;
        private void Gameover()
        {
            owner.game.IsMouseVisible = true;

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

            if(p1win)
                endGameTexture2D = content.Load<Texture2D>("Game/p1win");
            else
                endGameTexture2D = content.Load<Texture2D>("Game/p2win");
            endGame = true;
        }
        private void Retry(object button)
        {
            owner.ChangeScreen(new Player2Screen(owner, backup_map));
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
            player2 = new Player(content.Load<Texture2D>("Game/player2"), Color.White);
            bombTexture = content.Load<Texture2D>("Game/bomb");
            explosionSheet = content.Load<Texture2D>("Game/explosion");

            Player.DeathSoundEffect = content.Load<SoundEffect>("Game/Sound/death");
            Monster.DeathSoundEffect = content.Load<SoundEffect>("Game/Sound/mob_death");
            Bomb.ExplosionSoundEffect = content.Load<SoundEffect>("Game/Sound/explosion");

            player.Position = map.GetSpawnLocation(0);
            player.Position.Y -= player.Height / 2;
            player.Position.X += player.Width / 4 - 4;
            player.Death += new Player.PlayerDeathEventHandler(Gameover);

            player2.Position = map.GetSpawnLocation(1);
            player2.Position.Y -= player.Height / 2;
            player2.Position.X += player.Width / 4 - 4;
            player2.Death += new Player.PlayerDeathEventHandler(Gameover);

            map.LoadTileSet(content);

            if (Properties.Settings.Default.Music)
            {
                backgroundMusic = content.Load<SoundEffect>("Game/Sound/player1music.wav").CreateInstance();
                backgroundMusic.Volume = 0.5f;
                backgroundMusic.IsLooped = true;
                backgroundMusic.Play();
            }
        }
    }
}
