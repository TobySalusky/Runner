using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Runner
{
    public class Runner : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Runner instance;

        public static List<Particle>[] particles = new List<Particle>[3];
        public static List<Entity>[] entities = new List<Entity>[3];
        public static Camera camera;

        public static KeyboardState lastKeyState;

        public static ChunkMap map;
        public static Player player;

        public static float screenShakeTime, screenShakeStart, screenShakeIntensity;

        public static SoundEffect calm;
        public static SoundEffect chaos;

        public static SoundEffectInstance calmI;
        public static SoundEffectInstance chaosI;


        public Runner()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            
            instance = this;
        }

        public static void shakeScreen(float time, float intensity) {
            screenShakeStart = time;
            screenShakeTime = time;
            screenShakeIntensity = intensity;
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
            SoundPlayer.loadEffects();
            Chunk.loadMapData();

            calm = SoundPlayer.getEffect("calmrunner");
            chaos = SoundPlayer.getEffect("chaosrunner");

            calmI = calm.CreateInstance();
            chaosI = chaos.CreateInstance();

            calmI.Play();
            calmI.Volume = 1.0F;
            chaosI.Play();
            chaosI.Volume = 0.0F;

            map = new ChunkMap();

            camera = new Camera(Vector2.Zero, 5);
            player = new Player(playerStartPos());

            for (int i = 0; i < particles.Length; i++) {
                particles[i] = new List<Particle>();
            }
        }

        public static Vector2 playerStartPos() {
            return new Vector2(30, 40);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        private float delta(GameTime gameTime) {
            return (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void renderParticles(List<Particle> list) {
            foreach (var particle in list) {
                particle.render(camera, spriteBatch);
            }
        }
        
        public void renderEntities(List<Entity> list) {
            foreach (var entity in list) {
                entity.render(camera, spriteBatch);
            }
        }
        
        public void updateParticles(float deltaTime) {
            foreach (var list in particles) {
                for (int i = list.Count - 1; i >= 0; i--) {
                    Particle particle = list[i];

                    if (particle.deleteFlag) {
                        list.RemoveAt(i);
                        continue;
                    }

                    particle.update(deltaTime);
                }
            }
        }

        protected override void Update(GameTime gameTime) {

            float deltaTime = delta(gameTime);

            KeyboardState keyState = Keyboard.GetState();

            KeyInfo keys = new KeyInfo(keyState, lastKeyState);

            if (keys.down(Keys.Escape))
                Exit();

            lastKeyState = keyState;

            if (keys.down(Keys.Q)) 
            {
                calmI.Volume = (-1.0F) * calmI.Volume + 1.0F;
                chaosI.Volume = (-1.0F) * chaosI.Volume + 1.0F;
            }
            // TODO: Add your update logic here

            player.input(keys, deltaTime);
            player.update(deltaTime);
            
            updateParticles(deltaTime);

            camera.pos = player.pos - Vector2.UnitY * 5;
            // screen shake
            if (screenShakeTime > 0) {
                screenShakeTime -= deltaTime;
                camera.pos += Util.polar(screenShakeIntensity * screenShakeTime / screenShakeStart, Util.randomAngle());
            }

            Vector2 diff = camera.screenCenter / (camera.scale * camera.farMult(-2));
            float clampCameraY = Chunk.mapData[0].GetLength(1) - 1 - diff.Y;
            camera.pos.Y = Math.Min(camera.pos.Y,clampCameraY);

            base.Update(gameTime);


            for (int i = 0; i < entities.Length; i++) {
                entities[i] = new List<Entity>();
            }
            sortIntoEntities(player);
        }

        public void sortIntoEntities(Entity entity) {
            entities[entity.getLayer()].Add(entity);
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
            renderEntities(entities[0]);
            renderParticles(particles[0]);
            map.render(camera, spriteBatch, 1);
            renderEntities(entities[1]);
            renderParticles(particles[1]);
            map.render(camera, spriteBatch, 2);
            renderEntities(entities[2]);
            renderParticles(particles[2]);
            
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
