using System;

namespace Backrooms.Mapping.Interaction
{
    [Serializable]
    public class MapNoteEditRequest
    {
        public string noteId;
        public string newTitle;
        public string newBody;
        public string newUncertaintyLevel;
    }
}
