using Backrooms.Mapping.Data;
using Backrooms.Mapping.Persistence;
using Backrooms.Runtime.LevelContext;
using UnityEngine;
using UnityEngine.UI;

namespace Backrooms.Mapping.UI
{
    public class PrototypeMapCanvasController : MonoBehaviour
    {
        public KeyCode toggleKey = KeyCode.M;
        public bool startHidden = true;
        public GeneratedLevelRuntimeContext levelContext;
        public RectTransform mapRoot;
        public RectTransform roomLayer;
        public RectTransform connectionLayer;
        public RectTransform noteLayer;
        public Text headerText;
        public Text footerText;

        private MapLevelSaveData currentSave;
        private bool visible;
        private int lastNoteCount = -1;

        private void Start()
        {
            if (levelContext == null)
            {
                levelContext = Object.FindAnyObjectByType<GeneratedLevelRuntimeContext>();
            }

            if (levelContext != null)
            {
                currentSave = LocalMapSaveService.LoadLevel(levelContext.packageId, levelContext.levelId, levelContext.seed);
            }

            BuildGeneratedUi();
            RefreshMap();
            SetVisible(!startHidden);
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                SetVisible(!visible);
                if (visible)
                {
                    RefreshMap();
                }
            }
        }

        private void SetVisible(bool newVisible)
        {
            visible = newVisible;
            if (mapRoot != null)
            {
                mapRoot.gameObject.SetActive(visible);
            }
        }

        public void RefreshMap()
        {
            if (levelContext == null || mapRoot == null)
            {
                return;
            }

            currentSave = LocalMapSaveService.LoadLevel(levelContext.packageId, levelContext.levelId, levelContext.seed);
            int noteCount = currentSave == null || currentSave.notes == null ? 0 : currentSave.notes.Count;
            if (noteCount == lastNoteCount && visible)
            {
                // Rebuilds are intentionally coarse in this prototype.
            }

            lastNoteCount = noteCount;
            ClearLayer(roomLayer);
            ClearLayer(connectionLayer);
            ClearLayer(noteLayer);

            Rect mapRect = new Rect(-280f, -180f, 560f, 360f);
            MapViewModel model = MapViewModelBuilder.Build(levelContext, currentSave, mapRect, 20f);
            foreach (MapConnectionViewModel connection in model.connections)
            {
                UiLineUtility.CreateLine(connectionLayer, connection.fromMapPosition, connection.toMapPosition, connection.isOnMainRoute ? 4f : 2f, connection.isOnMainRoute ? Color.yellow : Color.gray, "Connection_" + connection.connectionId);
            }

            foreach (MapRoomViewModel room in model.rooms)
            {
                CreateMarker(roomLayer, room.mapPosition, room.isOnMainRoute ? Color.yellow : (room.discovered ? Color.cyan : Color.gray), room.hasLandmark ? 15f : 10f, "Room_" + room.roomId);
            }

            foreach (MapNoteViewModel note in model.notes)
            {
                CreateMarker(noteLayer, note.mapPosition, Color.magenta, 8f, "Note_" + note.noteId);
            }

            if (headerText != null)
            {
                headerText.text = model.packageId + " / " + model.levelId + " / seed " + model.seed;
            }

            if (footerText != null)
            {
                footerText.text = "Discovered " + model.discoveredRoomCount + "/" + model.totalRoomCount + " rooms | Notes " + model.notes.Count;
            }
        }

        private void BuildGeneratedUi()
        {
            if (mapRoot != null)
            {
                return;
            }

            GameObject canvasObject = new GameObject("Prototype_Map_Canvas");
            canvasObject.transform.SetParent(transform, false);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject panel = CreateUiObject(canvasObject.transform, "Map_Panel");
            mapRoot = panel.GetComponent<RectTransform>();
            mapRoot.anchorMin = new Vector2(0.5f, 0.5f);
            mapRoot.anchorMax = new Vector2(0.5f, 0.5f);
            mapRoot.sizeDelta = new Vector2(640f, 440f);
            mapRoot.anchoredPosition = Vector2.zero;
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.72f);

            headerText = CreateText(mapRoot, "Map_Header", new Vector2(0f, 190f), 18);
            footerText = CreateText(mapRoot, "Map_Footer", new Vector2(0f, -195f), 14);
            connectionLayer = CreateLayer(mapRoot, "Connection_Layer");
            roomLayer = CreateLayer(mapRoot, "Room_Layer");
            noteLayer = CreateLayer(mapRoot, "Note_Layer");
        }

        private static RectTransform CreateLayer(Transform parent, string name)
        {
            GameObject layer = CreateUiObject(parent, name);
            RectTransform rect = layer.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600f, 400f);
            rect.anchoredPosition = Vector2.zero;
            return rect;
        }

        private static Text CreateText(Transform parent, string name, Vector2 position, int fontSize)
        {
            GameObject textObject = CreateUiObject(parent, name);
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(560f, 32f);
            rect.anchoredPosition = position;
            return text;
        }

        private static GameObject CreateUiObject(Transform parent, string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();
            return obj;
        }

        private static void CreateMarker(RectTransform parent, Vector2 position, Color color, float size, string name)
        {
            GameObject marker = CreateUiObject(parent, name);
            RectTransform rect = marker.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(size, size);
            Image image = marker.AddComponent<Image>();
            image.color = color;
        }

        private static void ClearLayer(RectTransform layer)
        {
            if (layer == null)
            {
                return;
            }

            for (int i = layer.childCount - 1; i >= 0; i--)
            {
                Destroy(layer.GetChild(i).gameObject);
            }
        }
    }
}
