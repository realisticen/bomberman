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
        public short[] _mapLayout;
        public short[] _solidTiles;
        public short[] Portals;
        private short[] collisionMap;

        private Rectangle collisionBox;
        private Texture2D tileSet;
        public string tileSetPath;
        public short tileWidth, tileHeight, mapWidth, mapHeight;

        public short[] MapLayout
        {
            get { return _mapLayout; }
            set { _mapLayout = value; UpdateCollisionArray();}
        }

        public short[] SolidTiles
        {
            get { return _solidTiles; }
            set { _solidTiles = value; UpdateCollisionArray(); }
        }

        public void LoadTileSet(ContentManager content)
        {
            tileSet = content.Load<Texture2D>(tileSetPath);
            collisionBox = new Rectangle(0, 0, tileWidth, tileHeight);
        }

        public void UpdateCollisionArray()
        {
            if(_solidTiles == null || _mapLayout == null)
                return;

            collisionMap = new short[_mapLayout.Length];
            for (int i = 0; i < _mapLayout.Length - 1; i++)
            {
                if (_solidTiles.Contains(_mapLayout[i]))
                    collisionMap[i] = 1;
            }
        }

        private int startTileY, startTileX, endTileY, endTileX;
        private const int offSet= 2;
        public void Colides(MovableObject obj)
        {
            if (_solidTiles == null)
                return;

            startTileY = obj.MapCollisionBox.Y / tileHeight;
            startTileX = obj.MapCollisionBox.X / tileWidth;
            endTileY = obj.MapCollisionBox.Bottom / tileHeight + 1;
            endTileX = obj.MapCollisionBox.Right / tileWidth + 1;

            for (int i = startTileY; i < endTileY; i++)
            {
                for (int j = startTileX; j < endTileX; j++)
                {
                    if (collisionMap[i*mapWidth + j] == 1)
                    {
                        collisionBox.X = j*tileWidth;
                        collisionBox.Y = i*tileHeight;

                        if (!collisionBox.Intersects(obj.MapCollisionBox))
                            continue;

                        if (obj.OldPosition.X != obj.Position.X) // Če je pršu z leve/ desne
                        {
                            if (obj.OldPosition.X > obj.Position.X) // Če pride iz leve
                                obj.Acceleration.X = obj.Acceleration.X + (j * tileWidth + tileWidth) - obj.MapCollisionBox.X + offSet;
                            else
                                obj.Acceleration.X = obj.Acceleration.X - (obj.MapCollisionBox.Right - (j * tileWidth)) - offSet;
                        }
                        else
                        {
                            if (obj.OldPosition.Y > obj.Position.Y) // Če pride iz uspod
                                obj.Acceleration.Y = obj.Acceleration.Y + (i * tileHeight + tileHeight) - obj.MapCollisionBox.Y + offSet;
                            else
                                obj.Acceleration.Y = obj.Acceleration.Y - (obj.MapCollisionBox.Bottom - (i * tileHeight)) - offSet;
                        }
                        obj.UpdatePos();
                        return;
                    }
                }
            }
    
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    spriteBatch.Draw(tileSet,
                                                new Rectangle(j * tileWidth, i * tileHeight, tileWidth, tileHeight),
                                                new Rectangle(tileWidth * _mapLayout[i * mapWidth + j], 0, tileWidth, tileHeight),
                                                Color.White);
                }
            }
        }
    }
}
