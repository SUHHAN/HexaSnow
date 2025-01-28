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
    [SerializeField] private GameObject __Panel;

    // [Header("돈 관련 텍스트 관리")]
    // [SerializeField] private TextMeshProUGUI MyMoneyText;
    // [SerializeField] private TextMeshProUGUI MinMoneyText;

    [Header("기타 관리")]
    [SerializeField] private GameObject BlackPanel;

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
        // GoStorePanel.SetActive(false);
        // WhereStorePanel.SetActive(true);
    }

    public void noButton() {
        // GoStorePanel.SetActive(false);
    }

    void Show__Panel() {


        BlackPanel.SetActive(false);
    }

    // 넘어갈 씬으로 바꾸기 - Interior를
    public void changeNext() {
        SceneManager.LoadScene("Interior");
    }

    private IEnumerator ActivateDeadlinePanelAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        DeadlinePanel.SetActive(true);       // 패널 활성화
        BlackPanel.SetActive(true);
    }

}
