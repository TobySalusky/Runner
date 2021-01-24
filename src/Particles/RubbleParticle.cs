using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class RubbleParticle : PixelParticle {

        public float alpha = 1;
        
        public RubbleParticle(Vector2 pos, float zPos, Vector2 vel, Color tint) : base(pos, zPos, vel, tint) {
            hasCollision = true;
            bonkMult = -0.6F;

            timeLeft = Util.random(0.6F, 1F);
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            float accelSpeed = (collidesAt(pos + Vector2.UnitY * 0.1F)) ? 5 : 2.5F;
            vel.X -= vel.X * deltaTime * accelSpeed;

            vel.Y += Entity.gravity * deltaTime;

            const float shrinkStart = 0.3F;
            if (timeLeft < shrinkStart) {
                alpha = timeLeft / shrinkStart;
            }
        }
        
        public override void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed
            
            Util.render(texture, pos, dimen, zPos, rotation, camera, spriteBatch, new Color(tint, alpha));
        }
    }
}