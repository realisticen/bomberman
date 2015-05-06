using System;
using System.Collections.Generic;
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
    class MenuScreen : Screen
    {
        private ButtonManager buttonManager;
        private Texture2D backgroud;

        public MenuScreen(ScreenManager owner) : base( owner)
        {
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroud, Vector2.Zero);

            buttonManager.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            buttonManager.Update(owner.game.cam);
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            backgroud = content.Load<Texture2D>("MenuScreen/background");

            buttonManager = new ButtonManager();
            var image = content.Load<Texture2D>("MenuScreen/1player");
            var button = new ButtonImage(new Image(image));
            button.MouseClick += new Clickable.MouseEventHandler(Player_1);
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(100, 300);
            buttonManager.Buttons.Add(button);

            image = content.Load<Texture2D>("MenuScreen/2player");
            button = new ButtonImage(new Image(image));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseClick += new Clickable.MouseEventHandler(Player_2);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(100, 450);
            buttonManager.Buttons.Add(button);

            image = content.Load<Texture2D>("MenuScreen/mapeditor");
            button = new ButtonImage(new Image(image));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseClick += new Clickable.MouseEventHandler(MapEditor);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(780, 300);
            buttonManager.Buttons.Add(button);

            image = content.Load<Texture2D>("MenuScreen/options");
            button = new ButtonImage(new Image(image));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseClick += new Clickable.MouseEventHandler(Options);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(900, 450);
            buttonManager.Buttons.Add(button);

            image = content.Load<Texture2D>("MenuScreen/exit");
            button = new ButtonImage(new Image(image));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseClick += new Clickable.MouseEventHandler(Exit);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(540, 650);
            buttonManager.Buttons.Add(button);

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

        private void Hide(object button)
        {
            var but = button as ButtonImage;
            but.Image.Color = Color.Red;
        }

        private void Options(object button)
        {
            owner.ChangeScreen(new OptionsScreen(owner));
        }

        private void Player_1(object button)
        {
            owner.ChangeScreen(new MapPickScreen(owner, MapPickScreen.GameType.Player_1));
        }

        private void Player_2(object button)
        {
            owner.ChangeScreen(new MapPickScreen(owner, MapPickScreen.GameType.Player_2));
        }

        private void MapEditor(object button)
        {
            owner.ChangeScreen(new MapPickScreen(owner, MapPickScreen.GameType.MapEditor));
        }
        private void Exit(object button)
        {
            owner.game.Exit();
        }
    }
}
