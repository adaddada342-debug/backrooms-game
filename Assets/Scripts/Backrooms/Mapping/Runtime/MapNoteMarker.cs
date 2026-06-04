using UnityEngine;

namespace Backrooms.Mapping.Runtime
{
    public class MapNoteMarker : MonoBehaviour
    {
        public MapNote note;
        public bool visibleInWorld = true;

        public void Configure(MapNote newNote)
        {
            note = newNote;
            visibleInWorld = true;
        }
    }
}
