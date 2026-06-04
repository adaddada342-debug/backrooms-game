using System;
using UnityEngine;

namespace Backrooms.Mapping.UI
{
    [Serializable]
    public class MapNoteViewModel
    {
        public string noteId;
        public string roomId;
        public string title;
        public Vector2 mapPosition;
        public string uncertaintyLevel;
    }
}
