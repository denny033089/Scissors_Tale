using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Settings")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    
    [TextArea(3, 5)]
    public string skillDescription = "";

    [Header("Tooltip Position")]
    [Tooltip("Offset from button position (in pixels). Positive X = right, Positive Y = up")]
    public Vector2 tooltipOffset = new Vector2(0, 50);

    private RectTransform _buttonRectTransform;
    private RectTransform _tooltipRectTransform;

    void Start()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
            _tooltipRectTransform = tooltipPanel.GetComponent<RectTransform>();
        }
        
        _buttonRectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            // Set tooltip text
            if (tooltipText != null && !string.IsNullOrEmpty(skillDescription))
            {
                tooltipText.text = skillDescription;
            }

            // Position tooltip relative to button (stationary, not following mouse)
            if (_buttonRectTransform != null && _tooltipRectTransform != null)
            {
                // Get button's world position
                Vector3 buttonWorldPos = _buttonRectTransform.position;
                
                // Calculate tooltip position with offset
                Vector3 tooltipPos = buttonWorldPos + new Vector3(tooltipOffset.x, tooltipOffset.y, 0);
                _tooltipRectTransform.position = tooltipPos;
            }

            tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void OnDisable()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}
