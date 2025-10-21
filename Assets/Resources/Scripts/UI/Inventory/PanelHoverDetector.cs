using UnityEngine;
using UnityEngine.EventSystems;

public class PanelHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseOverPanel = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOverPanel = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOverPanel = false;
    }
}