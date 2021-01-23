using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Runner {
    public class Player : Entity {

        public float speed = 30, jumpHeight = 2;
        
        
        public Player(Vector2 pos) : base(pos, -1) {
            
        }

        public override void update(float deltaTime) {
            base.update(deltaTime);
        }
        
        public virtual void jump(float jumpHeight) {
            vel.Y -= Util.heightToJumpPower(jumpHeight, gravity);
        }

        public void input(KeyInfo keys, float deltaTime) {

            int inputX = 0;
            if (keys.down(Keys.A))
                inputX--;
            if (keys.down(Keys.D))
                inputX++;
            
            float accelSpeed = (inputX == 0 && grounded) ? 5 : 2.5F;
            vel.X += ((inputX * speed) - vel.X) * deltaTime * accelSpeed;

            if (grounded && keys.down(Keys.Space)) {
                jump(jumpHeight);
            }
        }
    }
}