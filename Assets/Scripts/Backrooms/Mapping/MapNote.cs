using System;
using UnityEngine;

namespace Backrooms.Mapping
{
    [Serializable]
    public class MapNote
    {
        public string noteId;
        public string packageId;
        public string localAreaId;
        public string title;
        public string body;
        public Vector3 worldPosition;
        public string uncertaintyLevel;
        public string createdAtUtc;
    }
}
