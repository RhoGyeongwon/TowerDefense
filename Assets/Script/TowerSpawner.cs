using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate towerTemplate;
    // [SerializeField] private GameObject towerPrefab;
    // [SerializeField] private int towerBuildGold = 50;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private SystemTextViewer systemTextViewer;

    public void SpawnTower(Transform tileTransform)
    {
        //if (towerBuildGold > playerGold.CurrentGold)
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            //골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        
        Tile tile = tileTransform.GetComponent<Tile>();

        //이미 타워가 건설되어 이씅면 타워 건설 x
        if (tile.IsBuildTower == true)
        {
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }
        
        tile.IsBuildTower = true;
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;
        Vector3 position = tileTransform.position + Vector3.back;
        //GameObject towerClone = Instantiate(towerPrefab, position, Quaternion.identity);
        GameObject towerClone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);
        //towerClone.GetComponent<TowerWeapon>().Setup(enemySpawner);
        towerClone.GetComponent<TowerWeapon>().Setup(enemySpawner, playerGold, tile);
    }
}
