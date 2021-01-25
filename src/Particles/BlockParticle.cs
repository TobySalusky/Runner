using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class BlockParticle : Particle {
        
        public float alpha = 1;
        public float rotSpeed;
        public Rectangle rect;
        
        public BlockParticle(Tile tile, Vector2 vel) : base(tile.pos + Vector2.One * 0.5F, tile.zPos(), vel, tile.texture) {
            rotSpeed = Util.random(0.5F, 1) * Maths.twoPI;
            dimen = Util.dimen(texture);
            rect = tile.atlasRect;
            rotSpeed = 0;
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            rotation += rotSpeed * deltaTime;
            
            vel.Y += Entity.gravity * deltaTime;

            const float shrinkStart = 0.3F;
            if (timeLeft < shrinkStart) {
                alpha = timeLeft / shrinkStart;
            }
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed
            
            Util.render(texture, pos, dimen, zPos, rotation, camera, spriteBatch, new Color(tint, alpha), rect);
        }
    }
}