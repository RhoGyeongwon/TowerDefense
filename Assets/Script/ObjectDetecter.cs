using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetecter : MonoBehaviour
{
    [SerializeField] private TowerSpawner towerSpawner;
    [SerializeField] private TowerDataViewer towerDataViewer;
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null; //마우스 픽킹으로 선택한 오브젝트 임시 저장
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() == true) //마우스가 UI에 머물러 있을 때는 아래 코드가 실행되지 않도록 함
        {
            return;
        }
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
                hitTransform = hit.transform;
                
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
        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스를 눌렀을 때 선택한 오브젝트가 없거나 선택한 오브젝트가 타워가 아니면 타워 정보가 사라진다.
            if (hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                // 타워 정보 패널을 비활성화 한다
                towerDataViewer.OffPanel();
            }

            // 선택된 오브젝트 초기화
            hitTransform = null;
        }
    }
}
