using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Runner {
    public class Player : Entity {

        public float speed = 30, jumpHeight = 5;
        public float jumpTime;
        
        public const float jumpTimeStart = 0.6F;
        
        
        public Player(Vector2 pos) : base(pos, -1) { }

        public override void update(float deltaTime) {
            base.update(deltaTime);
            
            
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