using UnityEngine;

namespace Backrooms.Debugging
{
    public class RoomIdentityLabel : MonoBehaviour
    {
        public string roomId;
        public string roomType;
        public bool showInWorld = true;

        public void Configure(string newRoomId, string newRoomType)
        {
            roomId = newRoomId;
            roomType = newRoomType;
        }

        private void OnDrawGizmos()
        {
            if (!showInWorld)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.2f, 0.35f);
        }
    }
}
