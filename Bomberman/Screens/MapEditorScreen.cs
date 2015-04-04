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
using Newtonsoft.Json;

namespace Bomberman.Screens
{
    class MapEditorScreen : Screen
    {
        private Map map;

        public MapEditorScreen(ScreenManager owner) : base( owner)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            map = new Map();
            //map.tileSet = content.Load<Texture2D>("Maps/tileset");
            map.tileSetPath = "Maps/tileset";
            map.tileWidth = 64;
            map.tileHeight = 64;
            map.mapHeight = 4;
            map.mapWidth = 4;
            map.MapLayout = new short[]
            {
                1,2,1,1,
                1,0,0,1,
                1,0,0,1,
                1,1,3,1
            };

            map.LoadTileSet(content);
            var json = JsonConvert.SerializeObject(map);
        }
    }
}
