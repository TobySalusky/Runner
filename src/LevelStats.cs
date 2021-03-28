namespace Runner {
    public class LevelStats {

        public int timesCompleted;
        public int timesFailed;
        
        public float bestTime = float.MaxValue;

        public string bestTimeStr() {
            return (timesCompleted > 0) ? bestTime.ToString("F") : "N/A";
        }
    }
}