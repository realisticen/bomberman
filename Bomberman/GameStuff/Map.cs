using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Bomberman.BaseClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.GameStuff
{

    class PowerUps
    {
        public enum PowerUp
        {
            Speed,
            BombAmount,
            ExplosionSize
        }

        public PowerUps(PowerUp power, Vector2 pos)
        {
            Power = power;
            Hitbox = new Rectangle((int)pos.X, (int)pos.Y, 32, 32);
            drawRect = new Rectangle(32 * (int)power, 0 , 32, 32);
        }

        public PowerUp Power;
        public Rectangle Hitbox, drawRect;
    }

    class Map
    {
        private List<PowerUps> powers = new List<PowerUps>();

        public short[] _mapLayout;
        public short[] _solidTiles;
        public List<Vector2> _portals = new List<Vector2>();
        private short[] collisionMap;

        public List<short> Spawns = new List<short>(2);
        private Rectangle collisionBox;
        private Texture2D tileSet, powerSheet;
        public string tileSetPath, powerSheetPath;
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

        public List<Vector2> Portals
        {
            get { return _portals; }
            set { _portals = value;
                UpdateCollisionArray();
            }
        }

        public void LoadTileSet(ContentManager content)
        {
            tileSet = content.Load<Texture2D>(tileSetPath);
            powerSheet = content.Load<Texture2D>(powerSheetPath);
            collisionBox = new Rectangle(0, 0, tileWidth, tileHeight);
        }

        public Vector2 GetSpawnLocation(int index)
        {
            return new Vector2(Spawns[index] % mapWidth * tileWidth, Spawns[index] / mapWidth * tileHeight);
        }

        public List<Monster> GetMonsters(Texture2D sheet)
        {
            if (Spawns.Count < 2)
                return null;
            var tmp = new List<Monster>(Spawns.Count - 2);
            for (int i = 0; i < tmp.Capacity; i++)
            {
                tmp.Add(new Monster(sheet));
                tmp[i].Position = GetSpawnLocation(i + 2);
                tmp[i].Position.X +=  tileWidth / 2- tmp[i].Width/2;
                //tmp[i].Position.Y -=  tileHeight / 2 - tmp[i].Height / 2;
                tmp[i].Move((Directons)MainGame.random.Next(1, 5));

            }

            return tmp;
        }

        private void MakePower(int index)
        {
            if (MainGame.random.Next(2) == 1)
                return;

            Vector2 pos = new Vector2((index % mapWidth) * tileWidth + 16, (index / mapWidth) * tileHeight + 16);
            powers.Add(new PowerUps((PowerUps.PowerUp)MainGame.random.Next(0, 3), pos));
        }

        public void CheckPowers(Player player)
        {
            for (int i = 0; i < powers.Count; i++)
            {
                if (player.MapCollisionBox.Intersects(powers[i].Hitbox))
                {
                    switch (powers[i].Power)
                    {
                        case PowerUps.PowerUp.Speed:
                            player.IncreaseSpeed(1);
                            break;
                        case PowerUps.PowerUp.BombAmount:
                            player.IncreaseMaxBombs(1);
                            break;
                        case PowerUps.PowerUp.ExplosionSize:
                            player.IncReaseBombSize(1);
                            break;
                    }
                    powers.RemoveAt(i);
                    i--;
                }
            }
        }

        private bool left, right, down, up;
        public Tuple<Rectangle, Rectangle> MakeExplosion(Bomb bomb)
        {
            int centerTileX = (int)bomb.Position.X / tileWidth;
            int centerTileY = (int)bomb.Position.Y / tileHeight;

            left = right = down = up = true;

            Rectangle hExpl = new Rectangle(centerTileX * tileWidth, centerTileY * tileHeight, tileWidth, tileHeight);
            Rectangle vExpl = new Rectangle(centerTileX * tileWidth, centerTileY * tileHeight, tileWidth, tileHeight);

            for (int i = 1; i < bomb.Owner.BombSize + 1; i++) // Odstran uzničlive bloke
            {
                if (left && centerTileX + (mapWidth * centerTileY) - i > -1) // Levo
                    if (_mapLayout[centerTileX + (mapWidth*centerTileY) - i] == 2)
                    {
                        left = false;
                        _mapLayout[centerTileX + (mapWidth * centerTileY) - i] = 0;
                        MakePower(centerTileX + (mapWidth * centerTileY) - i);
                    }
                    else if (_mapLayout[centerTileX + (mapWidth * centerTileY) - i] == 1)
                        left = false;

                if (right && centerTileX + (mapWidth * centerTileY) + i < _mapLayout.Length) // Desmo
                    if (_mapLayout[centerTileX + (mapWidth * centerTileY) + i] == 2)
                    {
                        right = false;
                        _mapLayout[centerTileX + (mapWidth * centerTileY) + i] = 0;
                        MakePower(centerTileX + (mapWidth * centerTileY) + i);
                    }
                    else if (_mapLayout[centerTileX + (mapWidth * centerTileY) + i] == 1)
                        right = false;

                if (down && centerTileX + (mapWidth * (centerTileY + i)) < _mapLayout.Length) // Dol
                    if (_mapLayout[centerTileX + (mapWidth * (centerTileY + i))] == 2)
                    {
                        down = false;
                        _mapLayout[centerTileX + (mapWidth * (centerTileY + i))] = 0;
                        MakePower(centerTileX + (mapWidth * (centerTileY + i)));
                    }
                    else if (_mapLayout[centerTileX + (mapWidth * (centerTileY + i))] == 1)
                        down = false;

                if (up && centerTileX + (mapWidth * (centerTileY - i)) > -1) //Gor
                    if (_mapLayout[centerTileX + (mapWidth * (centerTileY - i))] == 2)
                    {
                        up = false;
                        _mapLayout[centerTileX + (mapWidth * (centerTileY - i))] = 0;
                        MakePower(centerTileX + (mapWidth * (centerTileY - i)));
                    }
                    else if (_mapLayout[centerTileX + (mapWidth * (centerTileY - i))] == 1)
                        up = false;
            }
            UpdateCollisionArray();

            // Pogledam kako deleč lahko gre ekslpozija
            for (int i = 1; i < bomb.Owner.BombSize + 1; i++) // Levo
            {
                if (collisionMap[(centerTileX + (mapWidth * centerTileY)) - i] != 0 && _mapLayout[(centerTileX + (mapWidth * centerTileY)) - i] != 3)
                    break;

                hExpl.X -= tileWidth;
                hExpl.Width += tileWidth;
            }

            for (int i = 1; i < bomb.Owner.BombSize + 1; i++) // desno
            {
                if (collisionMap[centerTileX + (mapWidth * centerTileY) + i] != 0 && _mapLayout[centerTileX + (mapWidth * centerTileY) + i] != 3)
                    break;

                hExpl.Width += tileWidth;
            }

            for (int i = 1; i < bomb.Owner.BombSize + 1; i++) // gor
            {
                if (collisionMap[centerTileX + (mapWidth * (centerTileY - i))] != 0 && _mapLayout[centerTileX + (mapWidth * (centerTileY - i))] != 3)
                    break;

                vExpl.Y -= tileHeight;
                vExpl.Height += tileHeight;
            }
            for (int i = 1; i < bomb.Owner.BombSize + 1; i++) //dol
            {
                if (collisionMap[centerTileX + (mapWidth * (centerTileY + i))] != 0 && _mapLayout[centerTileX + (mapWidth * (centerTileY + i))] != 3)
                    break;

                vExpl.Height += tileHeight;
            }

            return Tuple.Create(vExpl, hExpl);
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
            for (short i = 0; i < Portals.Count; i++)
            {
                _mapLayout[(int)Portals[i].X] = 3;
                _mapLayout[(int)Portals[i].Y] = 3;
                collisionMap[(int)Portals[i].X] = (short)(i + SolidTiles.Length);
                collisionMap[(int)Portals[i].Y] = (short)(i + SolidTiles.Length);
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
                    if (i * mapWidth + j >= collisionMap.Length || i * mapWidth + j < 0)
                        continue;
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
                            obj.Acceleration.Y += (collisionBox.Y - obj.MapCollisionBox.Y) + ((collisionBox.Height / 2) - (obj.MapCollisionBox.Height / 2));

                            obj.UpdatePos();
                            obj.Teleported = true;
                            return;
                        }
                    }
                } // 2 for zanka
            } // 1. for zanka
    
        }

        public Vector2 GetTileCenter(Vector2 pos)
        {
            int x = (int)(pos.X / tileWidth);
            int y = (int)(pos.Y / tileHeight);

            return new Vector2((x * tileWidth) + (tileWidth / 2), (y * tileHeight) + (tileHeight / 2));
        }

        private float rotation = 0;
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    rotation += 0.0001f; 
                    if (_mapLayout[i*mapWidth + j] == 3)
                    {
                        spriteBatch.Draw(tileSet, new Rectangle(j * tileWidth, i * tileHeight, tileWidth, tileHeight), new Rectangle(0, 0, tileWidth, tileHeight), Color.White);

                        if (rotation < 0)
                            rotation = 0;

                        spriteBatch.Draw(tileSet, new Rectangle(j * tileWidth + 32, i * tileHeight + 32, tileWidth, tileHeight), new Rectangle(tileWidth * _mapLayout[i * mapWidth + j], 0, tileWidth, tileHeight), Color.White,
                            rotation, new Vector2(32, 32), SpriteEffects.None, 1);
                        continue;
                    }

                    spriteBatch.Draw(tileSet, new Rectangle(j * tileWidth, i * tileHeight, tileWidth, tileHeight), new Rectangle(tileWidth * _mapLayout[i * mapWidth + j], 0, tileWidth, tileHeight), Color.White);
                }
            }
            foreach (var power in powers)
            {
                spriteBatch.Draw(powerSheet, power.Hitbox, power.drawRect, Color.White);
            }
        }

        public void DrawSmall(SpriteBatch spriteBatch, float scale, Vector2 pos)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (_mapLayout[i * mapWidth + j] == 3)
                    {
                        spriteBatch.Draw(tileSet, new Vector2(j * tileWidth * scale + pos.X, i * tileHeight * scale + pos.Y), new Rectangle(0, 0, tileWidth, tileHeight), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1);

                        spriteBatch.Draw(tileSet, new Vector2(j * tileWidth * scale + pos.X, scale * i * tileHeight + pos.Y), new Rectangle(tileWidth * _mapLayout[i * mapWidth + j], 0, tileWidth, tileHeight), Color.White,
                            0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        continue;
                    }

                    spriteBatch.Draw(tileSet, new Vector2(j * tileWidth * scale + pos.X, scale * i * tileHeight + pos.Y), new Rectangle(tileWidth * _mapLayout[i * mapWidth + j], 0, tileWidth, tileHeight), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1);
                }
            }
        }
    }
}
