using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class deadline_h : MonoBehaviour
{
    // 마감 시, 상점 이동 확인 패널
    [SerializeField] private GameObject GoStorePanel;
    [SerializeField] private GameObject WhereStorePanel;

    void Start()
    {
        // 기본적으로 두 패널 다 비활성화 상태
        GoStorePanel.SetActive(false);
        WhereStorePanel.SetActive(false);

        StartCoroutine(ActivateGoStorePanelAfterDelay());
    }

    void Update()
    {
        
    }

    public void yesButton() {
        GoStorePanel.SetActive(false);
        WhereStorePanel.SetActive(true);
    }

    public void noButton() {
        GoStorePanel.SetActive(false);
    }

    public void changeIngredient() {
        SceneManager.LoadScene("Ingredient");
    }

    public void changeInterior() {
        SceneManager.LoadScene("Interior");
    }

    private IEnumerator ActivateGoStorePanelAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 2초 대기
        GoStorePanel.SetActive(true);       // 패널 활성화
    }

}
