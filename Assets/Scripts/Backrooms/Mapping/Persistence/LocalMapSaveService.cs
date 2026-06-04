using System.IO;
using Backrooms.Mapping.Data;
using UnityEngine;

namespace Backrooms.Mapping.Persistence
{
    public static class LocalMapSaveService
    {
        private const string FileName = "backrooms_map_save.json";

        public static MapSaveFile Load()
        {
            string path = GetSavePath();
            if (!File.Exists(path))
            {
                return new MapSaveFile();
            }

            try
            {
                string json = File.ReadAllText(path);
                MapSaveFile saveFile = JsonUtility.FromJson<MapSaveFile>(json);
                return saveFile ?? new MapSaveFile();
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning("Map save file could not be loaded. A new save file will be used. " + exception.Message);
                return new MapSaveFile();
            }
        }

        public static void Save(MapSaveFile saveFile)
        {
            if (saveFile == null)
            {
                return;
            }

            string path = GetSavePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonUtility.ToJson(saveFile, true));
        }

        public static MapLevelSaveData LoadLevel(string packageId, string levelId, int seed)
        {
            MapSaveFile saveFile = Load();
            return saveFile.GetOrCreateLevel(packageId, levelId, seed);
        }

        public static void SaveLevel(MapLevelSaveData levelData)
        {
            if (levelData == null)
            {
                return;
            }

            MapSaveFile saveFile = Load();
            MapLevelSaveData target = saveFile.GetOrCreateLevel(levelData.packageId, levelData.levelId, levelData.seed);
            target.discoveredRoomIds = levelData.discoveredRoomIds;
            target.notes = levelData.notes;
            target.lastKnownRoomId = levelData.lastKnownRoomId;
            target.lastKnownPlayerPosition = levelData.lastKnownPlayerPosition;
            target.updatedAtUtc = levelData.updatedAtUtc;
            Save(saveFile);
        }

        public static string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, FileName);
        }
    }
}
