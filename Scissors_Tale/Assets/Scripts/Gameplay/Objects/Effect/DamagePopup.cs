using UnityEngine;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private float moveSpeed = 2f;
    private float lifeTime = 0.5f;
    private float fadeSpeed;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeSpeed = 1f / lifeTime;
    }

    public void Setup(Sprite damageSprite)
    {
        spriteRenderer.sprite = damageSprite;
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        float timer = 0f;
        Color color = spriteRenderer.color;

        while (timer < lifeTime)
        {
            // 위로 떠오름
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // 페이드 아웃
            timer += Time.deltaTime;

            
            color.a = Mathf.Lerp(1, 0, timer / lifeTime); 
            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
