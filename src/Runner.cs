using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runner
{
    public class Runner : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Runner instance;

        public static List<Drawable3D> drawables = new List<Drawable3D>();
        public static Camera camera;

        public static KeyboardState lastKeyState;

        public static ChunkMap map;

        public Runner()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            
            instance = this;
        }

        public static GraphicsDevice getGraphicsDeviceManager() {
            return instance.GraphicsDevice;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            Window.IsBorderless = false;
            Window.AllowUserResizing = true;

            graphics.ApplyChanges();
            
            base.Initialize();
            
            Textures.loadTextures();
            Chunk.loadMapData();
            
            map = new ChunkMap();

            camera = new Camera(new Vector2(0, 50), 3);
            for (int i = 10; i >= 0; i--) {
                drawables.Add(new Entity(new Vector2(0, 10), -i * 0.7F));
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        private float delta(GameTime gameTime) {
            return (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        protected override void Update(GameTime gameTime) {

            float deltaTime = delta(gameTime);

            KeyboardState keyState = Keyboard.GetState();

            KeyInfo keys = new KeyInfo(keyState, lastKeyState);
            
            if (keys.down(Keys.Escape))
                Exit();

            const float sped = 20;
            if (keys.down(Keys.A))
                camera.pos -= Vector2.UnitX * deltaTime * sped;
            
            if (keys.down(Keys.D))
                camera.pos += Vector2.UnitX * deltaTime * sped;


            lastKeyState = keyState;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null, null, null, null);

            /*for (int i = 0; i < drawables.Count; i++) {
                drawables[i].render(camera, spriteBatch);
            }*/
            
            map.render(camera, spriteBatch, 0);
            map.render(camera, spriteBatch, 1);
            map.render(camera, spriteBatch, 2);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
