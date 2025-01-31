using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class deadlineLast_h : MonoBehaviour
{
    // 마감 시, 상점 이동 확인 패널
    [Header("경고문 관리 패널 관리")]
    [SerializeField] private GameObject DeadlinePanel;
    [SerializeField] private TextMeshProUGUI DeadlindText;
    [SerializeField] private GameObject __Panel;


    [Header("기타 관리")]
    [SerializeField] private GameObject BlackPanel;
    private string mydate = "0";
    [SerializeField] private GameData GD = new GameData();

    void Start()
    {
        // 기본적으로 패널 및 불투명 블랙 다 비활성화 상태
        BlackPanel.SetActive(false);

        DeadlinePanel.SetActive(false);
        __Panel.SetActive(false);

        StartCoroutine(ActivateDeadlinePanelAfterDelay());
    }

    void Update()
    {
        
    }

    public void yesButton() {
        DeadlinePanel.SetActive(false);
        BlackPanel.SetActive(false);

        // 하루를 넘어가는 버튼이 나오도록 하기
    }


    void Show__Panel() {

        BlackPanel.SetActive(false);
    }

    

    private IEnumerator ActivateDeadlinePanelAfterDelay()
    {
        DeadlindText.text = $"{mydate}일차 가게를 마감합니다.";

        yield return new WaitForSeconds(1f); // 1초 대기
        DeadlinePanel.SetActive(true);       // 패널 활성화
        BlackPanel.SetActive(true);
    }

    private void LoadDate() {

        GD = DataManager.Instance.LoadGameData();

        // !! 일차 업데이트하기
        // mydate = GD.lastLoginDate;
    }

}
