using System;
using UnityEngine;

namespace Backrooms.Mapping.Data
{
    [Serializable]
    public class MapNoteSaveData
    {
        public string noteId;
        public string packageId;
        public string levelId;
        public string roomId;
        public string title;
        public string body;
        public Vector3 worldPosition;
        public string uncertaintyLevel;
        public string createdAtUtc;
    }
}
