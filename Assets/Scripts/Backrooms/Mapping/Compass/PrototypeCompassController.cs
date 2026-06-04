using Backrooms.Mapping.Discovery;
using Backrooms.Runtime.LevelContext;
using UnityEngine;
using UnityEngine.UI;

namespace Backrooms.Mapping.Compass
{
    public class PrototypeCompassController : MonoBehaviour
    {
        public Camera playerCamera;
        public GeneratedLevelRuntimeContext levelContext;
        public MapDiscoveryTracker discoveryTracker;
        public KeyCode toggleKey = KeyCode.C;
        public bool visible = true;
        public Text compassText;

        private void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }

            if (levelContext == null)
            {
                levelContext = FindAnyObjectByType<GeneratedLevelRuntimeContext>();
            }

            if (discoveryTracker == null)
            {
                discoveryTracker = FindAnyObjectByType<MapDiscoveryTracker>();
            }

            BuildUi();
            SetVisible(visible);
            UpdateCompassText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                SetVisible(!visible);
            }

            if (visible)
            {
                UpdateCompassText();
            }
        }

        private void SetVisible(bool newVisible)
        {
            visible = newVisible;
            if (compassText != null)
            {
                compassText.gameObject.SetActive(visible);
            }
        }

        private void BuildUi()
        {
            if (compassText != null)
            {
                return;
            }

            GameObject canvasObject = new GameObject("Prototype_Compass_Canvas");
            canvasObject.transform.SetParent(transform, false);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject textObject = new GameObject("Compass_Text");
            textObject.transform.SetParent(canvasObject.transform, false);
            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -18f);
            rect.sizeDelta = new Vector2(420f, 32f);

            compassText = textObject.AddComponent<Text>();
            compassText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            compassText.fontSize = 16;
            compassText.alignment = TextAnchor.MiddleCenter;
            compassText.color = Color.white;
        }

        private void UpdateCompassText()
        {
            if (compassText == null)
            {
                return;
            }

            float heading = playerCamera == null ? 0f : playerCamera.transform.eulerAngles.y;
            string roomId = discoveryTracker == null ? string.Empty : discoveryTracker.currentRoomId;
            if (string.IsNullOrWhiteSpace(roomId))
            {
                roomId = "unknown";
            }

            compassText.text = Mathf.RoundToInt(heading) + " deg | " + roomId;
        }
    }
}
