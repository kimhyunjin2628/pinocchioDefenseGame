using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //Load한 Enemy 프리팹 정보
    Dictionary<string, GameObject> prefabCaChes = new Dictionary<string, GameObject>();

    [SerializeField]
    Transform enemyParents;

    //filePath, cacheCount 저장
    [SerializeField]
    PrefabCacheData[] prefabCacheDatas;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            SystemManager.Instance.PrefabCacheSystem.EnablePrefabCache(prefabCacheDatas[0].filePath);
    }

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


    public void EnableEnemy(int enemyIndex, int gateNum, int[] targetTile)
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

        //생성한 프리팹 객체 변수 초기화
        enemy.gateNum = gateNum;
        enemy.targetTile = SystemManager.Instance.TileManager.CreateTileMapArr(targetTile);

        //적을 초기상태로
        enemy.Reset();
    }

}
