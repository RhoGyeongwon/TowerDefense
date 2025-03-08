using UnityEngine;
using TMPro; // TextMeshPro 사용을 위해 필요

// 시스템 메시지 타입 (돈 부족, 건설 불가)
public enum SystemType 
{ 
    Money = 0, // 돈 부족
    Build // 건설 불가
}

public class SystemTextViewer : MonoBehaviour
{
    private TextMeshProUGUI textSystem; // 시스템 메시지를 표시할 UI 텍스트
    private TMPAlpha tmpAlpha; // 텍스트 페이드 아웃을 위한 TMPAlpha 스크립트

    private void Awake()
    {
        // 현재 오브젝트에서 TextMeshProUGUI와 TMPAlpha 컴포넌트 가져오기
        textSystem = GetComponent<TextMeshProUGUI>();
        tmpAlpha = GetComponent<TMPAlpha>();
    }

    public void PrintText(SystemType type)
    {
        switch (type)
        {
            case SystemType.Money:
                textSystem.text = "System : Not enough money..."; // 돈 부족 메시지 출력
                break;
            case SystemType.Build:
                Debug.Log(textSystem.text);
                textSystem.text = "System : Invalid build tower..."; // 건설 불가 메시지 출력
                break;
        }

        tmpAlpha.FadeOut(); // 메시지를 출력한 후 페이드 아웃 실행
    }
}