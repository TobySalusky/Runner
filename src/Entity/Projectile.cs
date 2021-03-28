using Microsoft.Xna.Framework;

namespace Runner {
    public class Projectile : Entity {
        
        public Projectile(Vector2 pos, Vector2 vel, float zPos) : base(pos, zPos) {
            this.vel = vel;
            hasGravity = false;
        }

        public override void bonk(Vector2 newPos) {
            deleteFlag = true;
        }
    }
}