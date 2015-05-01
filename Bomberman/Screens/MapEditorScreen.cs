using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bomberman.BaseClass;
using Bomberman.GameStuff;
using Bomberman.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.IO;

namespace Bomberman.Screens
{
    class MapEditorScreen : Screen
    {
        private Map map;
        private Texture2D tools, tileSet;
        private Image hudImg;
        private Rectangle sourcerRectangle, destinationRectangle;

        enum Tool
        {
            Grass,
            Wall,
            Block,
            Portal,
            MobSpawn,
            PlayerSpan,
            Save,
            Exit
        }

        public MapEditorScreen(ScreenManager owner, Map _map) : base( owner)
        {
            map = _map;
            sourcerRectangle = destinationRectangle = new Rectangle(0, 0, iconWidth, iconWidth);
        }

        private Tool tool;
        private const int startXIcons = (1344 - totaWidth) / 2, iconWidth = 64, totaWidth = 552;
        public override void Draw(SpriteBatch spriteBatch)
        {
            map.Draw(spriteBatch);

            foreach (var VARIABLE in map.Portals)
            {
                
            }

            var rect = new Rectangle(0, 0, 64, 64);
            for (int i = 0; i < map.Spawns.Count; i++)
            {
                rect.X = (int)map.GetSpawnLocation(i).X;
                rect.Y = (int)map.GetSpawnLocation(i).Y;
                if (i < 2)
                    spriteBatch.Draw(tools, rect, new Rectangle(64, 0, 64, 64), Color.White);
                else
                    spriteBatch.Draw(tools, rect, new Rectangle(0, 0, 64, 64), Color.White);
            }



            if(!hideHud)
            {
                hudImg.Draw(spriteBatch);

                sourcerRectangle.X = 0;
                destinationRectangle.X = startXIcons;
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(tileSet, destinationRectangle, sourcerRectangle, Color.White);
                    sourcerRectangle.X += iconWidth;
                    destinationRectangle.X += iconWidth;
                }

                sourcerRectangle.X = 0;
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(tools, destinationRectangle, sourcerRectangle, Color.White);
                    sourcerRectangle.X += iconWidth;
                    destinationRectangle.X += iconWidth;
                }
            }


        }

        private int player = 0;
        private bool hideHud = false, justPressed = false;
        public override void Update(GameTime gameTime)
        {
            if (!justPressed && Keyboard.GetState().IsKeyDown(Keys.H))
            {
                hideHud = !hideHud;
                justPressed = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.H))
                justPressed = false;

            var mouseState = Clickable.ApplayCamera(owner.game.cam, Mouse.GetState());
            if (!hideHud && mouseState.LeftButton == ButtonState.Pressed && mouseState.Y > destinationRectangle.Y && mouseState.Y < destinationRectangle.Bottom
                && mouseState.X > startXIcons && mouseState.X < startXIcons + totaWidth)
            {
                tool = (Tool)((mouseState.X - startXIcons) / 64);
                switch (tool)
                {
                    case Tool.Save:
                        var json = JsonConvert.SerializeObject(map); // TODO: SHRAN KT JSON PA NALODI KT JSON...
                        File.WriteAllText("Maps/test.map", json);
                        tool = Tool.Grass;                        
                        break;
                    case Tool.Exit:
                        owner.ChangeScreen(new MapPickScreen(owner));
                        tool = Tool.Grass;
                        break;
                }
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!hideHud && mouseState.Y > hudImg.Postion.Y && mouseState.Y < hudImg.Postion.Y + hudImg.GetHeight()
                    && mouseState.X > hudImg.Postion.X && mouseState.X < hudImg.Postion.X + hudImg.GetWidth())
                {
                    
                }
                else
                {
                    int tileX = mouseState.X / map.tileWidth;
                    int tileY = mouseState.Y / map.tileHeight;
                    if (tileY < map.mapHeight - 1 && tileX < map.mapWidth - 1 && tileY > 0 && tileX > 0)
                    {
                        if (tool == Tool.PlayerSpan)
                        {
                            if (player == 0)
                                player = 1;
                            else
                                player = 0;

                            if (map.Spawns[player] != (short)(tileY * map.mapWidth + tileX))
                            {
                                if (player == 0)
                                    player = 1;
                                else
                                    player = 0;
                                map.Spawns[player] = (short)(tileY*map.mapWidth + tileX);
                                map.MapLayout[tileY * map.mapWidth + tileX] = (int)Tool.Grass;
                            }

                            if (player == 0)
                                player = 1;
                            else
                                player = 0;

                        }
                        else if (tool == Tool.MobSpawn)
                        {
                            if (!map.Spawns.Contains((short) (tileY*map.mapWidth + tileX)))
                            {
                                map.Spawns.Add((short)(tileY * map.mapWidth + tileX));
                                map.MapLayout[tileY * map.mapWidth + tileX] = (int)Tool.Grass;
                            }
                        }
                        else
                        {
                            if(map.Spawns[0] != (short) (tileY*map.mapWidth + tileX) && map.Spawns[1] != (short) (tileY*map.mapWidth + tileX))
                            {
                                map.MapLayout[tileY * map.mapWidth + tileX] = (short)tool;
                                map.Spawns.RemoveAll(s => s == (short) (tileY*map.mapWidth + tileX));
                            }
                        }
                    }
                }
            }
        }

        public override void LoadContent(ContentManager content)
        {
            this.content = content;
            hudImg = new Image(content.Load<Texture2D>("MapEditor/hud"));
            hudImg.Postion = new Vector2(MainGame.VIRTUAL_RESOLUTION_WIDTH / 2 - hudImg.GetWidth() / 2, MainGame.VIRTUAL_RESOLUTION_HEIGHT - hudImg.GetHeight());
            tileSet = content.Load<Texture2D>(map.tileSetPath);
            tools = content.Load<Texture2D>("MapEditor/tools");

            destinationRectangle.Y = (int)hudImg.Postion.Y + (hudImg.GetHeight() / 2 - iconWidth/2);

            map.LoadTileSet(content);
        }
    }
}
