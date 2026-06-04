using System;
using Backrooms.Mapping.Data;
using Backrooms.Mapping.Persistence;
using Backrooms.Runtime.LevelContext;
using Backrooms.SceneAssembly;
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
        public GeneratedLevelRuntimeContext levelContext;
        public bool persistNotes = true;
        public MapLevelSaveData currentLevelSave;

        private void Start()
        {
            if (levelContext == null)
            {
                levelContext = UnityEngine.Object.FindAnyObjectByType<GeneratedLevelRuntimeContext>();
            }

            if (levelContext != null)
            {
                if (string.IsNullOrWhiteSpace(packageId))
                {
                    packageId = levelContext.packageId;
                }

                if (string.IsNullOrWhiteSpace(localAreaId))
                {
                    localAreaId = levelContext.levelId;
                }
            }

            if (persistNotes)
            {
                int seed = levelContext == null ? 0 : levelContext.seed;
                currentLevelSave = LocalMapSaveService.LoadLevel(packageId, localAreaId, seed);
                RestoreSavedMarkers();
            }
        }

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

            BlockoutRoomPlan nearestRoom = levelContext == null ? null : levelContext.FindNearestRoom(position);
            string roomId = nearestRoom == null ? string.Empty : nearestRoom.roomId;
            if (persistNotes)
            {
                if (currentLevelSave == null)
                {
                    currentLevelSave = LocalMapSaveService.LoadLevel(packageId, localAreaId, levelContext == null ? 0 : levelContext.seed);
                }

                if (nearestRoom != null)
                {
                    currentLevelSave.MarkRoomDiscovered(nearestRoom.roomId);
                }

                currentLevelSave.AddOrUpdateNote(MapNoteRuntimeUtility.ToSaveData(note, roomId));
                LocalMapSaveService.SaveLevel(currentLevelSave);
            }

            CreateMarker(note);
        }

        private void RestoreSavedMarkers()
        {
            if (currentLevelSave == null || currentLevelSave.notes == null)
            {
                return;
            }

            foreach (MapNoteSaveData saveData in currentLevelSave.notes)
            {
                MapNote note = MapNoteRuntimeUtility.FromSaveData(saveData);
                if (note != null)
                {
                    CreateMarker(note);
                }
            }
        }

        private static void CreateMarker(MapNote note)
        {
            if (note == null)
            {
                return;
            }

            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "MapNote_" + note.noteId;
            marker.transform.position = note.worldPosition;
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
