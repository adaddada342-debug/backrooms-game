using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.Mapping.UI
{
    [Serializable]
    public class MapViewModel
    {
        public string packageId;
        public string levelId;
        public int seed;
        public List<MapRoomViewModel> rooms = new List<MapRoomViewModel>();
        public List<MapConnectionViewModel> connections = new List<MapConnectionViewModel>();
        public List<MapNoteViewModel> notes = new List<MapNoteViewModel>();
        public int discoveredRoomCount;
        public int totalRoomCount;
        public string currentRoomId;
        public Vector2 playerMapPosition;
        public bool hasPlayerPosition;
    }
}
