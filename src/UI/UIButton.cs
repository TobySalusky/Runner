using System;
using Microsoft.Xna.Framework;

namespace Runner {
    public class UIButton : UIElement {
        
        public Action clickFunc;

        public bool hoverGrow;
        public float hoverTime;
        public const float hoverSpeed = 5;
        public const float hoverMult = 1.1F;
        
        public UIButton(Action clickFunc, Vector2 pos, Vector2 dimen, bool hoverGrow = true) {
            this.clickFunc = clickFunc;
            this.pos = pos;
            this.dimen = dimen;
            this.hoverGrow = hoverGrow;

            startPos = pos;
            startDimen = dimen;
            
            texture = Textures.get("pixel");
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
            clickFunc.Invoke();
        }
    }
}