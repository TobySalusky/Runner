using System.IO;

namespace Runner {
    public static class Paths {

        public static string solutionPath, assetPath, texturePath, effectPath;

        static Paths() {
            string path = Path.GetFullPath("hi");
            solutionPath = path.Substring(0, path.IndexOf("bin\\Debug"));
            assetPath = solutionPath + "Assets\\";
            texturePath = assetPath + "Textures\\";
            effectPath = assetPath + "SoundEffects\\";
        }

    }
}