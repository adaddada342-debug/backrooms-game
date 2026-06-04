using System;
using UnityEngine;

namespace Backrooms.Mapping.Runtime
{
    public class MapNotePlacementTester : MonoBehaviour
    {
        public Camera playerCamera;
        public float placementDistance = 2.5f;
        public KeyCode placeNoteKey = KeyCode.N;
        public string packageId;
        public string localAreaId;

        private void Update()
        {
            if (!Input.GetKeyDown(placeNoteKey))
            {
                return;
            }

            PlaceNote();
        }

        private void PlaceNote()
        {
            Camera cameraToUse = playerCamera != null ? playerCamera : Camera.main;
            Vector3 position = transform.position + transform.forward * placementDistance;
            if (cameraToUse != null)
            {
                Ray ray = new Ray(cameraToUse.transform.position, cameraToUse.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, placementDistance * 4f))
                {
                    position = hit.point;
                }
                else
                {
                    position = cameraToUse.transform.position + cameraToUse.transform.forward * placementDistance;
                }
            }

            string ticks = DateTime.UtcNow.Ticks.ToString();
            MapNote note = new MapNote
            {
                noteId = "note_" + ticks,
                packageId = packageId,
                localAreaId = localAreaId,
                title = "Player Note",
                body = "Temporary Wave 9 mapping note.",
                worldPosition = position,
                uncertaintyLevel = "unknown",
                createdAtUtc = DateTime.UtcNow.ToString("o")
            };

            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "MapNote_" + ticks;
            marker.transform.position = position;
            marker.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            Collider collider = marker.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            MapNoteMarker markerComponent = marker.AddComponent<MapNoteMarker>();
            markerComponent.Configure(note);
        }
    }
}
