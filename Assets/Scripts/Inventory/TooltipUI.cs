using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private Vector2 offset = new Vector2(15, -15);

    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        Hide();
    }

    private void Update()
    {
        FollowMouse();
    }

    public void Show(ItemData item)
    {
        if (item == null) return;

        gameObject.SetActive(true);
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;

        FollowMouse();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void FollowMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float canvasScale = parentCanvas.scaleFactor;
        transform.position = mousePosition + (offset * canvasScale);

        Vector2 size = rectTransform.sizeDelta * canvasScale;
        float rightEdge = transform.position.x + size.x;
        float bottomEdge = transform.position.y - size.y;

        if (rightEdge > Screen.width)
        {
            transform.position = new Vector3(mousePosition.x - size.x - offset.x, transform.position.y, 0);
        }
        if (bottomEdge < 0)
        {
            transform.position = new Vector3(transform.position.x, mousePosition.y + size.y + offset.y, 0);
        }
    }
}