using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;

    public void Setup(Transform target)
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return; // 적이 아닌 대상과 부딪히면
        if (collision.transform != target) return; // 현재 target인 적이 아닐 때
        
        collision.GetComponent<Enemy>().OnDie(); // 적 사망 함수 호출
        Destroy(gameObject); // 발사체 오브젝트 삭제
    }
}
