using System;
using System.Collections.Generic;
using Backrooms.Debugging;
using Backrooms.Mapping.Data;
using Backrooms.Mapping.Persistence;
using Backrooms.Runtime.LevelContext;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.Mapping.Discovery
{
    public class MapDiscoveryTracker : MonoBehaviour
    {
        public GeneratedLevelRuntimeContext levelContext;
        public MapDiscoverySettings settings;
        public MapLevelSaveData currentLevelSave;
        public string currentRoomId;
        public string previousRoomId;
        public List<RoomDiscoveryEvent> recentEvents = new List<RoomDiscoveryEvent>();

        public event Action<RoomDiscoveryEvent> RoomDiscovered;
        public event Action<string> CurrentRoomChanged;

        private float nextCheckTime;
        private readonly Dictionary<string, RoomIdentityLabel> labelsByRoomId = new Dictionary<string, RoomIdentityLabel>();

        private void Start()
        {
            if (levelContext == null)
            {
                levelContext = FindAnyObjectByType<GeneratedLevelRuntimeContext>();
            }

            if (settings == null)
            {
                settings = new MapDiscoverySettings();
            }

            settings.ClampValues();

            if (levelContext != null)
            {
                currentLevelSave = LocalMapSaveService.LoadLevel(levelContext.packageId, levelContext.levelId, levelContext.seed);
            }

            CacheRoomLabels();

            if (settings.discoverNearestRoomOnStart)
            {
                BlockoutRoomPlan nearest = levelContext == null ? null : levelContext.FindNearestRoom(transform.position);
                MarkRoomDiscovered(nearest, "start_nearest");
                UpdateCurrentRoom(nearest);
            }

            if (settings.revealMainRouteOnDebug)
            {
                ForceDiscoverMainRoute();
            }

            RefreshRoomLabels();
        }

        private void Update()
        {
            if (Time.time < nextCheckTime)
            {
                return;
            }

            settings.ClampValues();
            nextCheckTime = Time.time + settings.discoveryCheckInterval;
            RunDiscoveryCheck();
        }

        public bool IsRoomDiscovered(string roomId)
        {
            return currentLevelSave != null && currentLevelSave.HasDiscoveredRoom(roomId);
        }

        public void ForceDiscoverRoom(string roomId, string reason)
        {
            if (levelContext == null)
            {
                return;
            }

            MarkRoomDiscovered(levelContext.FindRoomById(roomId), reason);
        }

        public void ForceDiscoverMainRoute()
        {
            if (levelContext == null || levelContext.routeAnnotation == null || levelContext.routeAnnotation.orderedRoomIds == null)
            {
                return;
            }

            foreach (string roomId in levelContext.routeAnnotation.orderedRoomIds)
            {
                ForceDiscoverRoom(roomId, "debug_main_route");
            }
        }

        private void RunDiscoveryCheck()
        {
            if (levelContext == null || settings == null)
            {
                return;
            }

            BlockoutRoomPlan nearest = levelContext.FindNearestRoom(transform.position);
            UpdateCurrentRoom(nearest);

            if (nearest == null)
            {
                return;
            }

            float distance = Vector3.Distance(transform.position, nearest.position);
            float roomRadius = Mathf.Max(nearest.size.x, nearest.size.z) * 0.55f;
            if (distance <= settings.roomDiscoveryRadius || distance <= roomRadius)
            {
                MarkRoomDiscovered(nearest, "proximity");
            }

            if (currentLevelSave != null)
            {
                currentLevelSave.SetLastKnownPlayerPosition(transform.position);
                if (settings.saveDiscoveryImmediately)
                {
                    LocalMapSaveService.SaveLevel(currentLevelSave);
                }
            }
        }

        private void MarkRoomDiscovered(BlockoutRoomPlan room, string reason)
        {
            if (room == null)
            {
                return;
            }

            EnsureSave();
            if (currentLevelSave == null)
            {
                return;
            }

            bool firstDiscovery = !currentLevelSave.HasDiscoveredRoom(room.roomId);
            currentLevelSave.MarkRoomDiscovered(room.roomId);
            currentLevelSave.SetLastKnownRoom(room.roomId);
            currentLevelSave.SetLastKnownPlayerPosition(transform.position);

            RoomDiscoveryEvent discoveryEvent = new RoomDiscoveryEvent
            {
                roomId = room.roomId,
                roomType = room.roomType,
                worldPosition = room.position,
                discoveredAtUtc = DateTime.UtcNow.ToString("o"),
                firstDiscovery = firstDiscovery,
                discoveryReason = string.IsNullOrWhiteSpace(reason) ? "unknown" : reason
            };

            recentEvents.Add(discoveryEvent);
            if (recentEvents.Count > 24)
            {
                recentEvents.RemoveAt(0);
            }

            if (settings != null && settings.saveDiscoveryImmediately)
            {
                LocalMapSaveService.SaveLevel(currentLevelSave);
            }

            RoomDiscovered?.Invoke(discoveryEvent);
            RefreshRoomLabels();
        }

        private void UpdateCurrentRoom(BlockoutRoomPlan room)
        {
            string newRoomId = room == null ? string.Empty : room.roomId;
            if (string.Equals(currentRoomId, newRoomId, StringComparison.Ordinal))
            {
                return;
            }

            previousRoomId = currentRoomId;
            currentRoomId = newRoomId;

            EnsureSave();
            if (currentLevelSave != null)
            {
                currentLevelSave.SetLastKnownRoom(currentRoomId);
                currentLevelSave.SetLastKnownPlayerPosition(transform.position);
            }

            CurrentRoomChanged?.Invoke(currentRoomId);
            RefreshRoomLabels();
        }

        private void EnsureSave()
        {
            if (currentLevelSave != null || levelContext == null)
            {
                return;
            }

            currentLevelSave = LocalMapSaveService.LoadLevel(levelContext.packageId, levelContext.levelId, levelContext.seed);
        }

        private void CacheRoomLabels()
        {
            labelsByRoomId.Clear();
            RoomIdentityLabel[] labels = FindObjectsByType<RoomIdentityLabel>();
            foreach (RoomIdentityLabel label in labels)
            {
                if (label != null && !string.IsNullOrWhiteSpace(label.roomId) && !labelsByRoomId.ContainsKey(label.roomId))
                {
                    labelsByRoomId.Add(label.roomId, label);
                }
            }
        }

        private void RefreshRoomLabels()
        {
            foreach (KeyValuePair<string, RoomIdentityLabel> pair in labelsByRoomId)
            {
                if (pair.Value == null)
                {
                    continue;
                }

                bool discovered = IsRoomDiscovered(pair.Key);
                bool isCurrent = string.Equals(currentRoomId, pair.Key, StringComparison.Ordinal);
                pair.Value.SetDiscoveryState(discovered, isCurrent);
            }
        }
    }
}
