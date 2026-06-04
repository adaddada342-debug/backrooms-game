using Backrooms.Mapping.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Backrooms.Mapping.UI.Widgets
{
    public class MapUiClickTarget : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        public MapSelectionType selectionType;
        public string targetId;
        public PrototypeMapCanvasController controller;

        public void Configure(
            PrototypeMapCanvasController newController,
            MapSelectionType newSelectionType,
            string newTargetId)
        {
            controller = newController;
            selectionType = newSelectionType;
            targetId = newTargetId ?? string.Empty;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (controller != null)
            {
                controller.HandleMapTargetClicked(selectionType, targetId);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (controller != null)
            {
                controller.HandleMapTargetHovered(selectionType, targetId);
            }
        }
    }
}
