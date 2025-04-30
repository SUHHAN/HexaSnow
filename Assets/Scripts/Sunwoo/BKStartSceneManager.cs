using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BKStartSceneManager : MonoBehaviour
{
    public Button startButton; // 일반 버튼
    public Button specialButton; // 특별손님 버튼
    public GameObject StartPanel;

    private int currentDate;

    void Start()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);

        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadBakingScene);
        }

        if (specialButton != null)
        {
            specialButton.onClick.AddListener(LoadSpecialBakingScene);
            specialButton.interactable = false; // 기본적으로 비활성화
        }

        UiLogicManager.Instance.LoadMoneyData();
        CheckSpecialButtonAvailability();
    }

    private void CheckSpecialButtonAvailability()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameData == null)
        {
            Debug.LogError("DataManager 또는 GameData를 찾을 수 없습니다!");
            return;
        }

        GameData dateGD = DataManager.Instance.LoadGameData();
        currentDate = dateGD.date;
        Debug.Log($"현재 날짜: {currentDate}");

        // 2일차부터 specialButton 활성화
        if (specialButton != null)
        {
            if (currentDate >= 2)
            {
                specialButton.interactable = true;
                Debug.Log("특별손님 버튼 활성화됨 (2일차 이상)");
            }
            else
            {
                specialButton.interactable = false;
                Debug.Log("특별손님 버튼 비활성화됨 (1일차)");
            }
        }
    }

    public void LoadBakingScene()
    {
        SceneManager.LoadScene("Baking 1");
    }

    public void LoadSpecialBakingScene()
    {
        SceneManager.LoadScene("BakingSp");
    }
}
