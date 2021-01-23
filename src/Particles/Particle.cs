using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    
    public class Particle {
        
        public Vector2 pos, vel, dimen;

        public float zPos;
        
        public float rotation = 0;
        
        protected Texture2D texture;

        public bool deleteFlag;

        public float timeLeft = 3;

        public Color tint = Color.White;
        
        private const float collisionStep = 0.1F;
        public bool hasCollision = false; // defaults to point-collision
        public float bonkMult = 0;

        public Particle(Vector2 pos, float zPos, Vector2 vel, Texture2D texture) {
            this.pos = pos;
            this.zPos = zPos;
            this.vel = vel;
            
            this.texture = texture;
            dimen = new Vector2(texture.Width, texture.Height) / Tile.pixelCount;
        }

        public virtual void changeRotation() { }
        
        public virtual void update(float deltaTime) {

            timeLeft -= deltaTime;
            if (timeLeft <= 0) {
                deleteFlag = true;
            }

            if (hasCollision) {
                collisionMove(vel * deltaTime);
            }
            else {
                pos += vel * deltaTime;
            }

            changeRotation();
        }

        public bool collidesAt(Vector2 pos) {
            return Runner.map.pointCollide(pos, Entity.getLayer(zPos));
        }

        public virtual void render(Camera camera, SpriteBatch spriteBatch) { // TODO: perhaps use more efficient drawing unless needed
            
            Util.render(texture, pos, dimen, zPos, rotation, camera, spriteBatch);
        }

        public virtual Color findTint() {
            return tint;
        }
        
        protected void collisionMove(Vector2 fullDiff) {
            if (!collidesAt(pos + fullDiff)) { // TODO: improve (can result in clipping [due to initial skip])
                pos += fullDiff;
            } else {

                float diffX = 0, diffY = 0;
                float stepX = collisionStep * Math.Sign(fullDiff.X), stepY = collisionStep * Math.Sign(fullDiff.Y);
                
                // x-component
                if (!collidesAt(pos + Vector2.UnitX * fullDiff.X)) {
                    pos += Vector2.UnitX * fullDiff.X;
                } else {
                    for (int i = 0; i < Math.Abs(fullDiff.X) / collisionStep; i++) {
                        diffX += stepX;
                        if (collidesAt(pos + Vector2.UnitX * diffX)) {

                            diffX -= stepX;
                            vel.X *= bonkMult; // bonking
                            break;
                        }
                    }

                    pos += Vector2.UnitX * diffX;
                }

                // y-component
                if (!collidesAt(pos + Vector2.UnitY * fullDiff.Y)) {
                    pos += Vector2.UnitY * fullDiff.Y;
                } else {
                    for (int i = 0; i < Math.Abs(fullDiff.Y) / collisionStep; i++) {
                        diffY += stepY;
                        if (collidesAt(pos + Vector2.UnitY * diffY)) {
                            diffY -= stepY;
                            vel.Y *= bonkMult; // bonking
                            break;
                        }
                    }

                    pos += Vector2.UnitY * diffY;
                }
            }
        }

    }
}