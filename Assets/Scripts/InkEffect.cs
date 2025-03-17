using UnityEngine;
using UnityEngine.UI;

public class InkEffect : MonoBehaviour
{
    public Image uiImage;                      // Inspector에서 할당
    public float duration = 1f;                // 전체 애니메이션 시간
    public AnimationCurve revealCurve = AnimationCurve.Linear(0, 0, 1, 1); // 커스텀 커브
    public float blurAmount = 0.05f;           // 블러 영역의 크기 (Material에서 조절 가능)

    private Material revealMaterial;
    private float elapsedTime = 0f;

    void Start()
    {
        if(uiImage != null)
        {
            // Material 인스턴스를 생성하여 할당 (쉐어드 Material에 영향을 주지 않도록)
            revealMaterial = Instantiate(uiImage.material);
            uiImage.material = revealMaterial;
            revealMaterial.SetFloat("_Threshold", 0f);
            revealMaterial.SetFloat("_Blur", blurAmount);
        }
    }

    void Update()
    {
        if(revealMaterial != null && elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // 0~1 사이의 시간 값을 계산하고, 이를 커브에 대입하여 _Threshold 값을 결정
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            float thresholdValue = revealCurve.Evaluate(normalizedTime);
            revealMaterial.SetFloat("_Threshold", thresholdValue);
        }
    }
}