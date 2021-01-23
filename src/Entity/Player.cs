using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Runner {
    public class Player : Entity {

        public float speed = 30, jumpHeight = 5;
        public float jumpTime;
        
        public const float jumpTimeStart = 0.6F;

        public bool dead;
        public float deathTime;

        public float switchTime;
        public float switchFrom, switchTo;

        public Player(Vector2 pos) : base(pos, -1) {

            hasStep = false;
        }

        public override void update(float deltaTime) {

            deathTime -= deltaTime;
            if (dead && deathTime <= 0) {
                dead = false;
                deathReset();
            }

            if (dead) {
                return;
            }

            base.update(deltaTime);
            
            collideInsides();

            if (pos.Y > Chunk.mapData[0].GetLength(1)) {
                die();
            }
        }

        public void collideInsides() {
            Vector2 diff = dimen / 2;
            Point from = ChunkMap.blockIndices(pos - diff);
            Point to = ChunkMap.blockIndices(pos + diff);
            
            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    Tile tile = Runner.map.getTile(new Point(i, j), getLayer(zPos));

                    if (tile.tileType != Tile.type.Air) {
                        collideInside(tile);
                    }
                }
            }
        }

        public void collideInside(Tile tile) {
            if (tile.tileType == Tile.type.Spike) {
                die();
            }
        }

        public void die() {
            deathTime = 1;
            dead = true;
            texture = Textures.get("invis");
        }

        public void deathReset() {
            pos = Runner.playerStartPos();
            zPos = -1;
            vel = Vector2.Zero;
            texture = Textures.get("Player");
        }

        public virtual void jump(float jumpHeight) {
            vel.Y -= Util.heightToJumpPower(jumpHeight, gravity);
            jumpTime = jumpTimeStart;
        }

        public void tryMoveToZ(float toZ) {
            if (toZ <= 0 && toZ >= -2) {
                if (!collidesAt(pos, getLayer(toZ))) {
                    zPos = toZ;
                }
            }
        }

        public void input(KeyInfo keys, float deltaTime) {

            if (dead) {
                return;
            }
            
            int inputX = 0;
            if (keys.down(Keys.A))
                inputX--;
            if (keys.down(Keys.D))
                inputX++;
            
            if (keys.pressed(Keys.W))
                tryMoveToZ(zPos - 1);
            
            if (keys.pressed(Keys.S))
                tryMoveToZ(zPos + 1);
            
            float accelSpeed = (inputX == 0 && grounded) ? 5 : 2.5F;
            vel.X += ((inputX * speed) - vel.X) * deltaTime * accelSpeed;


            jumpTime -= deltaTime;
            
            if (grounded && vel.Y >= 0 && keys.down(Keys.Space) && jumpTime < jumpTimeStart - 0.1F) {

                jump(jumpHeight);
            }

            if (!grounded && keys.down(Keys.Space) && jumpTime > 0) {
                float fade = jumpTime / jumpTimeStart;
                vel.Y -= 50F * deltaTime * fade;
            }
        }
    }
}