using Bomberman.Screens;
using Bomberman.Utilities;
using Bomberman.Utilities.Examples.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bomberman
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public ResolutionRenderer cam;
        public const int VIRTUAL_RESOLUTION_WIDTH = 1344; // Maš 64*21 Tk da...
        public const int VIRTUAL_RESOLUTION_HEIGHT = 768;
        private ScreenManager screenManager;

        public MainGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1344;
            graphics.PreferredBackBufferHeight = 768;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 600;
            Window.Position = new Point(0, 0);
            IsMouseVisible = true;
        }

        public void ChangeWindowSice(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            cam = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            cam = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //SetFullScreen(true);

            base.Initialize();
        }

        public void SetFullScreen(bool value)
        {
            graphics.IsFullScreen = value;
            if(value)
                cam = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            else
            {
                cam = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);                
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            screenManager = new ScreenManager(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenManager.ChangeScreen(new MapPickScreen(screenManager)); // todo: naštimi da swe začene spalshcsreen
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            screenManager.Update(gameTime);

            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            cam.SetupVirtualScreenViewport();
            cam.Draw();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, cam.GetTransformationMatrix());
            screenManager.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
