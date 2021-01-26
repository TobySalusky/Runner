using System.Net.Http.Headers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class Drawable3D {
        
        public Vector2 pos;

        public float zPos;

        public Vector2 dimen;

        public Texture2D texture;

        public Rectangle drawLocation(Camera camera) {

            float mult = camera.farMult(zPos);
            
            Vector2 drawDimen = dimen * camera.scale * mult;
            Vector2 drawPos = camera.screenCenter + (pos - camera.pos) * camera.scale * mult;
            
            return Util.center(drawPos, drawDimen); // hello josh
        }

        public virtual void render(Camera camera, SpriteBatch spriteBatch) {
            
            spriteBatch.Draw(texture, drawLocation(camera), Color.White);
            
        }
    }
}