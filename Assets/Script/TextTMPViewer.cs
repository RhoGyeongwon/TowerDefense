using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP;  // Text - TextMeshPro UI [플레이어의 체력]
    [SerializeField] private TextMeshProUGUI textPlayerGold;
    [SerializeField] private TextMeshProUGUI textWave;
    [SerializeField] private TextMeshProUGUI textEnemyCount;
    [SerializeField] private PlayerHP playerHP;  // 플레이어의 체력 정보
    [SerializeField] private PlayerGold playergold;  // 플레이어의 체력 정보
    [SerializeField] private WaveSystem waveSystem;  // 플레이어의 체력 정보
    [SerializeField] private EnemySpawner enemySpawner;  // 플레이어의 체력 정보

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
        textPlayerGold.text = playergold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
    }
}
