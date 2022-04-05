using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefenseFlowData
{
    public int[] targetTileIndexArr;
    public int[] timeFlowIndexArr;
    public int[] enemyFlowIndexArr;
}
[System.Serializable]
public class DefenseFlowDataArr
{
    public int stageNum;
    public DefenseFlowData[] defenseFlowDataArr;
}
[System.Serializable]
public class DefenseFlowDataList
{
    public List<DefenseFlowDataArr> datas;
}

public class GameFlowManager : MonoBehaviour
{
    //게이트 숫자
    const int GATENUM = 3;
    
    enum GameState
    {
        Start,   //게임시작
        Defense, //
        End    //이동X, 비활성화 처리
    }
    [SerializeField]
    GameState gameState = GameState.Start;

    //배열 포인터
    int[] arrPointer = new int[GATENUM];

    //타이머
    float[] flowTimer = new float[GATENUM];


    //디펜스 페이지 흐름 관련 Data배열 
    public DefenseFlowData[] defenseFlowDatas;

    //디펜스 페이지 흐름 관련 Data 배열 리스트
    public List<DefenseFlowDataArr> defenseFlowDataList;

    // Start is called before the first frame update
    void Start()
    {
        //배열 포인터 초기화
        for (int i = 0; i < GATENUM; i++)
        {
            arrPointer[i] = 0;
            flowTimer[i] = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGame();
    }

    /// <summary>
    /// 실시간 게임 진행 상태 별 동작 : 김현진
    /// </summary>
    void UpdateGame()
    {
        switch (gameState)
        {
            case GameState.Start:
                break;
            case GameState.Defense:
                UpdateDefense();
                break;
            case GameState.End:
                break;
        }
    }

    /// <summary>
    /// 일정 시간마다 Enemy를 활성해준다 : 김현진
    /// </summary>
    void UpdateDefense()
    {
        //Gate 1~3
        for (int i = 0; i < GATENUM; i++)
        {
            if (Time.time - flowTimer[i] > defenseFlowDatas[i].timeFlowIndexArr[arrPointer[i]])
            {
                //Enemy 활성화
                SystemManager.Instance.EnemyManager.EnableEnemy(defenseFlowDatas[i].enemyFlowIndexArr[arrPointer[i]], i, defenseFlowDatas[i].targetTileIndexArr);

                //마지막 인덱스
                if (arrPointer[i] >= defenseFlowDatas[i].enemyFlowIndexArr.Length - 1)
                {
                    gameState = GameState.End;
                }
                else
                {
                    //배열 포인터 증가
                    arrPointer[i]++;

                    //타이머 초기화
                    flowTimer[i] = Time.time;
                }
            }

        }   

    }
}
