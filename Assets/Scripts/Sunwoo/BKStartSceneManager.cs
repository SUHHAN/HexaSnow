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
        // Main 씬을 Additive로 불러오기 (배경용)
        if (!SceneManager.GetSceneByName("Main").isLoaded)
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        }

        if (startButton != null)
            startButton.onClick.AddListener(LoadBakingScene);

        if (specialButton != null)
        {
            specialButton.onClick.AddListener(LoadSpecialBakingScene);
            specialButton.interactable = false; // 기본값: 비활성화
        }

        // 돈 불러오기 (날짜 포함 가능성 있음)
        if (UiLogicManager.Instance != null)
        {
            UiLogicManager.Instance.LoadMoneyData();
        }

        // 날짜 로딩 후 버튼 활성화 체크 (0.2초 대기)
        StartCoroutine(DelayedCheckSpecialButtonAvailability());
    }

    private IEnumerator DelayedCheckSpecialButtonAvailability()
    {
        yield return new WaitForSeconds(0.2f); // 데이터 로딩 대기
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
        Debug.Log($"[BKStartSceneManager] 현재 날짜: {currentDate}");

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
        if (Application.CanStreamedLevelBeLoaded("BakingSp"))
        {
            SceneManager.LoadScene("BakingSp");
        }
        else
        {
            Debug.LogError("Scene 'BakingSp'가 Build Settings에 등록되지 않았습니다. File -> Build Settings -> Scenes In Build에서 추가하세요.");
        }
    }
}
