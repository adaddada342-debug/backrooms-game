using Backrooms.Mapping.Data;
using Backrooms.Mapping.Discovery;
using Backrooms.Mapping.Interaction;
using Backrooms.Mapping.Persistence;
using Backrooms.Mapping.Runtime;
using Backrooms.Mapping.UI.Widgets;
using Backrooms.Runtime.LevelContext;
using Backrooms.SceneAssembly;
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
        public MapDiscoveryTracker discoveryTracker;
        public MapDiscoverySettings discoverySettings;
        public MapSelectionState selectionState;
        public RectTransform detailPanel;
        public Text detailTitleText;
        public Text detailBodyText;
        public InputField noteTitleInput;
        public InputField noteBodyInput;
        public Button saveNoteButton;
        public Button deleteNoteButton;
        public bool showPlayerMarker = true;

        private RectTransform playerLayer;
        private MapLevelSaveData currentSave;
        private bool visible;
        private MapNotePlacementTester notePlacementTester;

        private void Start()
        {
            if (levelContext == null)
            {
                levelContext = FindAnyObjectByType<GeneratedLevelRuntimeContext>();
            }

            if (discoveryTracker == null)
            {
                discoveryTracker = FindAnyObjectByType<MapDiscoveryTracker>();
            }

            if (discoverySettings == null)
            {
                discoverySettings = new MapDiscoverySettings();
            }

            discoverySettings.ClampValues();
            if (selectionState == null)
            {
                selectionState = new MapSelectionState();
            }

            if (levelContext != null)
            {
                currentSave = LocalMapSaveService.LoadLevel(levelContext.packageId, levelContext.levelId, levelContext.seed);
            }

            notePlacementTester = FindAnyObjectByType<MapNotePlacementTester>();
            SubscribeRuntimeEvents();
            BuildGeneratedUi();
            RefreshMap();
            SetVisible(!startHidden);
        }

        private void OnDestroy()
        {
            UnsubscribeRuntimeEvents();
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

        public void HandleMapTargetClicked(MapSelectionType type, string targetId)
        {
            if (type == MapSelectionType.Room)
            {
                SelectRoom(targetId);
            }
            else if (type == MapSelectionType.Note)
            {
                SelectNote(targetId);
            }
            else
            {
                selectionState.Clear();
                RefreshDetailPanel();
            }
        }

        public void HandleMapTargetHovered(MapSelectionType type, string targetId)
        {
            if (type == MapSelectionType.Room)
            {
                SelectRoom(targetId);
            }
        }

        public void SelectRoom(string roomId)
        {
            if (selectionState == null)
            {
                selectionState = new MapSelectionState();
            }

            BlockoutRoomPlan room = levelContext == null ? null : levelContext.FindRoomById(roomId);
            bool discovered = currentSave != null && currentSave.HasDiscoveredRoom(roomId);
            int noteCount = CountNotesForRoom(roomId);
            string title = string.IsNullOrWhiteSpace(roomId) ? "Room" : roomId;
            string body = "Type: " + (room == null ? "unknown" : room.roomType) +
                          "\nDiscovered: " + discovered +
                          "\nNotes: " + noteCount;
            selectionState.SelectRoom(roomId, title, body);
            RefreshDetailPanel();
        }

        public void SelectNote(string noteId)
        {
            if (selectionState == null)
            {
                selectionState = new MapSelectionState();
            }

            MapNoteSaveData note = FindNote(noteId);
            if (note == null)
            {
                selectionState.Clear();
            }
            else
            {
                selectionState.SelectNote(note.noteId, note.title, note.body);
            }

            RefreshDetailPanel();
        }

        public void SaveSelectedNoteEdits()
        {
            if (selectionState == null || selectionState.selectionType != MapSelectionType.Note)
            {
                return;
            }

            MapNoteEditRequest request = new MapNoteEditRequest
            {
                noteId = selectionState.selectedNoteId,
                newTitle = noteTitleInput == null ? string.Empty : noteTitleInput.text,
                newBody = noteBodyInput == null ? string.Empty : noteBodyInput.text,
                newUncertaintyLevel = "unknown"
            };

            if (MapNoteEditingService.TryEditNote(currentSave, request))
            {
                LocalMapSaveService.SaveLevel(currentSave);
                SelectNote(request.noteId);
                RefreshMap();
            }
        }

        public void DeleteSelectedNote()
        {
            if (selectionState == null || selectionState.selectionType != MapSelectionType.Note)
            {
                return;
            }

            if (MapNoteEditingService.TryDeleteNote(currentSave, selectionState.selectedNoteId))
            {
                LocalMapSaveService.SaveLevel(currentSave);
                selectionState.Clear();
                RefreshMap();
                RefreshDetailPanel();
            }
        }

        public void RefreshMap()
        {
            if (levelContext == null || mapRoot == null)
            {
                return;
            }

            currentSave = LocalMapSaveService.LoadLevel(levelContext.packageId, levelContext.levelId, levelContext.seed);
            ClearLayer(roomLayer);
            ClearLayer(connectionLayer);
            ClearLayer(noteLayer);
            ClearLayer(playerLayer);

            Rect mapRect = new Rect(-280f, -180f, 560f, 360f);
            MapViewModel model = MapViewModelBuilder.Build(levelContext, currentSave, mapRect, 20f, discoverySettings);

            foreach (MapConnectionViewModel connection in model.connections)
            {
                UiLineUtility.CreateLine(
                    connectionLayer,
                    connection.fromMapPosition,
                    connection.toMapPosition,
                    connection.isOnMainRoute ? 4f : 2f,
                    connection.isOnMainRoute ? Color.yellow : Color.gray,
                    "Connection_" + connection.connectionId);
            }

            foreach (MapRoomViewModel room in model.rooms)
            {
                if (!room.visibleOnMap)
                {
                    continue;
                }

                Color color = GetRoomColor(room);
                float size = room.isCurrentRoom ? 18f : (room.hasLandmark ? 15f : 11f);
                GameObject marker = CreateMarker(roomLayer, room.mapPosition, color, size, "Room_" + room.roomId);
                MapUiClickTarget clickTarget = marker.AddComponent<MapUiClickTarget>();
                clickTarget.Configure(this, MapSelectionType.Room, room.roomId);
            }

            foreach (MapNoteViewModel note in model.notes)
            {
                GameObject marker = CreateMarker(noteLayer, note.mapPosition, Color.magenta, 9f, "Note_" + note.noteId);
                MapUiClickTarget clickTarget = marker.AddComponent<MapUiClickTarget>();
                clickTarget.Configure(this, MapSelectionType.Note, note.noteId);
            }

            if (showPlayerMarker && model.hasPlayerPosition)
            {
                CreateMarker(playerLayer, model.playerMapPosition, Color.white, 8f, "Player_Map_Position");
            }

            if (headerText != null)
            {
                headerText.text = model.packageId + " / " + model.levelId + " / seed " + model.seed;
            }

            if (footerText != null)
            {
                string currentRoom = string.IsNullOrWhiteSpace(model.currentRoomId) ? "unknown" : model.currentRoomId;
                footerText.text = "Current " + currentRoom + " | Discovered " + model.discoveredRoomCount + "/" + model.totalRoomCount + " | Notes " + model.notes.Count;
            }

            RefreshDetailPanel();
        }

        public void RefreshDetailPanel()
        {
            if (detailPanel == null)
            {
                return;
            }

            if (selectionState == null)
            {
                selectionState = new MapSelectionState();
            }

            bool noteSelected = selectionState.selectionType == MapSelectionType.Note;
            if (detailTitleText != null)
            {
                detailTitleText.text = string.IsNullOrWhiteSpace(selectionState.title) ? "Map Details" : selectionState.title;
            }

            if (detailBodyText != null)
            {
                if (selectionState.selectionType == MapSelectionType.None)
                {
                    detailBodyText.text = "Select a room or note.";
                }
                else
                {
                    detailBodyText.text = selectionState.body ?? string.Empty;
                }
            }

            SetActive(noteTitleInput, noteSelected);
            SetActive(noteBodyInput, noteSelected);
            SetActive(saveNoteButton, noteSelected);
            SetActive(deleteNoteButton, noteSelected);

            if (noteSelected)
            {
                MapNoteSaveData note = FindNote(selectionState.selectedNoteId);
                if (noteTitleInput != null)
                {
                    noteTitleInput.text = note == null ? string.Empty : note.title;
                }

                if (noteBodyInput != null)
                {
                    noteBodyInput.text = note == null ? string.Empty : note.body;
                }
            }
        }

        private void SubscribeRuntimeEvents()
        {
            if (discoveryTracker != null)
            {
                discoveryTracker.RoomDiscovered += HandleRoomDiscovered;
                discoveryTracker.CurrentRoomChanged += HandleCurrentRoomChanged;
            }

            if (notePlacementTester != null)
            {
                notePlacementTester.NotePlaced += HandleNotePlaced;
            }
        }

        private void UnsubscribeRuntimeEvents()
        {
            if (discoveryTracker != null)
            {
                discoveryTracker.RoomDiscovered -= HandleRoomDiscovered;
                discoveryTracker.CurrentRoomChanged -= HandleCurrentRoomChanged;
            }

            if (notePlacementTester != null)
            {
                notePlacementTester.NotePlaced -= HandleNotePlaced;
            }
        }

        private void HandleRoomDiscovered(RoomDiscoveryEvent discoveryEvent)
        {
            if (visible)
            {
                RefreshMap();
            }
        }

        private void HandleCurrentRoomChanged(string roomId)
        {
            if (visible)
            {
                RefreshMap();
            }
        }

        private void HandleNotePlaced(MapNoteSaveData note)
        {
            if (visible)
            {
                RefreshMap();
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
            mapRoot.sizeDelta = new Vector2(860f, 520f);
            mapRoot.anchoredPosition = Vector2.zero;
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.78f);

            headerText = CreateText(mapRoot, "Map_Header", new Vector2(-110f, 230f), new Vector2(560f, 32f), 18, TextAnchor.MiddleCenter);
            footerText = CreateText(mapRoot, "Map_Footer", new Vector2(-110f, -230f), new Vector2(560f, 32f), 14, TextAnchor.MiddleCenter);
            connectionLayer = CreateLayer(mapRoot, "Connection_Layer", new Vector2(-110f, 0f));
            roomLayer = CreateLayer(mapRoot, "Room_Layer", new Vector2(-110f, 0f));
            noteLayer = CreateLayer(mapRoot, "Note_Layer", new Vector2(-110f, 0f));
            playerLayer = CreateLayer(mapRoot, "Player_Layer", new Vector2(-110f, 0f));
            BuildDetailPanel();
        }

        private void BuildDetailPanel()
        {
            GameObject panel = CreateUiObject(mapRoot, "Detail_Panel");
            detailPanel = panel.GetComponent<RectTransform>();
            detailPanel.anchorMin = new Vector2(0.5f, 0.5f);
            detailPanel.anchorMax = new Vector2(0.5f, 0.5f);
            detailPanel.sizeDelta = new Vector2(240f, 430f);
            detailPanel.anchoredPosition = new Vector2(300f, 0f);
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.08f, 0.08f, 0.08f, 0.9f);

            detailTitleText = CreateText(detailPanel, "Detail_Title", new Vector2(0f, 180f), new Vector2(210f, 34f), 16, TextAnchor.MiddleCenter);
            detailBodyText = CreateText(detailPanel, "Detail_Body", new Vector2(0f, 105f), new Vector2(210f, 100f), 13, TextAnchor.UpperLeft);
            noteTitleInput = CreateInputField(detailPanel, "Note_Title_Input", new Vector2(0f, 25f), new Vector2(205f, 32f));
            noteBodyInput = CreateInputField(detailPanel, "Note_Body_Input", new Vector2(0f, -42f), new Vector2(205f, 82f));
            noteBodyInput.lineType = InputField.LineType.MultiLineNewline;
            saveNoteButton = CreateButton(detailPanel, "Save_Note_Button", "Save", new Vector2(-55f, -145f), new Vector2(90f, 34f));
            deleteNoteButton = CreateButton(detailPanel, "Delete_Note_Button", "Delete", new Vector2(55f, -145f), new Vector2(90f, 34f));
            saveNoteButton.onClick.AddListener(SaveSelectedNoteEdits);
            deleteNoteButton.onClick.AddListener(DeleteSelectedNote);
        }

        private static RectTransform CreateLayer(Transform parent, string name, Vector2 position)
        {
            GameObject layer = CreateUiObject(parent, name);
            RectTransform rect = layer.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600f, 400f);
            rect.anchoredPosition = position;
            return rect;
        }

        private static Text CreateText(Transform parent, string name, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = CreateUiObject(parent, name);
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            return text;
        }

        private static InputField CreateInputField(Transform parent, string name, Vector2 position, Vector2 size)
        {
            GameObject inputObject = CreateUiObject(parent, name);
            RectTransform rect = inputObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            Image image = inputObject.AddComponent<Image>();
            image.color = new Color(0.95f, 0.95f, 0.9f, 0.95f);

            Text text = CreateText(inputObject.transform, name + "_Text", Vector2.zero, size - new Vector2(12f, 8f), 13, TextAnchor.MiddleLeft);
            text.color = Color.black;

            InputField input = inputObject.AddComponent<InputField>();
            input.textComponent = text;
            return input;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
        {
            GameObject buttonObject = CreateUiObject(parent, name);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.2f, 0.24f, 1f);
            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            CreateText(buttonObject.transform, name + "_Label", Vector2.zero, size, 13, TextAnchor.MiddleCenter).text = label;
            return button;
        }

        private static GameObject CreateUiObject(Transform parent, string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            return obj;
        }

        private static GameObject CreateMarker(RectTransform parent, Vector2 position, Color color, float size, string name)
        {
            GameObject marker = CreateUiObject(parent, name);
            RectTransform rect = marker.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(size, size);
            Image image = marker.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
            return marker;
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

        private Color GetRoomColor(MapRoomViewModel room)
        {
            if (room.isCurrentRoom)
            {
                return Color.green;
            }

            if (!room.discovered)
            {
                return discoverySettings != null && discoverySettings.dimUndiscoveredRooms
                    ? new Color(0.28f, 0.28f, 0.28f, 0.55f)
                    : Color.gray;
            }

            return room.isOnMainRoute ? Color.yellow : Color.cyan;
        }

        private MapNoteSaveData FindNote(string noteId)
        {
            if (currentSave == null || currentSave.notes == null || string.IsNullOrWhiteSpace(noteId))
            {
                return null;
            }

            foreach (MapNoteSaveData note in currentSave.notes)
            {
                if (note != null && note.noteId == noteId)
                {
                    return note;
                }
            }

            return null;
        }

        private int CountNotesForRoom(string roomId)
        {
            if (currentSave == null || currentSave.notes == null || string.IsNullOrWhiteSpace(roomId))
            {
                return 0;
            }

            int count = 0;
            foreach (MapNoteSaveData note in currentSave.notes)
            {
                if (note != null && note.roomId == roomId)
                {
                    count++;
                }
            }

            return count;
        }

        private static void SetActive(Selectable selectable, bool active)
        {
            if (selectable != null)
            {
                selectable.gameObject.SetActive(active);
            }
        }
    }
}
