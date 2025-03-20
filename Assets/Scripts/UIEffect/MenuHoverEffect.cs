using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MenuHoverEffect : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public Image buttonImage;
    
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.black;
    
    // 표시할 스프라이트를 할당 (Inspector에서 지정)
    public Sprite hoverSprite;
    
    // 애니메이션 시간 (초)
    public float animationDuration = 0.5f;
    
    private Coroutine currentCoroutine = null;

    private void Start()
    {
        if (buttonImage != null)
        {
            // 스프라이트를 할당하고 Filled 타입으로 설정
            buttonImage.sprite = hoverSprite;
            buttonImage.type = Image.Type.Filled;
            buttonImage.fillMethod = Image.FillMethod.Horizontal; // 수평으로 채워짐
            buttonImage.fillOrigin = 0; // 왼쪽부터 채워짐
            buttonImage.fillAmount = 0f; // 기본 상태는 완전 투명 (채워진 정도 0)
            // sprite 본연의 색상이 보이도록 흰색으로 유지
            buttonImage.color = Color.white;
        }
    }

    // PointerEnter 이벤트에 연결 (BaseEventData 매개변수 포함)
    public void OnHoverEnter(BaseEventData eventData)
    {
        if (tmpText != null)
            tmpText.color = hoverTextColor;
        
        if (buttonImage != null)
        {
            // 이미 진행 중인 애니메이션이 있다면 중지
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            // 왼쪽에서 오른쪽으로 채워지도록 애니메이션 시작
            currentCoroutine = StartCoroutine(AnimateFill(true));
        }

        AudioManager.Instance.PlaySFX("menuHovering");
    }
    
    // PointerExit 이벤트에 연결 (BaseEventData 매개변수 포함)
    public void OnHoverExit(BaseEventData eventData)
    {
        if (tmpText != null)
            tmpText.color = normalTextColor;
        
        if (buttonImage != null)
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            // 오른쪽에서 왼쪽으로 서서히 사라지도록 애니메이션 시작
            currentCoroutine = StartCoroutine(AnimateFill(false));
        }
    }
    
    // fillAmount를 애니메이션하는 코루틴
    private IEnumerator AnimateFill(bool filling)
    {
        float startFill = buttonImage.fillAmount;
        float targetFill = filling ? 1f : 0f;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            buttonImage.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }
        buttonImage.fillAmount = targetFill;
        currentCoroutine = null;
    }
}
