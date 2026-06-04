using System;
using UnityEngine;

namespace Backrooms.Mapping.UI
{
    [Serializable]
    public class MapConnectionViewModel
    {
        public string connectionId;
        public string fromRoomId;
        public string toRoomId;
        public Vector2 fromMapPosition;
        public Vector2 toMapPosition;
        public bool isOnMainRoute;
    }
}
