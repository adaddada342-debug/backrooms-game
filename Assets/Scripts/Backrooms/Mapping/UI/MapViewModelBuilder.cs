using System.Collections.Generic;
using Backrooms.LayoutSynthesis.Landmarks;
using Backrooms.Mapping.Data;
using Backrooms.Mapping.Discovery;
using Backrooms.Runtime.LevelContext;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.Mapping.UI
{
    public static class MapViewModelBuilder
    {
        public static MapViewModel Build(
            GeneratedLevelRuntimeContext context,
            MapLevelSaveData saveData,
            Rect mapRect,
            float padding)
        {
            return Build(context, saveData, mapRect, padding, new MapDiscoverySettings());
        }

        public static MapViewModel Build(
            GeneratedLevelRuntimeContext context,
            MapLevelSaveData saveData,
            Rect mapRect,
            float padding,
            MapDiscoverySettings settings)
        {
            MapViewModel model = new MapViewModel();
            if (context == null || context.plan == null)
            {
                return model;
            }

            if (settings == null)
            {
                settings = new MapDiscoverySettings();
            }

            settings.ClampValues();
            model.packageId = context.packageId;
            model.levelId = context.levelId;
            model.seed = context.seed;
            model.currentRoomId = saveData == null ? string.Empty : saveData.lastKnownRoomId;
            Bounds bounds = WorldToMapProjector.CalculateBounds(context.plan);
            Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();
            Dictionary<string, bool> visibleByRoomId = new Dictionary<string, bool>();

            if (context.plan.rooms != null)
            {
                foreach (BlockoutRoomPlan room in context.plan.rooms)
                {
                    if (room == null)
                    {
                        continue;
                    }

                    Vector2 mapPosition = WorldToMapProjector.Project(room.position, bounds, mapRect, padding);
                    positions[room.roomId] = mapPosition;
                    bool discovered = saveData != null && saveData.HasDiscoveredRoom(room.roomId);
                    bool isCurrentRoom = saveData != null && string.Equals(saveData.lastKnownRoomId, room.roomId, System.StringComparison.Ordinal);
                    bool visibleOnMap = settings.showUndiscoveredRooms || discovered || isCurrentRoom;
                    visibleByRoomId[room.roomId] = visibleOnMap;
                    if (discovered)
                    {
                        model.discoveredRoomCount++;
                    }

                    model.rooms.Add(new MapRoomViewModel
                    {
                        roomId = room.roomId,
                        roomType = room.roomType,
                        mapPosition = mapPosition,
                        isOnMainRoute = context.routeAnnotation != null && context.routeAnnotation.ContainsRoom(room.roomId),
                        discovered = discovered,
                        hasLandmark = HasLandmark(context.landmarkPlacementPlan, room.roomId),
                        isCurrentRoom = isCurrentRoom,
                        visibleOnMap = visibleOnMap,
                        noteCount = CountNotes(saveData, room.roomId)
                    });
                }
            }

            model.totalRoomCount = model.rooms.Count;

            if (context.plan.connections != null)
            {
                foreach (BlockoutConnectionPlan connection in context.plan.connections)
                {
                    if (connection == null || !positions.ContainsKey(connection.fromRoomId) || !positions.ContainsKey(connection.toRoomId))
                    {
                        continue;
                    }

                    if (!IsRoomVisible(visibleByRoomId, connection.fromRoomId) || !IsRoomVisible(visibleByRoomId, connection.toRoomId))
                    {
                        continue;
                    }

                    model.connections.Add(new MapConnectionViewModel
                    {
                        connectionId = connection.connectionId,
                        fromRoomId = connection.fromRoomId,
                        toRoomId = connection.toRoomId,
                        fromMapPosition = positions[connection.fromRoomId],
                        toMapPosition = positions[connection.toRoomId],
                        isOnMainRoute = context.routeAnnotation != null &&
                                        context.routeAnnotation.ContainsRoom(connection.fromRoomId) &&
                                        context.routeAnnotation.ContainsRoom(connection.toRoomId)
                    });
                }
            }

            if (saveData != null && saveData.notes != null)
            {
                foreach (MapNoteSaveData note in saveData.notes)
                {
                    if (note == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(note.roomId) && !IsRoomVisible(visibleByRoomId, note.roomId))
                    {
                        continue;
                    }

                    model.notes.Add(new MapNoteViewModel
                    {
                        noteId = note.noteId,
                        roomId = note.roomId,
                        title = note.title,
                        mapPosition = WorldToMapProjector.Project(note.worldPosition, bounds, mapRect, padding),
                        uncertaintyLevel = note.uncertaintyLevel
                    });
                }
            }

            if (saveData != null)
            {
                model.playerMapPosition = WorldToMapProjector.Project(saveData.lastKnownPlayerPosition, bounds, mapRect, padding);
                model.hasPlayerPosition = saveData.lastKnownPlayerPosition != Vector3.zero || context.HasValidPlan();
            }

            return model;
        }

        private static bool HasLandmark(LandmarkPlacementPlan plan, string roomId)
        {
            return plan != null && plan.FindByRoomId(roomId).Count > 0;
        }

        private static int CountNotes(MapLevelSaveData saveData, string roomId)
        {
            if (saveData == null || saveData.notes == null)
            {
                return 0;
            }

            int count = 0;
            foreach (MapNoteSaveData note in saveData.notes)
            {
                if (note != null && note.roomId == roomId)
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsRoomVisible(Dictionary<string, bool> visibleByRoomId, string roomId)
        {
            return visibleByRoomId != null &&
                   visibleByRoomId.TryGetValue(roomId, out bool visible) &&
                   visible;
        }
    }
}
