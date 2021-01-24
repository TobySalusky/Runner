namespace Runner {
    public static class FileUtil {
        
        public static string readTxtFile(string absolutePath) {
            return System.IO.File.ReadAllText(absolutePath);
        }

        public static string readTxtFile(string locationPath, string filename, string extension = ".txt") {
            return readTxtFile(locationPath + filename + extension);
        }
    }
}