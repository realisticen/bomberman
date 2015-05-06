using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Screens
{
    class OptionsScreen : Screen
    {
        private ButtonManager buttonManager;
        private Texture2D backgroud, music, effect, on, off;

        public OptionsScreen(ScreenManager owner) : base( owner)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroud, Vector2.Zero);
            spriteBatch.Draw(music, new Vector2(288, 250));
            spriteBatch.Draw(effect, new Vector2(200, 350));

            buttonManager.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            buttonManager.Update(owner.game.cam);
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;

            backgroud = content.Load<Texture2D>("OptionsScreen/background");
            effect = content.Load<Texture2D>("OptionsScreen/music");
            on= content.Load<Texture2D>("OptionsScreen/on");
            off= content.Load<Texture2D>("OptionsScreen/off");
            music = content.Load<Texture2D>("OptionsScreen/effects");

            buttonManager = new ButtonManager();
            var button = new ButtonImage(new Image(((Properties.Settings.Default.Sound) ? on : off)));
            button.MouseClick += new Clickable.MouseEventHandler(SoundToggle);
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(790, 246);
            buttonManager.Buttons.Add(button);

            button = new ButtonImage(new Image((Properties.Settings.Default.Music) ? on : off));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseClick += new Clickable.MouseEventHandler(MusicToggle);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(790, 346);
            buttonManager.Buttons.Add(button);

        }

        private void MusicToggle(object button)
        {
            var but = button as ButtonImage;
            Properties.Settings.Default.Music = !Properties.Settings.Default.Music;
            but.ChangeImage(new Image(((Properties.Settings.Default.Music) ? on : off)));
            Properties.Settings.Default.Save();
        }

        private void SoundToggle(object button)
        {
            var but = button as ButtonImage;
            Properties.Settings.Default.Sound = !Properties.Settings.Default.Sound;
            but.ChangeImage(new Image(((Properties.Settings.Default.Sound) ? on : off)));
            Properties.Settings.Default.Save();
        }

        private void Hover(object button)
        {
            var but = button as ButtonImage;
            but.Image.Color = Color.Turquoise;
        }

        private void Clear(object button)
        {
            var but = button as ButtonImage;
            but.Image.Color = Color.White;
        }
    }
}
