using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro 사용을 위해 필요

public class TMPAlpha : MonoBehaviour
{
    [SerializeField]
    private float lerpTime = 0.5f; // 페이드 아웃에 걸리는 시간
    private TextMeshProUGUI text; // UI 텍스트 (TextMeshPro)

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>(); // 현재 오브젝트의 TextMeshProUGUI 컴포넌트 가져오기
    }

    public void FadeOut()
    {
        StartCoroutine(AlphaLerp(1, 0)); // 알파 값 1 → 0으로 변경 (페이드 아웃)
    }

    private IEnumerator AlphaLerp(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while (percent < 1)
        {
            // lerpTime 시간 동안 while() 반복문 실행
            currentTime += Time.deltaTime;
            percent = currentTime / lerpTime;

            // 텍스트의 색상 변경 (알파 값 조정)
            Color color = text.color;
            color.a = Mathf.Lerp(start, end, percent); // start 값에서 end 값으로 보간
            text.color = color;

            yield return null; // 다음 프레임까지 대기
        }
    }
}