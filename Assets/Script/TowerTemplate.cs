using UnityEngine;

[CreateAssetMenu] //이걸 하면 하이어라키에서 생성할 때 보이는거같기도 하고..
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab; // 타워 생성을 위한 프리팹
    public GameObject followTowerPrefab;
    public Weapon[] weapon; // 레벨별 타워(무기) 정보

    [System.Serializable] //유니티에서 구조체(struct)나 클래스를 "인스펙터에서 보이게" 해주는 기능
    public struct Weapon
    {
        public Sprite sprite; // 보여지는 타워 이미지 (UI)
        public float damage; // 공격력
        public float rate; // 공격 속도
        public float range; // 공격 범위
        public int cost; // 필요 골드 (0레벨: 건설, 1~레벨: 업그레이드)
        public int sell; // 타워 판매 시 획득 골드
    }
}