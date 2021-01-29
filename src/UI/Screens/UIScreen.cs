using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class UIScreen {
        
        public List<UIElement> uiElements = new List<UIElement>();
        public List<UITransition> uiTransitions = new List<UITransition>();


        public virtual void update(MouseInfo mouse, KeyInfo keys, float deltaTime) {

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
        }

        public virtual void render(Game game, SpriteBatch spriteBatch) {
            // Rendering UI
            
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp,
                null, null, null, null);
            foreach (var element in uiElements) {
                element.render(spriteBatch);
            }
            spriteBatch.End();
        }
    }


    public class MainMenuScreen : UIScreen {

        public MainMenuScreen() {
            uiElements.Add(new UIButton(() => Runner.uiScreen = null, new Vector2(300, 300), new Vector2(200, 100), "Start Game"));
        }

    }
    
    public class LevelSelectScreen : UIScreen {

        public LevelSelectScreen() {
            int perRow = 5;

            for (int i = 0; i < Runner.levels.Length; i++) {
                int r = i / perRow;
                int c = i % perRow;

                int index = i;
                uiElements.Add(new UIButton(() => {
                    Runner.changeLevel(Runner.levels[index]);
                    Runner.uiScreen = null;
                }, new Vector2(300, 300) + new Vector2(170, 170) * new Vector2(c, r), new Vector2(150, 150), "Level " + (i + 1)));
            }
        }

    }
}