using System;
using UnityEngine;

namespace Backrooms.Mapping.UI
{
    [Serializable]
    public class MapRoomViewModel
    {
        public string roomId;
        public string roomType;
        public Vector2 mapPosition;
        public bool isOnMainRoute;
        public bool discovered;
        public bool hasLandmark;
        public bool isCurrentRoom;
        public bool visibleOnMap;
        public int noteCount;
    }
}
