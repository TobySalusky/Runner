using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Runner {
    public class Player : Entity {

        public float speed = 30, jumpHeight = 5;
        public float jumpTime;
        
        public const float jumpTimeStart = 0.6F;

        public bool dead;
        public float deathTime;

        public float switchTime;
        public const float switchTimeStart = 0.1F;
        public float switchFrom, switchTo;

        public float rotation;
        public float rotSpeed;

        public Player(Vector2 pos) : base(pos, -1) {

            texture = Textures.get("Player");
            dimen = Util.dimen(texture);
            
            hasStep = false;
        }

        public override void update(float deltaTime) {

            float rotMult = (grounded) ? 1 : 0.5F;
            float toRotSpeed = (vel.X / 20) * Maths.twoPI * rotMult;
            float rotAccelMult = (grounded) ? 10 : 3F;

            rotSpeed += (toRotSpeed - rotSpeed) * deltaTime * rotAccelMult;
            rotation += rotSpeed * deltaTime;
            
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

            if (pos.Y > Chunk.mapData[0].GetLength(1) - 10) {
                vel = -Vector2.UnitY * 20;
                die();
            }

            switchTime -= deltaTime;
            if (switchTime > 0) {
                float toZ = Util.revSinLerp(switchTime, switchTimeStart, switchFrom, switchTo);
                if (!tryMoveToZ(toZ)) {
                    startSwitchTo(switchFrom);
                }
            }
        }

        public void startSwitchTo(float switchTo) {

            int to = (int) Math.Round(switchTo);

            if (to >= -2 && to <= 0) {
                switchTime = switchTimeStart;
                switchFrom = zPos;
                this.switchTo = to;
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
        
        public void puffDeath() {

            var colorArray = Util.colorArray(texture);
            
            Vector2 tMid = new Vector2(texture.Width, texture.Height) / 2;
            float angle = Util.angle(vel);
            float mag = Math.Clamp(Util.mag(vel) / 10, 3, 7);
            
            for (int i = 0; i < texture.Width; i++) {
                for (int j = 0; j < texture.Height; j++) {
                    int index = i + j * texture.Width;

                    Color color = colorArray[index];

                    if (color.A != 0) {
                        Vector2 add = (new Vector2(i, j) - tMid) * Tile.pixelSize;
                        /*if (!facingLeft) {
                            add.X *= -1;
                        }*/

                        Vector2 pPos = pos + Util.rotate(add, rotation);

                        Particle particle = new RubbleParticle(pPos, zPos, vel + Util.polar(1, Util.randomAngle()), color);
                        
                        Runner.particles[getLayer(zPos)].Add(particle);
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

            if (switchTime > 0) {
                zPos = Util.lessDiff(zPos, switchTo, switchFrom); // TODO: make this look better (only not noticeable since switch is so fast)
                switchTime = -1;
            }

            puffDeath();
            texture = Textures.get("invis");
            
            Runner.shakeScreen(0.4F, 0.6F);
        }

        public void deathReset() {
            pos = Runner.playerStartPos();
            zPos = -1;
            vel = Vector2.Zero;
            texture = Textures.get("Player");

            rotation = 0;
            rotSpeed = 0;
        }

        public virtual void jump(float jumpHeight) {
            vel.Y -= Util.heightToJumpPower(jumpHeight, gravity);
            jumpTime = jumpTimeStart;
        }
        
        public bool tryMoveToZ(float toZ) {
            if (!collidesAt(pos, getLayer(toZ))) {
                zPos = toZ;
                return true;
            }

            return false;
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) {
            
            Util.render(texture, pos, dimen, zPos, rotation, camera, spriteBatch);
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
                startSwitchTo(zPos - 1);
            
            if (keys.pressed(Keys.S))
                startSwitchTo(zPos + 1);
            
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