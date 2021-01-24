using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class PixelParticle : Particle {

        private static readonly Texture2D pixelTexture = Textures.get("pixel");
        
        public PixelParticle(Vector2 pos, float zPos, Vector2 vel, Color tint) : base(pos, zPos, vel, pixelTexture) {
            this.tint = tint;
        }
        
        public override void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed
            
            Util.render(texture, pos, dimen, zPos, rotation, camera, spriteBatch, tint);
        }
    }
}