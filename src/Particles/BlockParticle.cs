using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class BlockParticle : Particle {
        
        public float alpha = 1;
        public float rotSpeed;
        public Rectangle rect;
        
        public BlockParticle(Tile tile, Vector2 vel) : base(tile.pos + Vector2.One * 0.5F, tile.zPos(), vel, tile.texture) {
            rotSpeed = Util.random(-1.5F, 1.5F) * Maths.twoPI;
            dimen = Util.dimen(texture);
            rect = tile.atlasRect;
            timeLeft = Util.random(0.5F, 0.8F);

            tint = Tile.layerColors[tile.layer];

            vel += Util.polar(Util.random(0, 2), Util.randomAngle());
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

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            
            Vector2 origin = new Vector2(rect.X + rect.Width / 2F, rect.Y + rect.Height / 2F);
            Util.render(texture, pos, dimen, zPos, rotation, camera, spriteBatch, new Color(tint, alpha), rect);
        }
    }
}