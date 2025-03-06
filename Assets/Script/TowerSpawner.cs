using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private EnemySpawner enemySpawner;

    public void SpawnTower(Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        //이미 타워가 건설되어 이씅면 타워 건설 x
        if (tile.IsBuildTower == true)
        {
            return;
        }
        
        tile.IsBuildTower = true;
        GameObject towerClone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        towerClone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
