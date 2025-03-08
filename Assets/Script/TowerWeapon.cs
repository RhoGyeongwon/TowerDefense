using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponType
{
    Cannon
}
public enum WeaponState
{
    SearchTarget = 0,
    TryAttackCannon
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
                //ChangeState(WeaponState.AttackToTarget);
                ChangeState(WeaponState.TryAttackCannon);
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
