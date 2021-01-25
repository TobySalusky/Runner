using System;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class DeathWall : Entity {

        public Vector2 crusherDimen, armDimen;
        public Texture2D crusher, armSegment;

        public float crushTimer;
        public Vector2 topCrush, bottomCrush;
        
        public DeathWall(float xPos, float zPos) : base(new Vector2(xPos, ChunkMap.mapHeight() / 2F), zPos) {

            vel = Vector2.UnitX * 10;
            hasGravity = false;
            hasCollision = false;

            crusher = Textures.get("DeathWallCrusher");
            armSegment = Textures.get("DeathWallArm");
            crusherDimen = Util.dimen(crusher);
            armDimen = Util.dimen(armSegment);
            
            dimen = new Vector2(armDimen.X, ChunkMap.mapHeight());
        }

        public float crushAmount() {
            return ((int) crushTimer % 2 == 0) ? crushTimer % 1 : (1 - (crushTimer % 1));
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            Player player = Runner.player;

            crushTimer += deltaTime * 2;
            
            float extend = dimen.Y / 2 - crusherDimen.Y / 2;
            float amount = crushAmount();
            topCrush = new Vector2(pos.X, extend * amount);
            bottomCrush = new Vector2(pos.X, ChunkMap.mapHeight() - extend * amount);
            
            if (collidesWith(player, topCrush, crusherDimen)) { // collides with top crusher
                Runner.player.vel = Vector2.UnitY * 20;
                Runner.player.die();
            }
            else if (collidesWith(player, bottomCrush, crusherDimen)) { // collides with top crusher
                Runner.player.vel = -Vector2.UnitY * 20;
                Runner.player.die();
            }
            else if (collidesWith(player) && !collidesWith(player, pos, new Vector2(dimen.X, bottomCrush.Y - topCrush.Y))) { // collides with arm
                Runner.player.vel = Vector2.UnitX * 20 * Math.Sign(player.pos.X - pos.X);
                Runner.player.die();
            }
            
            Point from = ChunkMap.blockIndices(topCrush - crusherDimen / 2);
            Point to = ChunkMap.blockIndices(topCrush + crusherDimen / 2);

            for (int x = from.X; x <= to.X; x++) {
                for (int y = from.Y; y <= to.Y; y++) {
                    Vector2 blockPos = new Vector2(x, y);
                    Runner.map.removeBlocks(blockPos, new Vector2(10, 10));
                }
            }
            from = ChunkMap.blockIndices(bottomCrush - crusherDimen / 2);
            to = ChunkMap.blockIndices(bottomCrush + crusherDimen / 2);
            for (int x = from.X; x <= to.X; x++) {
                for (int y = from.Y; y <= to.Y; y++) {
                    Vector2 blockPos = new Vector2(x, y);
                    Runner.map.removeBlocks(blockPos, new Vector2(10, -10));
                }
            }

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
            
            renderArm(camera, spriteBatch, topCrush - Vector2.UnitY * (crusherDimen.Y / 2 + armDimen.Y / 2), -1);
            renderArm(camera, spriteBatch, bottomCrush + Vector2.UnitY * (crusherDimen.Y / 2 + armDimen.Y / 2), 1);
            
            spriteBatch.Draw(crusher, camera.toScreen(topCrush, zPos, crusherDimen), Color.White);
            spriteBatch.Draw(crusher, camera.toScreen(bottomCrush, zPos, crusherDimen), Color.White);
            
        }
    }
}