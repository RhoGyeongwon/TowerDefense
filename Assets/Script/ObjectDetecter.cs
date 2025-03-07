using UnityEngine;

public class ObjectDetecter : MonoBehaviour
{
    [SerializeField] private TowerSpawner towerSpawner;
    [SerializeField] private TowerDataViewer towerDataViewer;
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) //마우스 왼쪽 버튼을 눌렀을 때
        {
            // ray.origin : 광선의 시작 위치 ( = 카메라 위치 )
            // ray.direction : 광선의 진행 방향
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // ray에 부딪히는 오브젝트를 검출해서 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) 
                // ray이 감지할 수 있는 최대 거리. 만약 멀리 있는 오브젝트는 클릭해도 안먹게 하고싶다면
                // 길이를 조정할 수 있다.
            {
                    // 광선에 부딪힌 오브젝트의 태그가 "Tile"이면
                if (hit.transform.CompareTag("Tile"))
                {
                    // 타워가 생성하는 SpawnTower() 호출
                    towerSpawner.SpawnTower(hit.transform);
                }
                else if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                }
            }
        }
    }
}
