using System.Collections;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate towerTemplate;
    // [SerializeField] private GameObject towerPrefab;
    // [SerializeField] private int towerBuildGold = 50;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private SystemTextViewer systemTextViewer;
    private bool isOnTowerButton = false; //타워 건설 버튼을 눌렀는지 체크
    private GameObject followTowerClone = null;
    
    public void ReadyToSpawnTower()
    {
        if (isOnTowerButton == true) //버튼을 중복해서 누르는 것을 방지하기 위해 필요
        {
            return;
        }
        
        // 타워 건설 가능 여부 확인
        // 타워를 건설할 만큼 돈이 없으면 타워 건설 X
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        {
            // 골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // 타워 건설 버튼을 눌렀다고 설정
        isOnTowerButton = true;
        // 마우스를 따라다니는 임시 타워 생성
        followTowerClone = Instantiate(towerTemplate.followTowerPrefab);
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransform)
    {
        if (isOnTowerButton == false)
        {
            return;
        }
        //if (towerBuildGold > playerGold.CurrentGold)
        // if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)
        // {
        //     //골드가 부족해서 타워 건설이 불가능하다고 출력
        //     systemTextViewer.PrintText(SystemType.Money);
        //     return;
        // }
        
        Tile tile = tileTransform.GetComponent<Tile>();

        //이미 타워가 건설되어 이씅면 타워 건설 x
        if (tile.IsBuildTower == true)
        {
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }
        
        isOnTowerButton = false; // 다시 타워 건설 버튼을 눌러서 타워를 건설하도록 변수 설정
        tile.IsBuildTower = true;
        //playerGold.CurrentGold -= towerBuildGold;
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;
        Vector3 position = tileTransform.position + Vector3.back;
        //GameObject towerClone = Instantiate(towerPrefab, position, Quaternion.identity);
        GameObject towerClone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);
        //towerClone.GetComponent<TowerWeapon>().Setup(enemySpawner);
        towerClone.GetComponent<TowerWeapon>().Setup(enemySpawner, playerGold, tile);
        
        Destroy(followTowerClone); // 타워를 제거했기 때문에 마우스를 따라다니느 임시 타워 삭제
        StartCoroutine("OnTowerCancelSystem");
    }
    
    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            // ESC 키 또는 마우스 오른쪽 버튼을 눌렀을 때 타워 건설 취소
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;

                // 마우스를 따라다니는 임시 타워 삭제
                Destroy(followTowerClone);
                break;
            }

            yield return null;
        }
    }
}
