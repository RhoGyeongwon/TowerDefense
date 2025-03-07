using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int towerBuildGold = 50;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerGold playerGold;

    public void SpawnTower(Transform tileTransform)
    {
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }
        
        Tile tile = tileTransform.GetComponent<Tile>();

        //이미 타워가 건설되어 이씅면 타워 건설 x
        if (tile.IsBuildTower == true)
        {
            return;
        }
        
        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerBuildGold;
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject towerClone = Instantiate(towerPrefab, position, Quaternion.identity);
        towerClone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
