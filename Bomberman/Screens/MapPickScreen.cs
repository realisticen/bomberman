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
        public MapPickScreen(ScreenManager owner) : base( owner)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;

            Map map = new Map();
            //map.tileSet = content.Load<Texture2D>("Maps/tileset");
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
                1,0,1,2,2,1,2,2,1,2,0,1,0,1,2,1,2,2,1,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,1,0,0,2,1,2,2,2,0,0,0,0,0,1,
                1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,1,2,2,0,1,0,0,0,0,0,0,0,0,1,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
            };
            map.Portals = new Vector2[]
            {
                new Vector2(200, 120),
                new Vector2(170, 110)
            };
            map.Spawns = new short[] {22};

            //Map map = JsonConvert.DeserializeObject<Map>(File.ReadAllText("Maps/test.map"));

            owner.ChangeScreen(new Player1Screen(owner, map));

            var json = JsonConvert.SerializeObject(map); // TODO: SHRAN KT JSON PA NALODI KT JSON...
            File.WriteAllText("Maps/test.map", json);
        }
    }
}
