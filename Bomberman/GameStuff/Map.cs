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
        // TODO: SPAWN
        public short[] _mapLayout;
        public short[] _solidTiles;
        public Vector2[] _portals;
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

        public Vector2[] Portals
        {
            get { return _portals; }
            set { _portals = value;
                UpdateCollisionArray();
            }
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

            if (Portals == null)
                return;
            for (short i = 0; i < Portals.Length; i+=2)
            {
                _mapLayout[(int)Portals[i].X] = 3;
                _mapLayout[(int)Portals[i].Y] = 3;
                collisionMap[(int)Portals[i].X] = (short)(i + 2);
                collisionMap[(int)Portals[i].Y] = (short)(i + 2);
            }
        }

        private int startTileY, startTileX, endTileY, endTileX;
        private const int offSet= 0;
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
                    if (collisionMap[i*mapWidth + j] > 0)
                    {
                        collisionBox.X = j*tileWidth;
                        collisionBox.Y = i*tileHeight;

                        if (!obj.MapCollisionBox.Intersects(collisionBox))
                            continue;

                        if (collisionMap[i * mapWidth + j] == 1)
                        {
                            if (obj.OldPosition.X != obj.Position.X) // Če je pršu z leve/ desne
                            {
                                if (obj.OldPosition.X > obj.Position.X) // Če pride iz leve
                                    obj.Acceleration.X += (collisionBox.Right - obj.MapCollisionBox.X); //+ offSet;
                                //    obj.Acceleration.X = obj.Acceleration.X + (j * tileWidth + tileWidth) - obj.MapCollisionBox.X + offSet;
                                else
                                    obj.Acceleration.X -= ((obj.MapCollisionBox.Right) - collisionBox.X); //+ offSet);
                                //obj.Acceleration.X = obj.Acceleration.X - (obj.MapCollisionBox.Right - (j * tileWidth)) - offSet;
                            }
                            else if (obj.OldPosition.Y != obj.Position.Y)
                            {
                                if (obj.OldPosition.Y > obj.Position.Y) // Če pride iz uspod
                                    obj.Acceleration.Y += (collisionBox.Bottom - obj.MapCollisionBox.Y);
                                //obj.Acceleration.Y = obj.Acceleration.Y + (i * tileHeight + tileHeight) - obj.MapCollisionBox.Y + offSet;
                                else
                                    obj.Acceleration.Y -= (obj.MapCollisionBox.Bottom - collisionBox.Y);
                                //    obj.Acceleration.Y = obj.Acceleration.Y - (obj.MapCollisionBox.Bottom - (i * tileHeight)) - offSet;
                            }
                            obj.UpdatePos();
                            return;
                        }
                        else
                        {
                            if(obj.Teleported)
                                continue;

                            int tmp = i*mapWidth + j;

                            foreach (var portal in _portals)
                            {
                                if (tmp == portal.X)
                                {
                                    tmp = (int)portal.Y;
                                    break;
                                }
                                else if (tmp == portal.Y)
                                {
                                    tmp = (int)portal.X;
                                    break;
                                }
                            }

                            //if (tmp == 120)
                            //    return;

                            collisionBox.X = (tmp % mapWidth) * tileWidth;
                            collisionBox.Y = (tmp / mapWidth) * tileHeight;

                            //obj.Acceleration.X += (obj.MapCollisionBox.X > collisionBox.X) ? (collisionBox.X - obj.MapCollisionBox.X) : (collisionBox.X - obj.MapCollisionBox.X);
                            //obj.Acceleration.Y += (obj.MapCollisionBox.Y > collisionBox.Y) ? (collisionBox.Center.Y - obj.MapCollisionBox.Y) : (collisionBox.Center.Y - obj.MapCollisionBox.Y);

                            obj.Acceleration.X += (collisionBox.X - obj.MapCollisionBox.X) + ((collisionBox.Width / 2) - (obj.MapCollisionBox.Width / 2));
                            obj.Acceleration.Y += (collisionBox.Center.Y - obj.MapCollisionBox.Y) + ((collisionBox.Height /2) - (obj.Height/ 2));

                            obj.UpdatePos();
                            obj.Teleported = true;
                            return;
                        }
                    }
                } // 2 for zanka
            } // 1. for zanka
    
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (_mapLayout[i*mapWidth + j] == 3)
                        spriteBatch.Draw(tileSet,
                            new Rectangle(j*tileWidth, i*tileHeight, tileWidth, tileHeight),
                            new Rectangle(0, 0, tileWidth, tileHeight),
                            Color.White);

                    spriteBatch.Draw(tileSet,
                                                new Rectangle(j * tileWidth, i * tileHeight, tileWidth, tileHeight),
                                                new Rectangle(tileWidth * _mapLayout[i * mapWidth + j], 0, tileWidth, tileHeight),
                                                Color.White);
                }
            }
        }
    }
}
