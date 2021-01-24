using Microsoft.Xna.Framework;

namespace Runner {
    public class Camera {

        public Vector2 pos;
        public float zPos;
        public float scale = 40;
        public Vector2 screenCenter = new Vector2(1920, 1080) / 2;

        public Camera(Vector2 pos, float zPos) {
            this.pos = pos;
            this.zPos = zPos;
        }

        public float scaleAt(float zPos) {
            return scale * farMult(zPos);
        }

        public Vector2 toScreen(Vector2 worldPos, float zPos) {
            return (worldPos - pos) * scaleAt(zPos) + screenCenter;
        }

        public Vector2 toWorld(Vector2 screenPos, float zPos) {
            return (screenPos - screenCenter) / scaleAt(zPos) + pos;
        }

        public float farMult(float zPos) {
            return 1 / ((this.zPos - zPos) * 0.25F);
        }
    }
}