using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public static List<Entity> updatedEntities = new List<Entity>();
        public static Camera camera;

        public static KeyboardState lastKeyState;
        public static MouseState lastMouseState;

        public static ChunkMap map;
        public static Player player;
        public static DeathWall backDeathWall, midDeathWall, frontDeathWall;

        public static float screenShakeTime, screenShakeStart, screenShakeIntensity;

        public static SoundEffect calm;
        public static SoundEffect chaos;

        public static SoundEffectInstance calmI;
        public static SoundEffectInstance chaosI;


        public static WiringEditor wiringEditor;
        public static bool editMode;

        public static Dictionary<string, LevelSettings> levelSettingsDict = new Dictionary<string, LevelSettings>();

        public static string[] levels = {
            "Old",
            "Sam",
            "Josh",
            "Fourth",
            "Gravity"
        };

        public static int levelIndex = 0;
        public static string levelName = levels[levelIndex];

        public static Effect gaussianBlurShader;
        public RenderTarget2D renderTarget;

        public static bool paused = false, endingPause = false;
        public static float pauseTimer;
        public const float pauseTransitionTime = 0.4F;

        public static float attemptTime;
        
        
        // UI
        public static List<UIElement> gameUI = new List<UIElement>();
        public static List<UIElement> pauseUI = new List<UIElement>();
        
        public static List<UIElement> uiElements = gameUI;

        public static List<UITransition> uiTransitions = new List<UITransition>();
        
        public Runner()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            
            instance = this;
        }

        public static void changeLevel(string newLevelName) {
            levelName = newLevelName;

            resetLevel();
        }

        public static void resetLevel() {
            
            loadLevelMap();

            // TODO: Fix: (currently says that you finished next level) {I'm lazy rn}
            Logger.log(levelName + ": " + ((player.dead) ? "Died" : "Finished") + " with time of: " + attemptTime);
            attemptTime = 0;
            
            player.deathReset();

            foreach (var list in particles) {
                list.Clear();
            }
            
            updatedEntities.Clear();
            updatedEntities.Add(player);

            frontDeathWall = new DeathWall(-5, 0);
            midDeathWall = new DeathWall(-5, -1);
            backDeathWall = new DeathWall(-5, -2);
            updatedEntities.Add(frontDeathWall);
            updatedEntities.Add(midDeathWall);
            updatedEntities.Add(backDeathWall);
        }

        public static void shakeScreen(float time, float intensity) {
            screenShakeStart = time;
            screenShakeTime = time;
            screenShakeIntensity = intensity;
        }

        public static GraphicsDevice getGraphicsDeviceManager() {
            return instance.GraphicsDevice;
        }

        public static void loadLevelMap() {
            Chunk.loadMapData(levelName);
            map = new ChunkMap();
            try {
                wiringEditor = DataSerializer.Deserialize<WiringEditor>(levelName + "Wiring");
            }
            catch (Exception e) {
                wiringEditor = new WiringEditor {
                    rects = new List<SelectRect>(), 
                    connections = new List<WireConnection>()
                };
            }
            wiringEditor.applyWiring();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            Window.IsBorderless = false;
            Window.AllowUserResizing = true;

            graphics.ApplyChanges();
            
            
            // level settings
            levelSettingsDict["Old"] = new LevelSettings {playerStartPos = new Vector2(30, 40)};
            levelSettingsDict["Sam"] = new LevelSettings { playerStartPos = new Vector2(10, 70) };
            levelSettingsDict["Gravity"] = new LevelSettings { playerStartPos = new Vector2(15, 94) };
            
            foreach (var level in levels) {
                if (!levelSettingsDict.Keys.Contains(level)) {
                    levelSettingsDict[level] = new LevelSettings();
                }
            }
            
            
            base.Initialize();

            Textures.loadTextures();
            Tile.exportTilePalette();
            SoundPlayer.loadEffects();
            
            calm = SoundPlayer.getEffect("CalmSong");
            chaos = SoundPlayer.getEffect("ChaosSong");

            calmI = calm.CreateInstance();
            chaosI = chaos.CreateInstance();

            calmI.IsLooped = true;
            chaosI.IsLooped = true;

            calmI.Play();
            calmI.Volume = 1.0F;
            chaosI.Play();
            chaosI.Volume = 0.0F;

            camera = new Camera(Vector2.Zero, 5);
            player = new Player(playerStartPos());

            for (int i = 0; i < particles.Length; i++) {
                particles[i] = new List<Particle>();
            }

            resetLevel();
            
            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            // UI
            pauseUI.Add(new UIButton(() => Logger.log("hi"), new Vector2(300, 200), new Vector2(300, 100)));
            pauseUI.Add(new UIButton(() => Logger.log("hi"), new Vector2(400, 310), new Vector2(300, 100)));
            pauseUI.Add(new UIButton(() => Logger.log("hi"), new Vector2(500, 420), new Vector2(300, 100)));
        }

        public static Vector2 playerStartPos() {
            return currentLevelSettings().playerStartPos;
        }

        public static LevelSettings currentLevelSettings() {
            return levelSettingsDict[levelName];
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            gaussianBlurShader = Shaders.gaussianBlur(Content);
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
        
        public void updateEntities(float deltaTime) {
            for (int i = updatedEntities.Count - 1; i >= 0; i--) {
                Entity entity = updatedEntities[i];

                if (entity.deleteFlag) {
                    updatedEntities.RemoveAt(i);
                    continue;
                }

                entity.update(deltaTime);
            }
        }

        public void setMusicFade(float calmVol) {
            calmI.Volume = calmVol;
            chaosI.Volume = 1 - calmVol;
        }

        public void adjustMusicFade() {
            float calmVol = 1;
            float xWallDiff = Math.Abs(player.pos.X - midDeathWall.pos.X);
            const float musicFadeStart = 40, musicFadeEnd = 70;
            if (xWallDiff < musicFadeEnd) {
                if (xWallDiff < musicFadeStart) {
                    calmVol = 0;
                }
                else {
                    calmVol = Util.sinSmooth((xWallDiff - musicFadeStart), (musicFadeEnd - musicFadeStart));
                }
            }
            setMusicFade(calmVol);
        }

        public void startEditMode() {
            backDeathWall.vel = Vector2.Zero;
            midDeathWall.vel = Vector2.Zero;
            frontDeathWall.vel = Vector2.Zero;

            Vector2 pos = new Vector2(-1000000, 0);
            backDeathWall.pos = pos;
            midDeathWall.pos = pos;
            frontDeathWall.pos = pos;

            editMode = true;
        }

        protected override void Update(GameTime gameTime) {
            
            base.Update(gameTime);

            float deltaTime = delta(gameTime);

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            KeyInfo keys = new KeyInfo(keyState, lastKeyState);
            MouseInfo mouse = new MouseInfo(mouseState, lastMouseState);
            
            lastKeyState = keyState;
            lastMouseState = mouseState;

            if (keys.down(Keys.Escape))
                Exit();

            if (keys.pressed(Keys.L)) 
                startEditMode();
            
            if (keys.pressed(Keys.S) && keys.down(Keys.LeftControl))
                wiringEditor?.saveWiring(levelName);

            if (keys.pressed(Keys.T)) {
                wiringEditor?.applyWiring();
            }

            if (keys.pressed(Keys.R))
                player.die();

            
            // debug change level
            if (keys.pressed(Keys.Left))
                lastLevel();
            if (keys.pressed(Keys.Right))
                nextLevel();
            
            
            // UI
            for (int i = uiTransitions.Count - 1; i >= 0; i--) {
                UITransition transition = uiTransitions[i];

                if (transition.deleteFlag) {
                    uiTransitions.RemoveAt(i);
                    continue;
                }

                transition.update(deltaTime);
            }
            
            foreach (var element in uiElements) {
                element.update(mouse, keys, deltaTime);
            }
            
            if (keys.pressed(Keys.P)) {

                if (!paused) {
                    paused = true;
                    pauseTimer = 0;
                    uiElements = pauseUI;
                    UI.transitionAll(uiElements, element => new SlideIn(element) {endTime = pauseTransitionTime});
                }
                else {
                    endingPause = true;
                    UI.transitionAll(uiElements, element => new SlideOut(element) {endTime = pauseTransitionTime});
                }
            }

            if (paused && endingPause && uiTransitions.Count == 0) {
                paused = false;
                endingPause = false;
                uiElements = gameUI;
            }

            if (paused) {
                if (endingPause)
                    pauseTimer -= deltaTime;
                else
                    pauseTimer += deltaTime;
            }

            pauseTimer = Math.Min(pauseTimer, pauseTransitionTime);


            // Must Be Un-paused to Run Following Code =======
            if (paused) return;
            
            if (!player.dead)
                attemptTime += deltaTime;

            adjustMusicFade();

            if (mouse.leftPressed) {
                player.pos = camera.toWorld(mouse.pos, player.zPos);
            }

            player.input(keys, deltaTime);

            if (editMode)
                wiringEditor?.input(mouse, keys, deltaTime);
            
            updateEntities(deltaTime);
            
            updateParticles(deltaTime);
            
            map.update(deltaTime);

            camera.pos = player.pos - Vector2.UnitY * 5;
            // screen shake
            if (screenShakeTime > 0) {
                screenShakeTime -= deltaTime;
                camera.pos += Util.polar(screenShakeIntensity * screenShakeTime / screenShakeStart, Util.randomAngle());
            }

            Vector2 diff = camera.screenCenter / (camera.scale * camera.farMult(-2));
            float clampCameraBottom = ChunkMap.mapHeight() - 1 - diff.Y;
            float clampCameraTop = 1 + diff.Y;
            camera.pos.Y = Math.Clamp(camera.pos.Y, clampCameraTop, clampCameraBottom);

            for (int i = 0; i < entities.Length; i++) {
                entities[i] = new List<Entity>();
            }

            foreach (var entity in updatedEntities) {
                sortIntoEntities(entity);
            }
        }

        public static void lastLevel() {
            levelIndex--;
            if (levelIndex < 0) levelIndex = levels.Length - 1;

            levelName = levels[levelIndex];
            
            changeLevel(levelName);
        }
        
        public static void nextLevel() {
            levelIndex++;
            levelIndex %= levels.Length;

            levelName = levels[levelIndex];
            
            changeLevel(levelName);
        }

        public void sortIntoEntities(Entity entity) {
            entities[entity.getLayer()].Add(entity);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(renderTarget);
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null, null, null, null);

            map.render(camera, spriteBatch, 0);
            renderEntities(entities[0]);
            renderParticles(particles[0]);
            map.render(camera, spriteBatch, 1);
            renderEntities(entities[1]);
            renderParticles(particles[1]);
            map.render(camera, spriteBatch, 2);
            renderEntities(entities[2]);
            renderParticles(particles[2]);
            
            if (editMode)
                wiringEditor?.render(camera, spriteBatch);
            
            spriteBatch.End();
            
            GraphicsDevice.SetRenderTarget(null);
            
            
            // POST-PROCESSING
            Effect shader = shouldBlur() ? gaussianBlurShader : null;

            if (shouldBlur()) {
                Shaders.setGaussianOffsets(2 * Util.sinSmooth(pauseTimer, pauseTransitionTime));
            }

            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null, null, shader, null);
            
            spriteBatch.Draw(renderTarget, new Rectangle(0,0,1920,1080), Color.White);
            
            spriteBatch.End();
            
            
            // Rendering UI
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null, null, null, null);
            foreach (var element in uiElements) {
                element.render(spriteBatch);
            }
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public bool shouldBlur() {
            return paused;
        }
    }
}
