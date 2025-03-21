using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    public Toggle fullscreenToggle; // 인스펙터에서 연결

    void Start()
    {
        // 현재 전체화면 상태에 맞게 토글 초기화
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isFullscreen)
    {
        AudioManager.Instance.PlaySFX("buttonClick");
        // 토글 상태에 따라 전체화면 설정 변경
        Screen.fullScreen = isFullscreen;
    }
}