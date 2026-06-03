using System;

namespace Backrooms.Grammar
{
    [Serializable]
    public class RoomGrammarProfile
    {
        public string grammarId;
        public string displayName;
        public string description;
        public int minimumRooms;
        public int maximumRooms;
        public int minimumConnections;
        public int maximumConnections;
        public bool allowDeadEnds;
        public bool allowLoops;
        public bool allowBranches;
        public bool requireLandmarks;
        public float corridorBias;
        public float roomBias;
        public float branchBias;
        public float deadEndBias;
        public float loopBias;
        public string[] mandatoryRoomTypes;
        public string[] optionalRoomTypes;
        public string[] forbiddenRoomTypes;
    }
}
