using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public static class Shaders {

        public static float[] gaussianWeights;

        public static void setGaussianOffsets(float mult) {
            Runner.gaussianBlurShader.Parameters["sampleOffsets"].SetValue(gaussianOffsets(mult));
        }

        public static Vector2[] gaussianOffsets(float mult) {
            Vector2 vec = new Vector2(1920, 1080);
            float pixX = 1 / vec.X * mult;
            float pixY = 1 / vec.Y * mult;
            return gaussianOffsets(pixX, pixY);
        }

        public static Vector2[] gaussianOffsets(float pixX, float pixY) {
            Vector2[] samplePositions = new Vector2[gaussianWeights.Length];
            
            // for single-pass gaussian blur (2D)
            for (int i = 0; i < samplePositions.Length; i++) {
                int size = (int) Math.Sqrt(samplePositions.Length);
                int sub = size / 2;
                samplePositions[i] = new Vector2(pixX * (i % size - sub), pixY * (i / size - sub));
            }

            return samplePositions;
        }

        public static Effect gaussianBlur(ContentManager Content) { 
            Effect shader = Content.Load<Effect>("Gaussian");

            Vector2 vec = new Vector2(1920, 1080);
            float mult = 2F;
            float pixX = 1 / vec.X * mult;
            float pixY = 1 / vec.Y * mult;

            /*float[] weights = new float[] {
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
            };*/

            double[] doubleWeights = { 
                
                0.005084,	0.009377,	0.013539,	0.015302,	0.013539,	0.009377,	0.005084,
                0.009377,	0.017296,	0.024972,	0.028224,	0.024972,	0.017296,	0.009377,
                0.013539,	0.024972,	0.036054,	0.040749,	0.036054,	0.024972,	0.013539,
                0.015302,	0.028224,	0.040749,	0.046056,	0.040749,	0.028224,	0.015302,
                0.013539,	0.024972,	0.036054,	0.040749,	0.036054,	0.024972,	0.013539,
                0.009377,	0.017296,	0.024972,	0.028224,	0.024972,	0.017296,	0.009377,
                0.005084,	0.009377,	0.013539,	0.015302,	0.013539,	0.009377,	0.005084,
            };
            
            float[] weights = new float[doubleWeights.Length];
            for (int i = 0; i < weights.Length; i++) {
                weights[i] = (float) doubleWeights[i];
            }

            gaussianWeights = weights;

            Vector2[] samplePositions = gaussianOffsets(pixX, pixY);

            shader.Parameters["sampleWeights"].SetValue(weights);
            shader.Parameters["sampleOffsets"].SetValue(samplePositions);
            
            return shader;
        }

    }
}