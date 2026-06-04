using Backrooms.Mapping.Data;

namespace Backrooms.Mapping.Runtime
{
    public static class MapNoteRuntimeUtility
    {
        public static MapNoteSaveData ToSaveData(MapNote note, string roomId)
        {
            if (note == null)
            {
                return null;
            }

            return new MapNoteSaveData
            {
                noteId = note.noteId,
                packageId = note.packageId,
                levelId = note.localAreaId,
                roomId = roomId,
                title = note.title,
                body = note.body,
                worldPosition = note.worldPosition,
                uncertaintyLevel = note.uncertaintyLevel,
                createdAtUtc = note.createdAtUtc
            };
        }

        public static MapNote FromSaveData(MapNoteSaveData saveData)
        {
            if (saveData == null)
            {
                return null;
            }

            return new MapNote
            {
                noteId = saveData.noteId,
                packageId = saveData.packageId,
                localAreaId = saveData.levelId,
                title = saveData.title,
                body = saveData.body,
                worldPosition = saveData.worldPosition,
                uncertaintyLevel = saveData.uncertaintyLevel,
                createdAtUtc = saveData.createdAtUtc
            };
        }
    }
}
