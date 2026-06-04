using System;

namespace Backrooms.Mapping.Interaction
{
    [Serializable]
    public class MapSelectionState
    {
        public MapSelectionType selectionType;
        public string selectedRoomId;
        public string selectedNoteId;
        public string title;
        public string body;

        public void Clear()
        {
            selectionType = MapSelectionType.None;
            selectedRoomId = string.Empty;
            selectedNoteId = string.Empty;
            title = string.Empty;
            body = string.Empty;
        }

        public void SelectRoom(string roomId, string newTitle, string newBody)
        {
            selectionType = MapSelectionType.Room;
            selectedRoomId = roomId ?? string.Empty;
            selectedNoteId = string.Empty;
            title = newTitle ?? string.Empty;
            body = newBody ?? string.Empty;
        }

        public void SelectNote(string noteId, string newTitle, string newBody)
        {
            selectionType = MapSelectionType.Note;
            selectedRoomId = string.Empty;
            selectedNoteId = noteId ?? string.Empty;
            title = newTitle ?? string.Empty;
            body = newBody ?? string.Empty;
        }
    }
}
