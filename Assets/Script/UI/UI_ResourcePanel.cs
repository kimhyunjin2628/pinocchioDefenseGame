using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResourcePanel : UI_Controller
{
    public string filePath;

    [SerializeField]
    Animator animator;

    [SerializeField]
    Sprite[] stageSprite;

    enum TextMeshProUGUIs
    {
        woodResourceText,
        StageNumText,
        StageStartText
    }

    enum Images
    {
        StageStartImage
    }

    enum GameObjects
    {
        StageStartPanel
    }

    /// <summary>
    /// enum에 열거된 이름으로 UI정보를 바인딩 : 김현진
    /// </summary>
    protected override void BindingUI()
    {
        base.BindingUI();

        Bind<TextMeshProUGUI>(typeof(TextMeshProUGUIs));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        //스테이지 정보 갱신
        if (SystemManager.Instance.GameFlowManager.stage == 0)
        {
            //튜토리얼에서는 비활성화
            animator.enabled = false;

            //패널 비활성화
            GetGameobject((int)GameObjects.StageStartPanel).SetActive(false);
        }
        else
        {
            GetTextMeshProUGUI((int)TextMeshProUGUIs.StageNumText).text = "Stage " +
           SystemManager.Instance.GameFlowManager.stage.ToString();

            GetTextMeshProUGUI((int)TextMeshProUGUIs.StageStartText).text = "Stage " +
               SystemManager.Instance.GameFlowManager.stage.ToString();

            //스테이지 이미지 변경
            if (SystemManager.Instance.UserInfo.selectedStageNum <= 10)
                    GetImage((int)Images.StageStartImage).sprite = stageSprite[0]; //브론즈
            else if(SystemManager.Instance.UserInfo.selectedStageNum <= 20)
                    GetImage((int)Images.StageStartImage).sprite = stageSprite[1]; //실버
            else if (SystemManager.Instance.UserInfo.selectedStageNum <= 25)
                GetImage((int)Images.StageStartImage).sprite = stageSprite[2]; //골드
            else if (SystemManager.Instance.UserInfo.selectedStageNum <= 30)
                GetImage((int)Images.StageStartImage).sprite = stageSprite[3]; //플래티넘
            else if (SystemManager.Instance.UserInfo.selectedStageNum <= 35)
                GetImage((int)Images.StageStartImage).sprite = stageSprite[4]; //다이아
            else 
                GetImage((int)Images.StageStartImage).sprite = stageSprite[5]; //마스터


            //디펜스 시작 코루틴 호출
            StartCoroutine("StartDefense");
        }
    }

    /// <summary>
    /// 현재 나무 자원값을 받아와 나무 자원을 표시해주는 UI를 갱신
    /// </summary>
    public void UpdateWoodResource()
    {
        //현재 나무 자원을 받아와 텍스트값을 변경
        GetTextMeshProUGUI((int)TextMeshProUGUIs.woodResourceText).text = SystemManager.Instance.ResourceManager.woodResource.ToString();
        SystemManager.Instance.PanelManager.turretMngPanel.UpdateWoodResource();
    }

    /// <summary>
    /// 시작상태에서 디펜스 상태로 변경 : 김현진
    /// </summary>
    IEnumerator StartDefense()
    {
        yield return new WaitForSeconds(2.0f);
        //디펜스 시작
        SystemManager.Instance.GameFlowManager.gameState = GameFlowManager.GameState.Defense;
        //UI활성화
        SystemManager.Instance.PanelManager.EnableFixedPanel(2);
        SystemManager.Instance.PanelManager.optionPopUpPanel.EnablePediaButton();
        //나무 자원 UI 초기화
        UpdateWoodResource();
    }

}
