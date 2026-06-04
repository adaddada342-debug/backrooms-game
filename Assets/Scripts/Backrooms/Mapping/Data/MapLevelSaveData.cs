using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backrooms.Mapping.Data
{
    [Serializable]
    public class MapLevelSaveData
    {
        public string saveId;
        public string packageId;
        public string levelId;
        public int seed;
        public List<string> discoveredRoomIds = new List<string>();
        public List<MapNoteSaveData> notes = new List<MapNoteSaveData>();
        public string lastKnownRoomId;
        public Vector3 lastKnownPlayerPosition;
        public string updatedAtUtc;

        public bool HasDiscoveredRoom(string roomId)
        {
            if (discoveredRoomIds == null || string.IsNullOrWhiteSpace(roomId))
            {
                return false;
            }

            foreach (string discovered in discoveredRoomIds)
            {
                if (string.Equals(discovered, roomId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public void MarkRoomDiscovered(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return;
            }

            if (discoveredRoomIds == null)
            {
                discoveredRoomIds = new List<string>();
            }

            if (!HasDiscoveredRoom(roomId))
            {
                discoveredRoomIds.Add(roomId);
            }
        }

        public void AddOrUpdateNote(MapNoteSaveData note)
        {
            if (note == null || string.IsNullOrWhiteSpace(note.noteId))
            {
                return;
            }

            if (notes == null)
            {
                notes = new List<MapNoteSaveData>();
            }

            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i] != null && notes[i].noteId == note.noteId)
                {
                    notes[i] = note;
                    updatedAtUtc = DateTime.UtcNow.ToString("o");
                    return;
                }
            }

            notes.Add(note);
            updatedAtUtc = DateTime.UtcNow.ToString("o");
        }

        public bool RemoveNote(string noteId)
        {
            if (notes == null || string.IsNullOrWhiteSpace(noteId))
            {
                return false;
            }

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                if (notes[i] != null && notes[i].noteId == noteId)
                {
                    notes.RemoveAt(i);
                    updatedAtUtc = DateTime.UtcNow.ToString("o");
                    return true;
                }
            }

            return false;
        }

        public void SetLastKnownRoom(string roomId)
        {
            lastKnownRoomId = roomId ?? string.Empty;
            updatedAtUtc = DateTime.UtcNow.ToString("o");
        }

        public void SetLastKnownPlayerPosition(Vector3 position)
        {
            lastKnownPlayerPosition = position;
            updatedAtUtc = DateTime.UtcNow.ToString("o");
        }
    }
}
