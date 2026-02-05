using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : Piece //02.04 정수민
{
    [Header("UI")]
    public TMP_Text HPText;

    public GameObject DamagePopupPrefab;
    public Transform PopupSpawnPoint;

    [Header("스탯")]
    public int CurrentHP = 20;
    public int MaxHP = 20;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void InitializeStats() {
        CurrentHP = MaxHP;
        UpdateHPText();
    }

    public virtual void SpawnDamageEffect(Sprite sprite)
    {
        if (DamagePopupPrefab == null || sprite == null) return;

        // 몬스터 위에서 스폰
        Vector3 spawnPos = (PopupSpawnPoint != null) ? PopupSpawnPoint.position : transform.position + Vector3.up * 1.0f;

        GameObject popupObj = Instantiate(DamagePopupPrefab, spawnPos, Quaternion.identity);
        DamagePopup popupScript = popupObj.GetComponent<DamagePopup>();

        if (popupScript != null)
        {
            popupScript.Setup(sprite);
        }
    }
    
    public virtual void TakeDamage(int damage)
    {
        for(int i = 0;i<damage;i++) {
            CurrentHP --;
            //SpawnDamageEffect(d);
        }

        if (CurrentHP < 0) CurrentHP = 0;

        Debug.Log($"[{gameObject.name} : {GetInstanceID()}] HP: {CurrentHP}");

        UpdateHPText();

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public virtual void UpdateHPText()  //01.19 정수민: public virtual로 수정
    {
        if (HPText != null)
        {
            HPText.text = $"{CurrentHP}/{MaxHP}";
        }
    }

    public virtual void Die() //01.19 정수민: public virtual로 수정 + MapManager 추가
    {
        Debug.Log("플레이어 사망");
        // 보드에서 지우기
        MapManager.Instance.Pieces[MyPos.Item1, MyPos.Item2] = null;

        // 오브젝트 삭제
        Destroy(gameObject);

    }
}
