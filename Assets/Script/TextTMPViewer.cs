using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;  // Text - TextMeshPro UI [플레이어의 체력]
    [SerializeField]
    private PlayerHP playerHP;  // 플레이어의 체력 정보

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
    }
}
