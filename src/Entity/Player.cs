using System;
using System.Linq;
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

        public float frontPercentOcluded, midPercentOcluded;

        public float animationTimer = 3;

        public bool godMode = false;

        public Player(Vector2 pos) : base(pos, -1) {

            texture = Textures.get("Player");
            dimen = Util.dimen(texture);
            
            hasStep = false;
        }

        public override void bonkY(Vector2 newPos) {
            bonk(newPos);

            if (vel.Y > 0) {
                Point from = ChunkMap.blockIndices(pos + new Vector2(-0.5F, 0.5F) * dimen);
                Point to = ChunkMap.blockIndices(pos + new Vector2(0.5F, 0.5F) * dimen);

                bool allRubber = true;
                
                for (int i = from.X; i <= to.X; i++) {
                    for (int j = from.Y; j <= to.Y; j++) {
                        Tile tile = Runner.map.getTile(new Point(i, j), getLayer(zPos));

                        if (tile.tileType != Tile.type.Rubber) {
                            allRubber = false;
                            goto loopEnd;
                        }
                    }
                }
                loopEnd:
                if (allRubber) {
                    vel.Y *= -0.9F;
                    return;
                }
            }

            vel.Y = 0;
        }

        public override void update(float deltaTime) {

            hasCollision = !godMode;
            hasGravity = !godMode;
            
            float rotMult = (grounded) ? 1 : 0.5F;
            float toRotSpeed = (vel.X / 20) * Maths.twoPI * rotMult;
            float rotAccelMult = (grounded) ? 10 : 3F;

            rotSpeed += (toRotSpeed - rotSpeed) * deltaTime * rotAccelMult;
            rotation += rotSpeed * deltaTime;
            
            deathTime -= deltaTime;
            if (dead && deathTime <= 0) {
                Runner.failLevel();
            }

            if (dead) {
                return;
            }

            base.update(deltaTime);
            
            collideInsides();
            collideTouching();

            if (pos.Y > ChunkMap.mapHeight() - 10) {
                vel = -Vector2.UnitY * 20;
                die();
            }
            
            if (pos.Y < 10) {
                vel = Vector2.UnitY * 20;
                die();
            }


            if (!dead) {
                animationTimer -= deltaTime;
                const float blinkStart = 0.4F;
                if (animationTimer > 0 && animationTimer < blinkStart) {
                    int index = (int) ((blinkStart - animationTimer) / blinkStart * 5);
                    if (index > 2) {
                        index = Math.Max(0, 5 - index);
                    }

                    texture = Textures.get("Blink" + index);
                }
                else {
                    texture = Textures.get("Player");

                    if (animationTimer < 0) {
                        animationTimer = Util.random(3, 10);
                    }
                }
            }


            switchTime -= deltaTime;
            if (switchTime > 0) {
                float toZ = Util.revSinLerp(switchTime, switchTimeStart, switchFrom, switchTo);
                if (!tryMoveToZ(toZ)) {
                    startSwitchTo(switchFrom);
                    SoundPlayer.play("Bonk", 0.5F);
                }
            }
            
            checkOclusion();
        }

        public void startSwitchTo(float switchTo) {

            int to = (int) Math.Round(switchTo);

            if (to >= -2 && to <= 0) {
                switchTime = switchTimeStart;
                switchFrom = zPos;
                SoundPlayer.play("Whoosh",0.8F);
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
        
        public void collideTouching() { // includes inside blocks TODO: dont?
            Vector2 diff = dimen / 2 + Vector2.One * 0.2F;
            Point from = ChunkMap.blockIndices(pos - diff);
            Point to = ChunkMap.blockIndices(pos + diff);

            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    Tile tile = Runner.map.getTile(new Point(i, j), getLayer(zPos));

                    if (tile.tileType != Tile.type.Air) {
                        collideTouching(tile);
                    }
                }
            }
        }

        public void collideTouching(Tile tile) {
            Tile.type tileType = tile.tileType;

            if (tileType == Tile.type.CrackedBrick) {
                ((FallingBlock)tile).startFall();
            }
        }

        public void checkOclusion() {
            Camera camera = Runner.camera;
            Vector2 diff = dimen / 2 * camera.scaleAt(zPos);
            Vector2 tl = camera.toScreen(pos, zPos) - diff;
            Vector2 br = tl + diff * 2;

            frontPercentOcluded = 0;
            midPercentOcluded = 0;

            if (getLayer() == 2) return;
            
            Point from = ChunkMap.blockIndices(camera.toWorld(tl, 0));
            Point to = ChunkMap.blockIndices(camera.toWorld(br, 0));
            
            int blockCount = (to.X - from.X + 1) * (to.Y - from.Y + 1);
            
            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    Tile.type tileType = Runner.map.getTile(new Point(i, j), 2).tileType;

                    if (!Tile.nonFullBlock.Contains(tileType) && !Tile.transparent.Contains(tileType)) {
                        frontPercentOcluded += 1F / blockCount;
                    }
                }
            }
            
            if (getLayer() == 1) return;
            from = ChunkMap.blockIndices(camera.toWorld(tl, -1));
            to = ChunkMap.blockIndices(camera.toWorld(br, -1));
            blockCount = (to.X - from.X + 1) * (to.Y - from.Y + 1);
            
            for (int i = from.X; i <= to.X; i++) {
                for (int j = from.Y; j <= to.Y; j++) {
                    Tile.type tileType = Runner.map.getTile(new Point(i, j), 1).tileType;

                    if (!Tile.nonFullBlock.Contains(tileType) && !Tile.transparent.Contains(tileType)) {
                        midPercentOcluded += 1F / blockCount;
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
            
            if (tile.tileType == Tile.type.Button) {
                ((ButtonTile)tile).activate();
            }
            
            if (tile.tileType == Tile.type.GravityButton) {
                ((GravitySwitcher)tile).activate();
            }

            if (tile.tileType == Tile.type.NextStage) {
                Runner.finishLevel();
            }
        }

        public void die() {

            if (dead) {
                return;
            }

            deathTime = 1;
            dead = true;

            if (switchTime > 0) {
                zPos = Util.lessDiff(zPos, switchTo, switchFrom); // TODO: make this look better (only not noticeable since switch is so fast)
                switchTime = -1;
            }

            puffDeath();
            SoundPlayer.play("Explosion", 0.6F);
            texture = Textures.get("invis");
            
            Runner.shakeScreen(0.4F, 0.6F);
        }

        public void deathReset() {
            dead = false;
            
            pos = Runner.playerStartPos();
            zPos = -1;
            vel = Vector2.Zero;
            texture = Textures.get("Player");

            rotation = 0;
            rotSpeed = 0;

            gravityDir = 1;
        }

        public virtual void jump(float jumpHeight) {
            vel.Y = -Util.heightToJumpPower(jumpHeight, gravity) * gravityDir;
            SoundPlayer.play("Jump",0.7F);
            jumpTime = jumpTimeStart;
        }
        
        public bool tryMoveToZ(float toZ) {
            //Checks if player can move to next layer
            if (!hasCollision || !collidesAt(pos, getLayer(toZ))) {
                //moves to next layer if possible
                zPos = toZ;
                return true;
            }
            //fail case

            return false;
        }

        public override void render(Camera camera, SpriteBatch spriteBatch) { // TODO: tint player to color of layer
            
            Util.render(texture, pos, dimen, zPos, rotation, camera, spriteBatch);
        }

        public void input(KeyInfo keys, float deltaTime) {
            //control scheme

            if (dead) {
                return;
            }

            int inputX = 0;
            if (keys.down(Keys.A))
                inputX--;
            if (keys.down(Keys.D))
                inputX++;

            if (keys.pressed(Keys.W) || keys.pressed(Keys.Up))
                startSwitchTo(zPos - 1);
            
            if (keys.pressed(Keys.S) || keys.pressed(Keys.Down))
                startSwitchTo(zPos + 1);
           
            
            float accelSpeed = (inputX == 0 && grounded) ? 5 : 2.5F;
            vel.X += ((inputX * speed) - vel.X) * deltaTime * accelSpeed;


            if (!godMode) {
                jumpTime -= deltaTime;

                if (grounded && vel.Y >= 0 && keys.down(Keys.Space) && jumpTime < jumpTimeStart - 0.1F) {

                    jump(jumpHeight);
                }

                if (!grounded && keys.down(Keys.Space) && jumpTime > 0) {
                    float fade = jumpTime / jumpTimeStart;
                    vel.Y -= 50F * deltaTime * fade * gravityDir;
                }
            }
            else {

                int inputY = 0;
                if (keys.down(Keys.Space))
                    inputY--;
                if (keys.down(Keys.LeftShift))
                    inputY++;
                
                vel.Y += ((inputY * speed) - vel.Y) * deltaTime * accelSpeed;
            }
        }
    }
}