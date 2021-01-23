using Microsoft.Xna.Framework;

namespace Runner {
    public class Entity : Drawable3D {

        public Vector2 vel;
        
        public Entity(Vector2 pos, float zPos) {
            this.pos = pos;
            dimen = Vector2.One * 5;

            texture = Textures.nullTexture;

            this.zPos = zPos;
        }

    }
}