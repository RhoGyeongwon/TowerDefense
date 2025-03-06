using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyHPSliderPrefab;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private float spawnTime;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private PlayerHP playerHP;
    [SerializeField] private PlayerGold playerGold;
    private List<Enemy> enemyList;
    public List<Enemy> EnemyList => enemyList;
    void Awake()
    {
        enemyList = new List<Enemy>();
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);
            SpawnEnemyHPSlider(clone); //일단여기추가
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        else if (type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;
        }
        
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);
        sliderClone.transform.SetParent(canvasTransform);
        sliderClone.transform.localPosition = Vector3.one;
        
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
