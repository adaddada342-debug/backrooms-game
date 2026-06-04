using System.Collections.Generic;
using Backrooms.SceneAssembly;
using UnityEngine;

namespace Backrooms.LayoutSynthesis.Gizmos
{
    public class LayoutDebugGizmo : MonoBehaviour
    {
        public bool drawRoomLabels = true;
        public bool drawConnections = true;
        public bool drawTransitions = true;
        public bool drawLandmarks = true;
        public Color roomColor = new Color(0.2f, 0.85f, 1f, 0.9f);
        public Color connectionColor = new Color(0.2f, 1f, 0.45f, 0.9f);
        public Color transitionColor = new Color(1f, 0.75f, 0.2f, 0.9f);
        public Color landmarkColor = new Color(1f, 0.25f, 0.75f, 0.9f);
        public List<string> roomIds = new List<string>();
        public List<Vector3> roomPositions = new List<Vector3>();
        public List<string> connectionIds = new List<string>();
        public List<Vector3> connectionPositions = new List<Vector3>();
        public List<string> transitionIds = new List<string>();
        public List<Vector3> transitionPositions = new List<Vector3>();
        public List<string> landmarkIds = new List<string>();
        public List<Vector3> landmarkPositions = new List<Vector3>();

        public void Configure(SceneAssemblyPlan plan)
        {
            roomIds.Clear();
            roomPositions.Clear();
            connectionIds.Clear();
            connectionPositions.Clear();
            transitionIds.Clear();
            transitionPositions.Clear();
            landmarkIds.Clear();
            landmarkPositions.Clear();

            if (plan == null)
            {
                return;
            }

            if (plan.rooms != null)
            {
                foreach (BlockoutRoomPlan room in plan.rooms)
                {
                    if (room == null)
                    {
                        continue;
                    }

                    roomIds.Add(room.roomId);
                    roomPositions.Add(room.position + Vector3.up * 0.2f);
                }
            }

            if (plan.connections != null)
            {
                foreach (BlockoutConnectionPlan connection in plan.connections)
                {
                    if (connection == null)
                    {
                        continue;
                    }

                    connectionIds.Add(connection.connectionId);
                    connectionPositions.Add(connection.position + Vector3.up * 0.4f);
                }
            }

            if (plan.transitions != null)
            {
                foreach (BlockoutTransitionPlan transition in plan.transitions)
                {
                    if (transition == null)
                    {
                        continue;
                    }

                    transitionIds.Add(transition.transitionId);
                    transitionPositions.Add(transition.position);
                }
            }
        }

        public void AddLandmark(string landmarkId, Vector3 position)
        {
            landmarkIds.Add(landmarkId);
            landmarkPositions.Add(position);
        }

        private void OnDrawGizmos()
        {
            UnityEngine.Gizmos.color = roomColor;
            for (int i = 0; i < roomPositions.Count; i++)
            {
                UnityEngine.Gizmos.DrawWireCube(roomPositions[i], new Vector3(1.2f, 0.4f, 1.2f));
            }

            if (drawConnections)
            {
                UnityEngine.Gizmos.color = connectionColor;
                for (int i = 0; i < connectionPositions.Count; i++)
                {
                    UnityEngine.Gizmos.DrawWireSphere(connectionPositions[i], 0.45f);
                }
            }

            if (drawTransitions)
            {
                UnityEngine.Gizmos.color = transitionColor;
                for (int i = 0; i < transitionPositions.Count; i++)
                {
                    UnityEngine.Gizmos.DrawWireCube(transitionPositions[i], new Vector3(1f, 1f, 1f));
                }
            }

            if (drawLandmarks)
            {
                UnityEngine.Gizmos.color = landmarkColor;
                for (int i = 0; i < landmarkPositions.Count; i++)
                {
                    UnityEngine.Gizmos.DrawWireSphere(landmarkPositions[i], 0.6f);
                }
            }

            // TODO: Add editor-only Handles labels in a future debug visualization pass.
        }
    }
}
