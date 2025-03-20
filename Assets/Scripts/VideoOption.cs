using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class VideoOption : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;   // 드롭다운 연결
    public Toggle verticalToggle;             // 세로 모드 토글 연결

    private Resolution[] allResolutions;

    void Start()
    {
        // 시스템 해상도 목록 가져오기
        allResolutions = Screen.resolutions;

        // 토글 이벤트 등록: 토글 상태가 변경되면 드롭다운 갱신
        verticalToggle.onValueChanged.AddListener(delegate { RefreshDropdown(); });

        // 드롭다운 선택 변경 시 해상도 적용
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChanged(); });

        // 초기 드롭다운 옵션 설정
        RefreshDropdown();
    }

    // 토글 상태에 따라 해상도 목록을 갱신하는 함수
    void RefreshDropdown()
    {
        // 60Hz 해상도만 필터링
        List<Resolution> filteredResolutions = new List<Resolution>();
        foreach (Resolution res in allResolutions)
        {
            if (res.refreshRate == 60)
            {
                // 토글 상태에 따라 가로/세로 필터 적용
                if (verticalToggle.isOn)
                {
                    if (res.height > res.width)
                        filteredResolutions.Add(res);
                }
                else
                {
                    if (res.width >= res.height)
                        filteredResolutions.Add(res);
                }
            }
        }

        // 화면 비율별 대표 해상도 선택 (동일 비율 중 면적이 큰 해상도)
        Dictionary<string, Resolution> aspectResolutions = new Dictionary<string, Resolution>();
        foreach (Resolution res in filteredResolutions)
        {
            int gcd = GCD(res.width, res.height);
            int aspectW = res.width / gcd;
            int aspectH = res.height / gcd;
            string aspectKey = aspectW + ":" + aspectH;

            if (aspectResolutions.ContainsKey(aspectKey))
            {
                Resolution existing = aspectResolutions[aspectKey];
                if ((res.width * res.height) > (existing.width * existing.height))
                {
                    aspectResolutions[aspectKey] = res;
                }
            }
            else
            {
                aspectResolutions.Add(aspectKey, res);
            }
        }

        // 대표 해상도를 면적 내림차순으로 정렬 (높은 해상도가 위쪽)
        List<Resolution> sortedResolutions = aspectResolutions.Values
            .OrderByDescending(r => r.width * r.height).ToList();

        // 드롭다운 옵션 갱신
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < sortedResolutions.Count; i++)
        {
            Resolution res = sortedResolutions[i];
            int gcd = GCD(res.width, res.height);
            string aspectRatio = (res.width / gcd) + ":" + (res.height / gcd);
            string optionText = string.Format("{0} x {1} ({2})", res.width, res.height, aspectRatio);
            options.Add(optionText);

            // 현재 화면 해상도와 일치하는 경우 인덱스 지정
            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // 해상도 변경 함수 (드롭다운 선택에 따라 적용)
    void OnResolutionChanged()
    {
        // 현재 토글 상태에 따라 다시 필터링 후 정렬 (RefreshDropdown()와 동일 로직)
        List<Resolution> filteredResolutions = new List<Resolution>();
        foreach (Resolution res in allResolutions)
        {
            if (res.refreshRate == 60)
            {
                if (verticalToggle.isOn)
                {
                    if (res.height > res.width)
                        filteredResolutions.Add(res);
                }
                else
                {
                    if (res.width >= res.height)
                        filteredResolutions.Add(res);
                }
            }
        }

        Dictionary<string, Resolution> aspectResolutions = new Dictionary<string, Resolution>();
        foreach (Resolution res in filteredResolutions)
        {
            int gcd = GCD(res.width, res.height);
            int aspectW = res.width / gcd;
            int aspectH = res.height / gcd;
            string aspectKey = aspectW + ":" + aspectH;

            if (aspectResolutions.ContainsKey(aspectKey))
            {
                Resolution existing = aspectResolutions[aspectKey];
                if ((res.width * res.height) > (existing.width * existing.height))
                {
                    aspectResolutions[aspectKey] = res;
                }
            }
            else
            {
                aspectResolutions.Add(aspectKey, res);
            }
        }
        List<Resolution> sortedResolutions = aspectResolutions.Values
            .OrderByDescending(r => r.width * r.height).ToList();

        int index = resolutionDropdown.value;
        if (index < sortedResolutions.Count)
        {
            Resolution chosen = sortedResolutions[index];
            Screen.SetResolution(chosen.width, chosen.height, Screen.fullScreen, chosen.refreshRate);
        }
    }

    // 최대공약수(GCD) 계산 함수 (화면 비율 계산에 사용)
    int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
