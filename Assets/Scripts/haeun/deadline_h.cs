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
    [SerializeField] private GameObject IngredientStoreGoldPanel;
    [SerializeField] private TextMeshProUGUI MyMoneyText;
    [SerializeField] private TextMeshProUGUI MinMoneyText;

    public int MyMoney = 5000;
    public int minMoney = 500;

    void Start()
    {
        // 기본적으로 두 패널 다 비활성화 상태
        GoStorePanel.SetActive(false);
        WhereStorePanel.SetActive(false);
        IngredientStoreGoldPanel.SetActive(false);

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

    public void ShowIngredientGoldPanel() {
        WhereStorePanel.SetActive(false);

        MyMoneyText.text = $"보유 골드 : {MyMoney} G";
        MinMoneyText.text = $"{minMoney} G를\n지불하시겠습니까?";

        IngredientStoreGoldPanel.SetActive(true);
    }

    public void changeIngredient() {

        if (MyMoney >= minMoney) {
            SceneManager.LoadScene("Ingredient");
            IngredientStoreGoldPanel.SetActive(false);
        } else{
            Debug.Log("돈이 부족합니다. 재료를 구매할 수 없습니다.");
            Debug.Log("다음 날로 넘어갑니다.");
            IngredientStoreGoldPanel.SetActive(false);
        }
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
