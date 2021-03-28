using System.Collections.Generic;

namespace Runner {
    public class Stats {

        public Dictionary<string, LevelStats> allLevelStats = new Dictionary<string, LevelStats>();

        public Stats() { }

        public Stats(StatData data) {
            for (int i = 0; i < data.levelArr.Length; i++) {
                allLevelStats[data.levelArr[i]] = data.levelStatArr[i];
            }
        }

        public StatData genData() {
            StatData data = new StatData();

            int count = allLevelStats.Count;

            data.levelArr = new string[count];
            data.levelStatArr = new LevelStats[count];

            int i = 0;
            foreach (var pair in allLevelStats) {
                data.levelArr[i] = pair.Key;
                data.levelStatArr[i] = pair.Value;
                i++;
            }
            
            return data;
        }

        public LevelStats getLevelStates(string levelName) {

            if (!allLevelStats.ContainsKey(levelName))
                allLevelStats[levelName] = new LevelStats();
            
            return allLevelStats[levelName];
        }
    }
}