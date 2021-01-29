using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Runner {
    public class Textures {

        private static Dictionary<string, Texture2D> textures;
        public static Texture2D nullTexture;

        public static void loadTextures() {

            textures = new Dictionary<string, Texture2D>();

            textures["pixel"] = genRect(Color.White);
            textures["UIButton"] = genRect(new Color(Color.Black, 0.5F));
            textures["invis"] = genRect(new Color(1F,1F,1F,0F));

            processFolder(Paths.texturePath);

            nullTexture = textures["null"];
        }

        public static Dictionary<string, Texture2D> debugTexturesGrab() {
            return textures;
        }

        private static Texture2D genRect(Color rectColor) {
            Texture2D rect = new Texture2D(Runner.getGraphicsDeviceManager(), 1, 1);
            rect.SetData(new[] {rectColor});
            return rect;
        }

        private static void processFile(string path) { // assumes a png file...
            int start = path.LastIndexOf("\\") + 1;
            int pngIndex = path.LastIndexOf(".png");
            
            if (pngIndex != -1) {
                string filename = path.Substring(start, pngIndex - start);
                loadTexture(filename, path);
            }
        }

        private static void processFolder(string dirPath) {
            string [] files = Directory.GetFiles(dirPath);
            foreach (string file in files)
                processFile(file);

            // recursive calls
            string [] subDirs = Directory.GetDirectories(dirPath);
            foreach(string subDir in subDirs)
                processFolder(subDir);
        }

        private static void loadTexture(string identifier, string path) {
            Texture2D texture = Texture2D.FromFile(Runner.getGraphicsDeviceManager(), path);
            textures[identifier] = texture;
        }

        public static bool has(string identifier) {
            return textures.ContainsKey(identifier);
        }

        public static Texture2D get(string identifier) {
            
            textures.TryGetValue(identifier, out var texture);

            if (texture == null) {
                return textures["null"];
            }
            return texture;
        }

        public static void exportTexture(Texture2D texture, string location, string identifier) {
            Stream stream = File.Create(location + identifier + ".png");
            texture.SaveAsPng( stream, texture.Width, texture.Height );
            stream.Dispose();
        }
        
        public static void exportTexture(Texture2D texture, string absolutePath) {
            Stream stream = File.Create(absolutePath);
            texture.SaveAsPng( stream, texture.Width, texture.Height );
            stream.Dispose();
        }
    }
}