using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.GameStuff;
using Bomberman.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Newtonsoft.Json;

namespace Bomberman.Screens
{
    class MapPickScreen : Screen
    {
        private List<Map> maps = new List<Map>();
        private List<string> files;
        private int page = 1;
        private Texture2D background;
        private Image modePicture;
        private ButtonImage selectedButton;
        private GameType _type;

        private ButtonManager buttonManager;
        private const float scale = 0.3f;

        public enum GameType
        {
            MapEditor,
            Player_1,
            Player_2
        }

        public MapPickScreen(ScreenManager owner, GameType type) : base( owner)
        {
            owner.game.cam.BackgroundColor = new Color(0, 18, 102);
            _type = type;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            for (int i = (page - 1) * 6; i < page * 6; i++)
            {
                if (maps.Count == i)
                    break;

                maps[i].DrawSmall(spriteBatch, scale, new Vector2(((i % 6) % 3) * 430 + 38, 250 * (int)((i % 6) / 3) + 140));
            }
            modePicture.Draw(spriteBatch);
            buttonManager.Draw(spriteBatch);

            if(selectedButton != null)
                selectedButton.Draw(spriteBatch);

        }

        private bool presse = false, deleteMode = false;
        public override void Update(GameTime gameTime)
        {
            buttonManager.Update(owner.game.cam);
            var state = Clickable.ApplayCamera(owner.game.cam, Mouse.GetState());

            if (state.LeftButton == ButtonState.Released)
                presse = false;
            if (selectedButton != null)
                selectedButton.Enabled = false;
            for (int i = (page - 1) * 6; i < page * 6; i++)
            {
                if (maps.Count == i)
                    break;

                selectedButton.SetPosition(((i % 6) % 3) * 430 + 38, 250 * (int)((i % 6) / 3) + 140);
                selectedButton.Update(state);
                if (selectedButton.Enabled)
                {
                    if (state.LeftButton == ButtonState.Pressed && !presse)
                    {
                        if (!deleteMode)
                            MapSelected(i);
                        else
                            DeleteMap(i);
                    }
                    break;
                }
                //maps[i].DrawSmall(spriteBatch, scale, new Vector2(((i % 6) % 3) * 430 + 38, 250 * (int)((i % 6) / 3) + 140));
            }
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                presse = true;

            background = content.Load<Texture2D>("MapPickScreen/background");
            switch (_type)
            {
                case GameType.MapEditor:
                    modePicture = new Image(content.Load<Texture2D>("MenuScreen/mapeditor"));
                    break;
                case GameType.Player_1:
                    modePicture = new Image(content.Load<Texture2D>("MenuScreen/1player"));
                    break;
                case GameType.Player_2:
                    modePicture = new Image(content.Load<Texture2D>("MenuScreen/2player"));
                    break;
            }
            modePicture.Postion.X = MainGame.VIRTUAL_RESOLUTION_WIDTH/2 - modePicture.GetWidth()/2;
            modePicture.Postion.Y = MainGame.VIRTUAL_RESOLUTION_HEIGHT - modePicture.GetHeight() - 50;

            if (!Directory.Exists("Maps"))
                Directory.CreateDirectory("Maps");

            files = Directory.GetFiles("Maps", "*.map").ToList();
            foreach (var file in files)
            {
                maps.Add(JsonConvert.DeserializeObject<Map>(File.ReadAllText(file)));
                maps.Last().LoadTileSet(content);
            }

            buttonManager = new ButtonManager();
            var image = content.Load<Texture2D>("MapPickScreen/nextpage");
            var button = new ButtonImage(new Image(image)) { Enabled = false};
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.MouseClick += new Clickable.MouseEventHandler(NextPage);
            button.SetPosition(1050, (int)modePicture.Postion.Y + modePicture.GetHeight() - image.Height - 10);
            buttonManager.Buttons.Add(button);

            if (maps.Count/6 > 1)
                button.Enabled = true;

            image = content.Load<Texture2D>("MapPickScreen/prevpage");
            button = new ButtonImage(new Image(image));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseClick += new Clickable.MouseEventHandler(PrevPage);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.Enabled = false;
            button.SetPosition(40, (int)modePicture.Postion.Y + modePicture.GetHeight() - image.Height - 10);
            buttonManager.Buttons.Add(button);

            image = content.Load<Texture2D>("MapPickScreen/back");
            button = new ButtonImage(new Image(image));
            button.MouseLeave += new Clickable.MouseEventHandler(Clear);
            button.MouseClick += new Clickable.MouseEventHandler(GoToMenu);
            button.MouseEnter += new Clickable.MouseEventHandler(Hover);
            button.SetPosition(40, 60);
            buttonManager.Buttons.Add(button);

            if (maps.Count > 0)
            {
                var test = new Texture2D(owner.game.GraphicsDevice, (int)(scale * maps[0].tileWidth * maps[0].mapWidth), (int)(scale * maps[0].tileHeight * maps[0].mapHeight));
                var data = new Color[test.Width*test.Height];
                for (int i = 0; i < data.Length; i++) data[i] = Color.Green;
                test.SetData(data);
                selectedButton = new ButtonImage(new Image(test) {Alpha = 0.65f}) { Enabled = false};
                selectedButton.MouseOver += new Clickable.MouseEventHandler(OverMap);
            }

            if (_type == GameType.MapEditor)
            {
                image = content.Load<Texture2D>("MapPickScreen/newmap");
                button = new ButtonImage(new Image(image));
                button.MouseLeave += new Clickable.MouseEventHandler(Clear);
                button.MouseClick += new Clickable.MouseEventHandler(NewMap);
                button.MouseEnter += new Clickable.MouseEventHandler(Hover);
                button.SetPosition(1050, 38);
                buttonManager.Buttons.Add(button);

                image = content.Load<Texture2D>("MapPickScreen/deletemode");
                button = new ButtonImage(new Image(image));
                button.MouseClick += new Clickable.MouseEventHandler(ToggleDeleteMode);
                button.SetPosition(1050, 90);
                buttonManager.Buttons.Add(button);
            }

            //Map map = JsonConvert.DeserializeObject<Map>(File.ReadAllText("Maps/2.map"));

            //owner.ChangeScreen(new Player1Screen(owner, map));
            //owner.ChangeScreen(new MapEditorScreen(owner, maps[0], "3.map"));

            //var json = JsonConvert.SerializeObject(map); // TODO: SHRAN KT JSON PA NALODI KT JSON...
            //File.WriteAllText("Maps/test.map", json);

        }

