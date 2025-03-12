using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource[] sfxSources;
    
    // 오디오 클립들
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;
    
    // 음소거 상태
    private bool bgmMuted = false;
    private bool sfxMuted = false;
    
    // 현재 볼륨 값 (0.0 ~ 1.0)
    private float bgmVolume = 1.0f;
    private float sfxVolume = 1.0f;
    
    private void Start()
    {
        LoadSettings();
    }
    
    private void LoadSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        bgmMuted = PlayerPrefs.GetInt("BGMMute", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SFXMute", 0) == 1;
        
        // 볼륨 적용
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
        
        // 음소거 적용
        MuteBGM(bgmMuted);
        MuteSFX(sfxMuted);
    }
    
    // 배경음 볼륨 설정 (0.0 ~ 1.0)
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        
        // 0 = -80
        float dbValue = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("BGMVolume", dbValue);

        MuteBGM(volume == 0);

        // 설정 저장
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }
    
    // 효과음 볼륨 설정 (0.0 ~ 1.0)
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        
        float dbValue = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("SFXVolume", dbValue);

        MuteSFX(volume == 0);
        
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
    
    public void MuteBGM(bool mute)
    {
        bgmMuted = mute;
        
        // 음소거 시 -80dB (거의 무음), 아닐 시 저장된 볼륨 적용
        float dbValue = mute ? -80f : (bgmVolume > 0.0001f ? Mathf.Log10(bgmVolume) * 20 : -80f);
        audioMixer.SetFloat("BGMVolume", dbValue);
        
        PlayerPrefs.SetInt("BGMMute", mute ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void MuteSFX(bool mute)
    {
        sfxMuted = mute;
        
        float dbValue = mute ? -80f : (sfxVolume > 0.0001f ? Mathf.Log10(sfxVolume) * 20 : -80f);
        audioMixer.SetFloat("SFXVolume", dbValue);
        
        PlayerPrefs.SetInt("SFXMute", mute ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    // 배경음악 재생
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length) return;
        
        // BGM 전환
        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }
    
    // 효과음 재생
    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length) return;
        
        // 재생 가능한 효과음 소스 찾기
        for (int i = 0; i < sfxSources.Length; i++)
        {
            if (!sfxSources[i].isPlaying)
            {
                sfxSources[i].clip = sfxClips[index];
                sfxSources[i].loop = false;
                sfxSources[i].Play();
                return;
            }
        }
        
        // 모든 소스가 사용 중이면 첫 번째 소스 재사용
        if (sfxSources.Length > 0)
        {
            sfxSources[0].Stop();
            sfxSources[0].clip = sfxClips[index];
            sfxSources[0].Play();
        }
    }
}