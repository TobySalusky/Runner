using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class DeathWall : Entity {

        public Vector2 crusherDimen, armDimen;
        public Texture2D crusher, armSegment;

        public float crushTimer;
        
        public DeathWall(float xPos, float zPos) : base(new Vector2(xPos, ChunkMap.mapHeight() / 2F), zPos) {
            dimen = new Vector2(10, ChunkMap.mapHeight());
            
            vel = Vector2.UnitX * 10;
            hasGravity = false;
            hasCollision = false;

            crusher = Textures.get("DeathWallCrusher");
            armSegment = Textures.get("DeathWallArm");
            crusherDimen = new Vector2(12, 4);
            armDimen = new Vector2(6, 3);
        }

        public float crushAmount() {
            return ((int) crushTimer % 2 == 0) ? crushTimer % 1 : (1 - (crushTimer % 1));
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            if (collidesWith(Runner.player)) {
                Runner.player.vel = Vector2.UnitX * 20;
                Runner.player.die();
            }

            crushTimer += deltaTime * 2;
        }

        public void renderArm(Camera camera, SpriteBatch spriteBatch, Vector2 start, int dir) {

            Vector2 diff = new Vector2(1920, 1080) / camera.scaleAt(zPos) * 0.5F;
            Vector2 tl = camera.pos - diff;
            Vector2 br = tl + diff * 2;
            
            Vector2 incAmount = Vector2.UnitY * (armDimen.Y - 0.05F);

            for (int i = 0; i < 20; i++) {
                Vector2 segPos = start + dir * i * incAmount;
                
                if (segPos.Y + armDimen.Y / 2 < tl.Y || segPos.Y - armDimen.Y / 2 > br.Y) break;
                spriteBatch.Draw(armSegment, camera.toScreen(segPos, zPos, armDimen), Color.White);
            }
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            //base.render(camera, spriteBatch);

            float extend = dimen.Y / 2 - crusherDimen.Y / 2;
            float amount = crushAmount();
            Vector2 topCrush = new Vector2(pos.X, extend * amount);
            Vector2 bottomCrush = new Vector2(pos.X, ChunkMap.mapHeight() - extend * amount);
            
            renderArm(camera, spriteBatch, topCrush - Vector2.UnitY * (crusherDimen.Y / 2 + armDimen.Y / 2), -1);
            renderArm(camera, spriteBatch, bottomCrush + Vector2.UnitY * (crusherDimen.Y / 2 + armDimen.Y / 2), 1);
            
            spriteBatch.Draw(crusher, camera.toScreen(topCrush, zPos, crusherDimen), Color.White);
            spriteBatch.Draw(crusher, camera.toScreen(bottomCrush, zPos, crusherDimen), Color.White);
            
        }
    }
}