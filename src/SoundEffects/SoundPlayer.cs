using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Runner
{

    public class SoundPlayer
    {
        public static Dictionary<string, SoundEffect> effects = new Dictionary<string, SoundEffect>();
        public static SoundEffect curEffect;
        public SoundPlayer() { }

        //effectName is the filename
        public static void play(string effectName, float volume = 1.0F) {
            curEffect = getEffect(effectName);
            curEffect.Play(volume, 0.0F, 0.0F);
        }
        
        
        public static SoundEffect getEffect(string identifier) {
            
            effects.TryGetValue(identifier, out var SoundEffect);
            
            return (SoundEffect == null) ? effects["null"] : SoundEffect;
        }
        
        public static void loadEffects() {
            
            processFolder(Paths.effectPath);
        }
        //used only in loadEffects, ignore usually
        private static void processFile(string path) { // assumes a png file...
            int start = path.LastIndexOf("\\") + 1;
            int pngIndex = path.LastIndexOf(".wav");
            
            if (pngIndex != -1) {
                string filename = path.Substring(start, pngIndex - start);
                loadEffect(filename, path);
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

        private static void loadEffect(string identifier, string absolutePath) {
            
            SoundEffect effect = SoundEffect.FromFile(absolutePath);
            effects[identifier] = effect;
            
        }
    }
}