        private void NewMap(object button)
        {
            Map map = new Map();
            map.tileSetPath = "Maps/tileset";
            map.powerSheetPath = "Maps/powers";
            map.tileWidth = 64;
            map.tileHeight = 64;
            map.mapHeight = 12;
            map.mapWidth = 21;
            map.SolidTiles = new short[] { 1, 2, 3 };
            map.MapLayout = new short[]
            {
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
            };
            map.Spawns.AddRange(new short[] { 22, 46 });

            string map_name = ".map";
            for (int i = 0; i < Int32.MaxValue; i++)
            {
                if (!File.Exists("Maps/" + i + map_name))
                {
                    map_name = "Maps/" + i + map_name;
                    break;
                }
            }
            owner.ChangeScreen(new MapEditorScreen(owner, map, map_name));
        }

        private void DeleteMap(int index)
        {
            presse = true;
            File.Delete(files[index]);
            maps.RemoveAt(index);
            files.RemoveAt(index);
            if (maps.Count <= page*6)
            {
                buttonManager.Buttons[0].Enabled = false;
                if ((page*6) - maps.Count > 5)
                {
                    if (page > 1)
                    {
                        page--;
                        if (page == 1)
                            buttonManager.Buttons[1].Enabled = false;
                    }
                }
            }
        }

        private void ToggleDeleteMode(object button)
        {

            var but = button as ButtonImage;
            if (!deleteMode)
            {
                deleteMode = true;
                but.Image.Color = Color.LawnGreen;
                selectedButton.Image.Color = Color.Purple;
            }
            else
            {
                deleteMode = false;
                but.Image.Color = Color.White;
                selectedButton.Image.Color = Color.White;
            }
        }

        private void MapSelected(int index)
        {
            switch (_type)
            {
                case GameType.MapEditor:
                    owner.ChangeScreen(new MapEditorScreen(owner, maps[index], files[index]));
                    break;
                case GameType.Player_1:
                    owner.ChangeScreen(new Player1Screen(owner,maps[index]));
                    break;
                case GameType.Player_2:
                    owner.ChangeScreen(new Player2Screen(owner, maps[index]));
                    break;
            }
        }

        private void GoToMenu(object button)
        {
            owner.ChangeScreen(new MenuScreen(owner));
        }

        private void OverMap(object button)
        {
            var but = button as ButtonImage;
            but.Enabled = true;
        }

        private void Hover(object button)
        {
            var but = button as ButtonImage;
            but.Image.Color = Color.Turquoise;
        }

        private void NextPage(object button)
        {
            var but = button as ButtonImage;
            if (maps.Count > 6*page)
            {
                page++;
                buttonManager.Buttons[1].Enabled = true;
                if (maps.Count <= page*6)
                    but.Enabled = false;
            }
        }

        public void PrevPage(object button)
        {
            var but = button as ButtonImage;
            if (page > 1)
            {
                page --;
                if (page == 1)
                    but.Enabled = false;
                buttonManager.Buttons[0].Enabled = true;
            }
        }

        private void Clear(object button)
        {
            var but = button as ButtonImage;
            but.Image.Color = Color.White;
        }
    }
}
