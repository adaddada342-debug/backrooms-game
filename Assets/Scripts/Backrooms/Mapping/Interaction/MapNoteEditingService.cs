using System;
using Backrooms.Mapping.Data;

namespace Backrooms.Mapping.Interaction
{
    public static class MapNoteEditingService
    {
        public static bool TryEditNote(MapLevelSaveData saveData, MapNoteEditRequest request)
        {
            if (saveData == null || saveData.notes == null || request == null || string.IsNullOrWhiteSpace(request.noteId))
            {
                return false;
            }

            foreach (MapNoteSaveData note in saveData.notes)
            {
                if (note == null || note.noteId != request.noteId)
                {
                    continue;
                }

                note.title = request.newTitle ?? string.Empty;
                note.body = request.newBody ?? string.Empty;
                note.uncertaintyLevel = request.newUncertaintyLevel ?? string.Empty;
                saveData.updatedAtUtc = DateTime.UtcNow.ToString("o");
                return true;
            }

            return false;
        }

        public static bool TryDeleteNote(MapLevelSaveData saveData, string noteId)
        {
            return saveData != null && saveData.RemoveNote(noteId);
        }
    }
}
