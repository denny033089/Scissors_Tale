using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드 효과 및 배경 음악 관리
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
/// [System.Serializable]
[System.Serializable]
public class SoundData
{
    public string soundName; // sfx/bgm의 이름
    public AudioClip clip;
}
public class SoundManager : Singleton<SoundManager>
{
    [Header("Audio Source")]
    //오디오 전반
    public AudioSource BGM_Source;
    public AudioSource SFX_Source;

    [Header("Sound Library")]
    //해당 라이브러리에 사운드 저장
    public List<SoundData> BGM_Library = new List<SoundData>();
    public List<SoundData> SFX_Library = new List<SoundData>();

    protected override void Awake()
    {
        base.Awake();
        if (BGM_Source == null) BGM_Source = gameObject.AddComponent<AudioSource>();
        if (SFX_Source == null) SFX_Source = gameObject.AddComponent<AudioSource>();

        BGM_Source.loop = true;
        SFX_Source.loop = false;
    }
    //BGM 재생과 정지
    public void PlayBGM(string name)
    {
        SoundData data = BGM_Library.Find(x => x.soundName == name);
        if (data != null && data.clip != null)
        {
            if (BGM_Source.clip == data.clip && BGM_Source.isPlaying) return; //똑같은 노래 재생 방지

            BGM_Source.clip = data.clip;
            BGM_Source.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{name}' 못찾음.");
        }
    }

    public void StopBGM()
    {
        BGM_Source.Stop();
    }
    //SFX 재생
    public void PlaySFX(string name)
    {
        SoundData data = SFX_Library.Find(x => x.soundName == name);
        if (data != null && data.clip != null)
        {
            SFX_Source.PlayOneShot(data.clip);
        }
        else
        {
            Debug.LogWarning($"SFX '{name}' 못찾음");
        }
    }
}

