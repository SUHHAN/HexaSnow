using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class deadline_h : MonoBehaviour
{
    // 마감 시, 상점 이동 확인 패널
    [Header("경고문 관리 패널 관리")]
    [SerializeField] private GameObject GoStorePanel;
    [SerializeField] private GameObject WhereStorePanel;
    [SerializeField] private GameObject IngredientStoreGoldPanel;
    [SerializeField] private GameObject NoMoneyPanel;

    [Header("돈 관련 텍스트 관리")]
    [SerializeField] private TextMeshProUGUI MyMoneyText;
    [SerializeField] private TextMeshProUGUI MinMoneyText;

    [Header("기타 관리")]
    [SerializeField] private GameObject BlackPanel;


    public int MyMoney = 400;
    private int minMoney = 500;

    void Start()
    {
        // 기본적으로 패널 및 불투명 블랙 다 비활성화 상태
        BlackPanel.SetActive(false);

        GoStorePanel.SetActive(false);
        WhereStorePanel.SetActive(false);
        IngredientStoreGoldPanel.SetActive(false);
        NoMoneyPanel.SetActive(false);

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

            MyMoney = MyMoney - minMoney;
            // 여기서 나의 머니 다시 저장하기

            SceneManager.LoadScene("Ingredient");
            IngredientStoreGoldPanel.SetActive(false);
        } else{
            Debug.Log("돈이 부족합니다. 재료를 구매할 수 없습니다.");

            IngredientStoreGoldPanel.SetActive(false);

            // 여기에 경고창 넣는걸로 하기
            NoMoneyPanel.SetActive(true);
            Invoke("ShowNoMoneyPanel", 3f); // 경고창을 3초 뒤에 끄도록
        }
    }

    void ShowNoMoneyPanel() {
        NoMoneyPanel.SetActive(false);
        BlackPanel.SetActive(false);
    }

    public void changeInterior() {
        SceneManager.LoadScene("Interior");
    }

    private IEnumerator ActivateGoStorePanelAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        GoStorePanel.SetActive(true);       // 패널 활성화
        BlackPanel.SetActive(true);
    }

}
