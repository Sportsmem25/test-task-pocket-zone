using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handler;

    private float handleRange = 50f;
    private Vector2 input = Vector2.zero;
    private Vector2 bgCenterScreen;

    private void Start()
    {
        if (background == null || handler == null) 
            Debug.LogError("Background or handle not assigned.");
        
        // ïåğåñ÷åò öåíòğà â screen space
        Vector3[] worldCorners = new Vector3[4];
        background.GetWorldCorners(worldCorners);
        
        // öåíòğ = ñğåäíåå
        bgCenterScreen = (RectTransformUtility.WorldToScreenPoint(null, worldCorners[0]) + 
            RectTransformUtility.WorldToScreenPoint(null, worldCorners[2])) * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pointerScreenPos = eventData.position;
        Vector2 delta = pointerScreenPos - bgCenterScreen;
        
        // Îãğàíè÷åíèå ïî ğàäèóñó handleRange
        if (delta.magnitude > handleRange) delta = delta.normalized * handleRange;

        // Ïåğåâîäèì handle â ëîêàëüíûå êîîğäèíàòû background'à
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, bgCenterScreen + delta, eventData.pressEventCamera, out localPoint);
        handler.anchoredPosition = localPoint;

        // Íîğìàëèçîâàííûé ââîä
        input = new Vector2(delta.x / handleRange, delta.y / handleRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handler.anchoredPosition = Vector2.zero;
    }
    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => input;
}