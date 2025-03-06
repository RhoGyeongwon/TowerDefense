using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int wayPointCount; //이동 경로 갯수
    private Transform[] wayPoints; //이동 경로 정보
    private int currentIndex = 0; //현재 목표지점 인덱스
    private Movement2D movement2D; //오브젝트 이동 제어
    private EnemySpawner enemySpawner;
    
    public void Setup(EnemySpawner _enemySpawner, Transform[] _wayPoints) // 초기 세팅
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = _enemySpawner;
        wayPointCount = _wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount]; // this.wayPoints = _wayPoints.ToArray();중 뭐가 좋을까? 그리고 new를 안해도 배열 대입 가능한데, 뭐가 제일 best지?
        this.wayPoints = _wayPoints;
        
        transform.position = _wayPoints[currentIndex].position;

        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        while (true)
        {
            transform.Rotate(Vector3.forward * 10);

            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {//이게 공식이 왜 0.02f * movement2D.MoveSpeed인지는 잘 모르겠다.
                NextMoveTo(); //다음 방향 설정
            }
            
            yield return null;
        }
    }

    private void NextMoveTo()
    {
        if (currentIndex < wayPoints.Length - 1)
        {
            transform.position = wayPoints[currentIndex].position; // 포지션에 근접할 경우 값 대입, 근데 거리니까 정밀하게 측정할 이유는 없을듯
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            // 거리를 정규화시킨건데 어떻게 vector3값이 나오지?
            // direction은 목표 지점까지의 거리 벡터(즉, Vector3 값)를 그대로 갖고 있음.
            // 거리가 멀면 direction 벡터의 크기(길이)가 커지고,
            // 거리가 가까우면 direction 벡터의 크기가 작아짐.
            // 결과적으로, 거리가 멀수록 빠르게 이동하고, 거리가 가까울수록 느리게 이동하는 문제 발생!
            
            // .normalized를 사용하면 벡터의 크기(길이)가 항상 1이 됨!
            // 이렇게 하면 속도가 거리와 관계없이 일정하게 유지됨.
            // 거리가 멀든 가깝든, 항상 같은 속도로 이동해서 위치가 밀착됨.
            
            //이때 direction은 벡터의 크기를 포함하고 있기 때문에, 거리(목표 위치와의 차이)가 커질수록 벡터의 크기도 커져
            
            // (50, 0, 0) - (0, 0, 0) = (50, 0, 0) → 크기(50)
            //     (10, 0, 0) - (0, 0, 0) = (10, 0, 0) → 크기(10)
            // 거리가 5라면 (5, 0, 0) → 크기(5)
            // 점점 크기가 작아지니까 속도가 느려지는 것처럼 보임.
            //     📌 결과: 목표점에 가까워질수록 점점 속도가 줄어들고 부드럽게 멈추는 것처럼 보임
            //공식 : vector3 / vector3의 길이
            movement2D.MoveTo(direction); //normalized로 정규화 안시키면 거리가 더 떨어져보이네.
        }
        else
        {
            OnDie();
        }
    }

    public void OnDie()
    {
        //EnemySpawner에서 리스트로 적 정보를 관리하기 때문에 Destroy()를 직접하지 않고
        //EnemySpawner에게 본인이 삭제될 때 필요한 처리를 하도록 DestroyEnemy() 함수 호출
        enemySpawner.DestroyEnemy(this);
    }
}

