using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP;  // Text - TextMeshPro UI [플레이어의 체력]
    [SerializeField] private TextMeshProUGUI textPlayerGold;
    [SerializeField] private PlayerHP playerHP;  // 플레이어의 체력 정보
    [SerializeField] private PlayerGold playergold;  // 플레이어의 체력 정보

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
        textPlayerGold.text = playergold.CurrentGold.ToString();
    }
}
