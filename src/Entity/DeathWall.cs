using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class DeathWall : Entity {
        public DeathWall(Vector2 pos, float zPos) : base(pos, zPos) {
            dimen = new Vector2(10, ChunkMap.mapHeight());
            
            vel = Vector2.UnitX * 5;
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            base.render(camera, spriteBatch);
        }
    }
}