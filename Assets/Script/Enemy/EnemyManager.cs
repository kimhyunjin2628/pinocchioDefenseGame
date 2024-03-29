using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Load한 Enemy 프리팹 정보
    Dictionary<string, GameObject> prefabCaChes = new Dictionary<string, GameObject>();

    // 활성화된 enemy를 받아올 리스트
    public List<GameObject> enemies;

    [SerializeField]
    public Transform enemyParents;

    //filePath, cacheCount 저장
    [SerializeField]
    PrefabCacheData[] prefabCacheDatas;

    [Header("Weight_Hard")]
    [SerializeField]
    float powerWeight_hard;

    [SerializeField]
    float speedWeight_hard;

    [SerializeField]
    float defenseWeight_hard;

    [SerializeField]
    float hpWeight_hard;

    [SerializeField]
    float rewardWeight_hard;

    // Start is called before the first frame update
    void Start()
    {
        PrepareData();
    }

    /// <summary>
    /// 씬 로드 후 Enemy 캐시 데이터를 바탕으로 생성할 함수 호출 : 김현진
    /// </summary>
    void PrepareData()
    {
        for (int i = 0; i < prefabCacheDatas.Length; i++)
        {
            SystemManager.Instance.PrefabCacheSystem.GeneratePrefabCache(prefabCacheDatas[i].filePath, prefabCacheDatas[i].cacheCount, Load(prefabCacheDatas[i].filePath), enemyParents);      
        }
    }

    /// <summary>
    /// 프리팹 경로를 통해 게임오브젝트를 가져온다 : 김현진
    /// </summary>
    /// <param name="filePath">프리팹이 저장되있는 경로</param>
    /// <returns>경로에서 가져온 게임 오브젝트</returns>
    GameObject Load(string filePath)
    {
        //이미 캐시에 포함되어 있을 경우
        if (prefabCaChes.ContainsKey(filePath))
            return prefabCaChes[filePath];
        else
        {
            GameObject go = Resources.Load<GameObject>(filePath);
            prefabCaChes.Add(filePath, go);

            return go;
        }
    }

    /// <summary>
    /// Enemy 객체를 생성
    /// </summary>
    /// <param name="enemyIndex">생성할 Enemy가 저장될 인덱스</param>
    /// <param name="gateNum">생성될 Gate 인덱스 0~2</param>
    /// <param name="targetTile">생성될 Enemy가 따라갈 targetPoint번호</param>
    public void EnableEnemy(int enemyIndex, int gateNum, int[] targetPoint)
    {
        //예외처리
        if (enemyIndex >= prefabCacheDatas.Length || prefabCacheDatas[enemyIndex].filePath == null)
            return;

        //생성한 프리팹 게임오브젝트 정보 받아오기
        GameObject go = SystemManager.Instance.PrefabCacheSystem.EnablePrefabCache(prefabCacheDatas[enemyIndex].filePath);     

        if (go == null)
            return;

        //생성한 프리팹이 유효할 경우
        Enemy enemy = go.GetComponent<Enemy>();
        enemies.Add(go);

        //생성한 프리팹 객체 변수 초기화
        enemy.enemyIndex = enemies.FindIndex(x => x == go); //enemise 리스트의 인덱스와 일치하는 번호 저장

        enemy.gateNum = gateNum;
        enemy.targetPoint = SystemManager.Instance.BlockManager.CreateTargetArr(targetPoint);

        //적을 초기상태로
        enemy.Reset();

        // 에너미 상태 관리 패널 생성        
        GameObject statusMngPanelGo = SystemManager.Instance.PanelManager.EnablePanel<StatusMngPanel>(3, go);
        
        if (!statusMngPanelGo)
        {
            Debug.Log("Enable statusMngPanel is null");
            return;
        }
        
        StatusMngPanel statusMngPanel = statusMngPanelGo.GetComponent<StatusMngPanel>();

        // 패널 정보 넘겨줌
        enemy.statusMngPanel = statusMngPanel;

        statusMngPanel.panelPos = enemy.hpPos.transform.position;
        statusMngPanel.hpBarOwner = go;
        statusMngPanel.SetHPBarColor(enemy.maxHP, go.tag);

        //하드모드일경우 능력치 가중치 부여
        /*
        if (SystemManager.Instance.UserInfo.selectMode == 1)//하드모드
        {
            //hp증가
            enemy.maxHP = (int)(enemy.currentHP * hpWeight_hard);
            //공격력증가
            enemy.power = (int)(enemy.currentPower * powerWeight_hard);
            //방어력
            enemy.defense = (int)(enemy.currentDefense * defenseWeight_hard);
            //속도
            enemy.speed = (int)(enemy.currentSpeed * speedWeight_hard);
            //보상
            enemy.rewardWoodResource = (int)(enemy.rewardWoodResource * rewardWeight_hard);
        }*/
    }

    /// <summary>
    /// 리스트에서 삭제될 enemy를 제외하고 리스트를 재구성
    /// </summary>
    /// <param name="removeEnemyIndex">재구성시 제거할 gameObject</param>
    public void ReorganizationEnemiesList(int removeEnemyIndex)
    {
        List<GameObject> tempEnemies = new List<GameObject>();
        int index = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            //제거할 gameObject면 제외
            if (i != removeEnemyIndex)
            {
                //enemies[i]가 null이면 제외
                if (enemies[i])
                {
                    //리스트 재구성
                    tempEnemies.Add(enemies[i]);
                    //enemyIndex번호 초기화
                    enemies[i].GetComponent<Enemy>().enemyIndex = index;

                    index++;
                }
            }else
                enemies[i].GetComponent<Enemy>().enemyIndex = -1;
        }//end of for

        enemies = tempEnemies;
    }
}
