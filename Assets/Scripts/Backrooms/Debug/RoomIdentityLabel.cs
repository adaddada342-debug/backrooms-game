using UnityEngine;

namespace Backrooms.Debugging
{
    public class RoomIdentityLabel : MonoBehaviour
    {
        public string roomId;
        public string roomType;
        public bool showInWorld = true;
        public bool discovered;
        public bool currentRoom;

        public void Configure(string newRoomId, string newRoomType)
        {
            roomId = newRoomId;
            roomType = newRoomType;
        }

        public void SetDiscoveryState(bool isDiscovered, bool isCurrentRoom)
        {
            discovered = isDiscovered;
            currentRoom = isCurrentRoom;
        }

        private void OnDrawGizmos()
        {
            if (!showInWorld)
            {
                return;
            }

            if (currentRoom)
            {
                Gizmos.color = Color.green;
            }
            else if (discovered)
            {
                Gizmos.color = Color.cyan;
            }
            else
            {
                Gizmos.color = Color.gray;
            }

            Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.2f, 0.35f);
        }
    }
}
