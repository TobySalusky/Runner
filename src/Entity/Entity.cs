using System;
using Microsoft.Xna.Framework;

namespace Runner {
    public class Entity : Drawable3D {

        public Vector2 vel;
        
        private const float collisionStep = 0.1F;
        public bool hasStep = true;
        public float maxStepHeight = 1.1F;

        public bool grounded;
        public const float gravity = 70F;

        public bool deleteFlag = false;
        public bool hasGravity = true, hasCollision = true;
        public int gravityDir = 1;
        
        public Entity(Vector2 pos, float zPos) {
            this.pos = pos;
            dimen = Vector2.One * 2;

            texture = Textures.nullTexture;

            this.zPos = zPos;
        }
        
        protected bool collidesWith(Entity entity) {
            return collidesWith(entity, pos, dimen);
        }
        
        protected bool collidesWith(Entity entity, Vector2 pos, Vector2 dimen) {
            return entity.getLayer() == getLayer() && (pos.X + dimen.X / 2 > entity.pos.X - entity.dimen.X / 2 &&
                                                       pos.X - dimen.X / 2 < entity.pos.X + entity.dimen.X / 2 &&
                                                       pos.Y + dimen.Y / 2 > entity.pos.Y - entity.dimen.Y / 2 &&
                                                       pos.Y - dimen.Y / 2 < entity.pos.Y + entity.dimen.Y / 2);
        }

        public int getLayer() {
            return getLayer(zPos);
        }
        
        public static int getLayer(float zPos) {
            if (zPos > -0.5F) return 2;
            if (zPos > -1.5F) return 1;
            return 0;
        }

        public bool collidesAt(Vector2 pos, Vector2 dimen, int layer) {
            return Runner.map.rectangleCollide(pos, dimen, layer);
        }

        public bool collidesAt(Vector2 pos, int layer) {
            return collidesAt(pos, dimen, layer);
        }
        
        public bool collidesAt(Vector2 pos, Vector2 dimen) {
            return collidesAt(pos, dimen, getLayer());
        }

        public bool collidesAt(Vector2 pos) {
            return collidesAt(pos, dimen);
        }
        
        public virtual void bonk(Vector2 newPos) {
            
        }

        public virtual void bonkX(Vector2 newPos) {
            vel.X = 0;
            bonk(newPos);
        }
        
        public virtual void bonkY(Vector2 newPos) {

            vel.Y = 0;
            bonk(newPos);
        }

        public virtual void update(float deltaTime) {
            
            grounded = collidesAt(pos + Vector2.UnitY * 0.1F * gravityDir);
            if (hasGravity)
                vel.Y += gravity * deltaTime * gravityDir; // gravity

            if (hasCollision) {
                collisionMove(vel * deltaTime);
            } else {
                pos += vel * deltaTime;
            }
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

                            if (hasStep && collidesAt(pos + Vector2.UnitY, dimen)) { // stepping up single blocks
                                for (float step = 0; step <= maxStepHeight; step += collisionStep) {
                                    if (!collidesAt(pos + new Vector2(diffX, -step))) {
                                        pos += new Vector2(diffX, -step);
                                        collisionMove(fullDiff - Vector2.UnitX * diffX);
                                        return;
                                    }
                                }
                            }

                            diffX -= stepX;
                            bonkX(pos + Vector2.UnitX * diffX); // bonking
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
                            bonkY(pos + Vector2.UnitY * diffY); // bonking
                            break;
                        }
                    }

                    pos += Vector2.UnitY * diffY;
                }
            }
        }
        

    }
}