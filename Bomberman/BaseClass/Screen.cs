using Bomberman.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bomberman.BaseClass
{
    public class Screen
    {
        protected ContentManager content;
        protected ScreenManager owner;

        public Screen(ScreenManager _owner)
        {
            owner = _owner;
        }

        virtual public void Update(GameTime gameTime)
        {
            
        }

        virtual public void Draw(SpriteBatch spriteBatch)
        {
            
        }

        virtual public void LoadContent(ContentManager content)
        {
            this.content = content;
        }

        virtual public void UnloadContent()
        {
            content.Unload();
        }
    }
}
