using UnityEngine;
using UnityEngine.UI;

namespace Backrooms.Mapping.UI
{
    public static class UiLineUtility
    {
        public static Image CreateLine(RectTransform parent, Vector2 start, Vector2 end, float thickness, Color color, string name)
        {
            if (parent == null)
            {
                return null;
            }

            GameObject lineObject = new GameObject(name);
            lineObject.transform.SetParent(parent, false);
            Image image = lineObject.AddComponent<Image>();
            image.color = color;
            RectTransform rect = lineObject.GetComponent<RectTransform>();
            Vector2 delta = end - start;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = start + delta * 0.5f;
            rect.sizeDelta = new Vector2(delta.magnitude, thickness);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            return image;
        }
    }
}
