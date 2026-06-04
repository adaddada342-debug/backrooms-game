using System;
using System.Collections.Generic;

namespace Backrooms.Mapping.Data
{
    [Serializable]
    public class MapSaveFile
    {
        public string schemaVersion = "wave10.map_save.v1";
        public List<MapLevelSaveData> levels = new List<MapLevelSaveData>();

        public MapLevelSaveData GetOrCreateLevel(string packageId, string levelId, int seed)
        {
            if (levels == null)
            {
                levels = new List<MapLevelSaveData>();
            }

            foreach (MapLevelSaveData level in levels)
            {
                if (level != null &&
                    level.packageId == packageId &&
                    level.levelId == levelId &&
                    level.seed == seed)
                {
                    return level;
                }
            }

            MapLevelSaveData created = new MapLevelSaveData
            {
                saveId = packageId + "_" + levelId + "_" + seed,
                packageId = packageId,
                levelId = levelId,
                seed = seed,
                updatedAtUtc = DateTime.UtcNow.ToString("o")
            };
            levels.Add(created);
            return created;
        }
    }
}
