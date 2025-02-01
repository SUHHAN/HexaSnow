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
    [SerializeField] private GameData GD = new GameData();


    private int MyMoney;
    private int minMoney = 500;

    void Start()
    {

        // 돈 관리
        LoadMoneyData();

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
        AudioManager.Instance.PlaySys(AudioManager.Sys.button);
        GoStorePanel.SetActive(false);
        WhereStorePanel.SetActive(true);
    }

    public void noButton() {
        AudioManager.Instance.PlaySys(AudioManager.Sys.button);
        GoStorePanel.SetActive(false);
        // 만약에, no 버튼을 누르면 재료 상점을 이용하지 않는 걸로 하기
    }

    public void ShowIngredientGoldPanel() {
        WhereStorePanel.SetActive(false);

        MyMoneyText.text = $"보유 골드 : {MyMoney} G";
        MinMoneyText.text = $"{minMoney} G를\n지불하시겠습니까?";

        IngredientStoreGoldPanel.SetActive(true);
    }

    // public void changeIngredient() {

    //     if (MyMoney >= minMoney) {

    //         MyMoney = MyMoney - minMoney;
    //         // 여기서 나의 머니 다시 저장하기

    //         SceneManager.LoadScene("Ingredient");
    //         IngredientStoreGoldPanel.SetActive(false);
    //     } else{
    //         Debug.Log("돈이 부족합니다. 재료를 구매할 수 없습니다.");

    //         IngredientStoreGoldPanel.SetActive(false);

    //         // 여기에 경고창 넣는걸로 하기
    //         NoMoneyPanel.SetActive(true);
    //         Invoke("ShowNoMoneyPanel", 3f); // 경고창을 3초 뒤에 끄도록
    //     }
    // }

    public void YesIngredientStore() {
        AudioManager.Instance.PlaySys(AudioManager.Sys.button);
        if (MyMoney >= minMoney) {

            MyMoney = MyMoney - minMoney;
            // 여기서 나의 머니 다시 저장하기
            SaveMoneyData();

            SceneManager.LoadScene("Ingredient");
            IngredientStoreGoldPanel.SetActive(false);
        } else{
            Debug.Log("돈이 부족합니다. 재료를 구매할 수 없습니다.");

            IngredientStoreGoldPanel.SetActive(false);

            // 여기에 경고창 넣는걸로 하기
            NoMoneyPanel.SetActive(true);
            Invoke("ShowNoMoneyPanel", 3f); // 경고창을 3초 뒤에 끄도록

            // 경고창 끈다음에 하루가 넘어가도록 하는 씬 추가
            SceneManager.LoadScene("Deadline_Last");
        }
    }

    public void NoIngredientStore() {
        AudioManager.Instance.PlaySys(AudioManager.Sys.button);
        IngredientStoreGoldPanel.SetActive(false);

        // 경고창 끈다음에 하루가 넘어가도록 하는 씬 추가
        SceneManager.LoadScene("Deadline_Last");
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

    private void LoadMoneyData()
    {
        if (DataManager.Instance != null)
        {   
            // 저장되어 있던 재료 리스트 로드
            GD = DataManager.Instance.LoadGameData();
        
            // 저장된 돈 가지고 오기
            MyMoney = GD.money; //money에는 지원 언니가 정한 돈 관리 변수로 쓰기
        }
        else
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    private void SaveMoneyData() {
        if (DataManager.Instance != null)
        {   
            // 돈을 저장한 뒤에 넘기기
            DataManager.Instance.gameData.money = MyMoney;
            DataManager.Instance.SaveGameData(); // 저장 함수 호출
            Debug.Log("GameData에 money 저장 완료!");
        }
        else
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다.");
        }
    }

}
