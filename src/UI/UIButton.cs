using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class UIButton : UIElement {
        
        public Action clickFunc;

        public bool hoverGrow;
        public float hoverTime;
        public const float hoverSpeed = 5;
        public const float hoverMult = 1.1F;
        public string text;
        public Color color = new Color(Color.Black, 0.5F);
        public Color textColor = Color.White;
        public SpriteFont font = Fonts.arial;
        
        public UIButton(Action clickFunc, Vector2 pos, Vector2 dimen, string text = "") {
            this.clickFunc = clickFunc;
            this.pos = pos;
            this.dimen = dimen;
            this.text = text;

            hoverGrow = true;
            
            startPos = pos;
            startDimen = dimen;
            
            texture = Textures.rect;
        }

        public override void render(SpriteBatch spriteBatch) {
            spriteBatch.Draw(texture, drawRect(), color);
            Vector2 nameDimen = font.MeasureString(text);
            spriteBatch.DrawString(font, text, pos - nameDimen / 2, textColor);
        }

        public override void update(MouseInfo mouse, KeyInfo keys, float deltaTime) {

            if (hoverGrow) {
                hoverTime = Math.Clamp(hoverTime, 0, 1);
                dimen = Util.sinLerp(hoverTime, startDimen, startDimen * hoverMult);
            }

            base.update(mouse, keys, deltaTime);
        }

        public override void hovered(MouseInfo mouse, KeyInfo keys, float deltaTime) {
            hoverTime += deltaTime * hoverSpeed;
        }

        public override void notHovered(MouseInfo mouse, KeyInfo keys, float deltaTime) {
            hoverTime -= deltaTime * hoverSpeed;
        }
        
        public override void clicked(MouseInfo mouse, KeyInfo keys, float deltaTime) {
            clickFunc?.Invoke();
        }
    }
}