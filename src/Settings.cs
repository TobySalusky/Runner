
namespace Runner {
    public class Settings {

        public float masterVol { get; set; }

        public Settings() {
            
        }

        public Settings(int test) {
            masterVol = test;
            Logger.log(masterVol);
        }

        public void save() {
            DataSerializer.Serialize("Settings", this);
        }
    }
}