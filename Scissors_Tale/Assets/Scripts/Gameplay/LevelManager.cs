using UnityEngine;

/// <summary>
/// 레벨의 상태(준비, 진행 중, 완료, 실패) 관리
/// <para>레벨 안에서만 유지되는 싱글톤</para>
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    protected override bool DontDestroy => false; // 씬 전환 시 파괴됨
}

