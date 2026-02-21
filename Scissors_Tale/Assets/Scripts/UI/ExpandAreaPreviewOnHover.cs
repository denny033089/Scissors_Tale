using UnityEngine;
using UnityEngine.EventSystems;

public class ExpandAreaPreviewOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.SetExpandAreaPreview(true);
            if (GameManager.Instance != null)
                GameManager.Instance.UpdateAttackAreaTiles();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.SetExpandAreaPreview(false);
            if (GameManager.Instance != null)
                GameManager.Instance.UpdateAttackAreaTiles();
        }
    }

    void OnDisable()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.SetExpandAreaPreview(false);
            if (GameManager.Instance != null)
                GameManager.Instance.UpdateAttackAreaTiles();
        }
    }
}
