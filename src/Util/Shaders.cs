using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public static class Shaders {

        public static Effect gaussianBlur(ContentManager Content) { 
            Effect shader = Content.Load<Effect>("Gaussian");

            Vector2 vec = new Vector2(1920, 1080);
            float mult = 2;
            float pixX = 1 / vec.X * mult;
            float pixY = 1 / vec.Y * mult;
            shader.Parameters["sampleOffsets"].SetValue(new Vector2[] {
                new Vector2(-pixX * 2, -pixY * 2),
                new Vector2(-pixX, -pixY * 2),
                new Vector2(0, -pixY * 2),
                new Vector2(pixX, -pixY * 2),
                new Vector2(pixX * 2, -pixY * 2),
                
                new Vector2(-pixX * 2, -pixY),
                new Vector2(-pixX, -pixY),
                new Vector2(0, -pixY),
                new Vector2(pixX, -pixY),
                new Vector2(pixX * 2, -pixY),
                
                new Vector2(-pixX * 2, 0),
                new Vector2(-pixX, 0),
                new Vector2(0, 0),
                new Vector2(pixX, 0),
                new Vector2(pixX * 2, 0),
                
                new Vector2(-pixX * 2, pixY),
                new Vector2(-pixX, pixY),
                new Vector2(0, pixY),
                new Vector2(pixX, pixY),
                new Vector2(pixX * 2, pixY),
                
                new Vector2(-pixX * 2, pixY * 2),
                new Vector2(-pixX, pixY * 2),
                new Vector2(0, pixY * 2),
                new Vector2(pixX, pixY * 2),
                new Vector2(pixX * 2, pixY * 2),
            });
            
            shader.Parameters["sampleWeights"].SetValue(new float[] {
                0.003765F,
                0.015019F,
                0.023792F,
                0.015019F,
                0.003765F,
                
                0.015019F,
                0.059912F,
                0.094907F,
                0.059912F,
                0.015019F,
                
                0.023792F,
                0.094907F,
                0.150342F,
                0.094907F,
                0.023792F,
                
                0.015019F,
                0.059912F,
                0.094907F,
                0.059912F,
                0.015019F,
                
                0.003765F,
                0.015019F,
                0.023792F,
                0.015019F,
                0.003765F,
            });
            return shader;
        }

    }
}