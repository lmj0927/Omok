using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SoundType
{
    None,
    BGM,
    SFX
};

public class VolumeControlPanel : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private Toggle muteToggle;
    [SerializeField] private SoundType soundType;
    
    private bool isUpdatingUI = false;  // 무한 루프 방지용 플래그

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggleValueChanged);
        
        // 초기 값 설정
        string soundKey = soundType == SoundType.BGM ? "BGM" : "SFX";
        float savedVolume = PlayerPrefs.GetFloat(soundKey, 1f);
        bool isMuted = PlayerPrefs.GetInt(soundKey + "Mute", 0) == 1;
        
        UpdateUI(isMuted ? 0f : savedVolume, isMuted);
    }

    private void OnMuteToggleValueChanged(bool isMuted)
    {
        if (isUpdatingUI) return;
        
        string soundKey = soundType == SoundType.BGM ? "BGM" : "SFX";
        float volume = isMuted ? 0f : PlayerPrefs.GetFloat(soundKey, 1f);
        
        // 볼륨 설정 및 저장
        SetAudioVolume(volume);
        PlayerPrefs.SetInt(soundKey + "Mute", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        UpdateUI(volume, isMuted);
    }

    private void OnVolumeSliderValueChanged(float volume)
    {
        if (isUpdatingUI) return;
        
        string soundKey = soundType == SoundType.BGM ? "BGM" : "SFX";
        bool isMuted = volume <= 0.01f;
        
        // 볼륨 설정 및 저장
        SetAudioVolume(volume);
        PlayerPrefs.SetFloat(soundKey, volume);
        PlayerPrefs.SetInt(soundKey + "Mute", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        UpdateUI(volume, isMuted);
    }
    
    private void SetAudioVolume(float volume)
    {
        if (soundType == SoundType.BGM)
        {
            AudioManager.Instance.SetBGMVolume(volume);
        }
        else if (soundType == SoundType.SFX)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
    }
    
    private void UpdateUI(float volume, bool isMuted)
    {
        isUpdatingUI = true;
        
        volumeText.text = $"{(int)(volume * 100)}";
        volumeSlider.value = volume;
        muteToggle.isOn = isMuted;
        
        isUpdatingUI = false;
    }
}