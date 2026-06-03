using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.SceneAssembly
{
    [Serializable]
    public class SceneAssemblyPlan
    {
        public string planId;
        public string packageId;
        public string sceneName;
        public string levelId;
        public int seed;
        public List<BlockoutRoomPlan> rooms = new List<BlockoutRoomPlan>();
        public List<BlockoutConnectionPlan> connections = new List<BlockoutConnectionPlan>();
        public List<BlockoutLightPlan> lights = new List<BlockoutLightPlan>();
        public List<BlockoutTransitionPlan> transitions = new List<BlockoutTransitionPlan>();
        public List<BlockoutOpeningPlan> openings = new List<BlockoutOpeningPlan>();
    }

    [Serializable]
    public class BlockoutRoomPlan
    {
        public string roomId;
        public string roomType;
        public Vector3 position;
        public Vector3 size;
        public string materialHint;
    }

    [Serializable]
    public class BlockoutConnectionPlan
    {
        public string connectionId;
        public string fromRoomId;
        public string toRoomId;
        public Vector3 position;
        public Vector3 size;
    }

    [Serializable]
    public class BlockoutLightPlan
    {
        public string lightId;
        public Vector3 position;
        public Vector3 size;
        public float intensity;
        public string lightTypeHint;
    }

    [Serializable]
    public class BlockoutTransitionPlan
    {
        public string transitionId;
        public Vector3 position;
        public Vector3 size;
        public string targetLevelId;
        public string targetPackageId;
        public string transitionType;
    }

    [Serializable]
    public class BlockoutOpeningPlan
    {
        public string openingId;
        public string roomId;
        public Vector3 position;
        public Vector3 size;
        public string directionHint;
    }
}
