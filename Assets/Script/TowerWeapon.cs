using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponType
{
    Cannon,
    Laser
}
public enum WeaponState
{
    SearchTarget = 0,
    TryAttackCannon,
    TryAttackLaser
    //AttackToTarget
}

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField] private TowerTemplate towerTemplate;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WeaponType weaponType;
    // [SerializeField] private float attackRate = 0.5f;
    // [SerializeField] private float attackRange = 2.0f;
    // [SerializeField] private int attackDamage = 1;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private EnemySpawner enemySpawner;
    
    [Header("Cannon")]
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Laser")]
    [SerializeField] private LineRenderer lineRenderer; // 레이저로 사용되는 선 (LineRenderer)
    [SerializeField] private Transform hitEffect; // 타격 효과
    [SerializeField] private LayerMask targetLayer; // 광선에 부딪히는 레이어 설정
    
    private int level = 0;
    private SpriteRenderer spriteRenderer;
    private PlayerGold playerGold;
    private Tile ownerTile;
    
    public Sprite TowerSprite => towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int MaxLevel => towerTemplate.weapon.Length;
    
    // public float Damage => attackDamage;
    // public float Rate => attackRate;
    // public float Range => attackRange;
    public int Level => level + 1;
    
    public void Setup(EnemySpawner _enemySpawner, PlayerGold _playerGold, Tile _ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = _enemySpawner;
        this.playerGold = _playerGold;
        this.ownerTile = _ownerTile;
        
        ChangeState(WeaponState.SearchTarget); //최초 상태를 WeaponState.SearchTarget으로 설정
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());
        weaponState = newState;
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        // 원점으로부터의 거리와 수평축으로부터의 각도를 이용해 위치를 구하는 극 좌표계 이용
        // 각도 = arctan(y/x)
        // x,y 변위값 구하기
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x,y 변위값을 바탕으로 각도 구하기
        // 각도가 radian 단위이기 때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
        float degree  = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            // // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
            // float closestDistSqr = Mathf.Infinity;
            // // EnemySpawner의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
            // for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            // {
            //     float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            //     
            //     // 현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
            //     //if (distance <= attackRange && distance <= closestDistSqr)
            //     if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            //     {
            //         closestDistSqr = distance;
            //         attackTarget = enemySpawner.EnemyList[i].transform;
            //     }
            // }

            attackTarget = FindClosestAttackTarget();
            
            if (attackTarget != null)
            {
                if (weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if (weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }
            
            yield return null;
        }
    }

    // private IEnumerator AttackToTarget()
    // {
    //     while (true)
    //     {
    //         if (attackTarget == null)
    //         {
    //             ChangeState(WeaponState.SearchTarget);
    //             break;
    //         }
    //         
    //         float distance = Vector3.Distance(attackTarget.position, transform.position);
    //         //if (distance > attackRange)
    //         if (distance > towerTemplate.weapon[level].range)
    //         {
    //             attackTarget = null;
    //             ChangeState(WeaponState.SearchTarget);
    //             break;
    //         }
    //
    //         //yield return new WaitForSeconds(attackRate);
    //         yield return new WaitForSeconds(towerTemplate.weapon[level].rate);
    //         SpawnProjectile();
    //     }
    // }
    
    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            // if (attackTarget == null)
            // {
            //     ChangeState(WeaponState.SearchTarget);
            //     break;
            // }
            //
            // float distance = Vector3.Distance(attackTarget.position, transform.position);
            // //if (distance > attackRange)
            // if (distance > towerTemplate.weapon[level].range)
            // {
            //     attackTarget = null;
            //     ChangeState(WeaponState.SearchTarget);
            //     break;
            // }

            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // 레이저, 레이저 타격 효과 활성화
        EnableLaser();

        while (true)
        {
            // target을 공격할 수 있는지 검사
            if (IsPossibleToAttackTarget() == false)
            {
                // 레이저, 레이저 타격 효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 레이저 공격
            SpawnLaser();

            yield return null;
        }
    }

    private Transform FindClosestAttackTarget()
    {
        // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
        float closestDistSqr = Mathf.Infinity;
        Transform closestTarget = null;

        // EnemySpawner의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            Transform enemyTransform = enemySpawner.EnemyList[i].transform;
            float distance = Vector3.Distance(enemyTransform.position, transform.position);

            // 현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
            if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                closestTarget = enemyTransform;
            }
        }

        return closestTarget;
    }
    
    private bool IsPossibleToAttackTarget()
    {
        // target이 존재하는지 검사 (다른 발사체에 의해 제거, 목표 지점 도달로 삭제된 경우 등)
        if (attackTarget == null)
        {
            return false;
        }

        // target이 공격 범위 안에 있는지 검사 (공격 범위를 벗어나면 새로운 적 탐색)
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if (distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null; // 타겟을 잃음
            return false;
        }

        return true;
    }
    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        //clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }
    
    private void EnableLaser()
    {
        // 레이저 선과 타격 효과 활성화
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        // 레이저 선과 타격 효과 비활성화
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        // 레이저 방향 벡터 설정 (공격 대상 - 발사 위치)
        Vector3 direction = attackTarget.position - spawnPoint.position;

        // 2D 물리 레이캐스트 실행 (발사 위치에서 지정된 방향으로 광선을 발사하여 충돌 감지)
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, 
            towerTemplate.weapon[level].range, targetLayer);

        // 같은 방향으로 여러 개의 광선을 쏴서 그중 현재 attackTarget과 동일한 오브젝트를 찾음
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform == attackTarget)
            {
                // 레이저 선의 시작 위치 설정 (발사 위치)
                lineRenderer.SetPosition(0, spawnPoint.position);

                // 레이저 선의 목표 지점 설정 (충돌한 위치)
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);

                // 타격 효과 위치 설정
                hitEffect.position = hit[i].point;

                // 적 체력 감소 (1초에 damage 값만큼 감소)
                attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage * Time.deltaTime);
            }
        }
    }

    public bool Upgrade()
    {
        // 타워 업그레이드에 필요한 골드가 충분한지 검사
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false; // 골드가 부족하면 업그레이드 불가
        }

        // 타워 레벨 증가
        level++;

        // 타워 외형 변경 (Sprite)
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;

        // 골드 차감
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        // 무기 속성이 레이저이면
        if (weaponType == WeaponType.Laser)
        {
            // 레벨에 따라 레이저의 굵기 설정
            lineRenderer.startWidth = 0.05f + level * 0.05f; // 시작 지점의 굵기
            lineRenderer.endWidth = 0.05f; // 끝 지점의 굵기
        }

        return true; // 업그레이드 성공
    }
    
    public void Sell()
    {
        // 골드 증가
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;

        // 현재 타일에 다시 타워 건설이 가능하도록 설정
        ownerTile.IsBuildTower = false;

        // 타워 파괴
        Destroy(gameObject);
    }
}
