using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.BaseClass;
using Bomberman.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomberman.Screens
{
    class MapEditorScreen : Screen
    {

        public MapEditorScreen(ScreenManager owner) : base( owner)
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

        }
    }
}
