using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.GameStuff
{
    class Map
    {
        public short[] MapLayout;
        public short[] SolidTiles;
        public short[] Portals;
        private Texture2D tileSet;
        public string tileSetPath;
        public short tileWidth, tileHeight, mapWidth, mapHeight;
        private Rectangle collisionBox;

        public void LoadTileSet(ContentManager content)
        {
            tileSet = content.Load<Texture2D>(tileSetPath);
            collisionBox = new Rectangle(0, 0, tileWidth, tileHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    spriteBatch.Draw(tileSet,
                                                new Rectangle(j * tileWidth, i * tileHeight, tileWidth, tileHeight),
                                                new Rectangle(tileWidth * MapLayout[i * mapWidth + j], 0, tileWidth, tileHeight),
                                                Color.White);
                }
            }
        }
    }
}
