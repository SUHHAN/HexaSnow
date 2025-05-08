using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BKStartSceneManager : MonoBehaviour
{
    public Button startButton;       // 일반 베이킹 버튼
    public Button specialButton;     // 특별손님 버튼
    public GameObject StartPanel;
    public TextMeshProUGUI closingText; // 마감 텍스트
    public Image closingImage;

    private int currentDate;

    void Start()
    {
        if (!SceneManager.GetSceneByName("Main").isLoaded)
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        }

        if (startButton != null)
            startButton.onClick.AddListener(LoadBakingScene);

        if (specialButton != null)
        {
            specialButton.onClick.AddListener(LoadSpecialBakingScene);
            specialButton.interactable = false;
        }

        if (UiLogicManager.Instance != null)
        {
            UiLogicManager.Instance.LoadMoneyData();
        }

        if (closingText != null)
            closingText.gameObject.SetActive(false); // 기본은 비활성화

        if (closingImage != null)
            closingImage.gameObject.SetActive(false); // 기본은 비활성화

        StartCoroutine(DelayedCheckSpecialButtonAvailability());
    }

    private IEnumerator DelayedCheckSpecialButtonAvailability()
    {
        yield return new WaitForSeconds(0.2f);
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
        float time = dateGD.time;

        int hours = (int)time;
        int minutes = Mathf.FloorToInt((time - hours) * 60);

        Debug.Log($"[BKStartSceneManager] 현재 날짜: {currentDate}, 현재 시간: {hours:D2}:{minutes:D2}");

        if (hours >= 16)
        {
            if (startButton != null) startButton.interactable = false;
            if (specialButton != null) specialButton.interactable = false;

            if (closingText != null)
            {
                closingText.gameObject.SetActive(true);
                closingText.text = "이제 마감할 시간이야";
            }

            if (closingImage != null)
            {
                closingImage.gameObject.SetActive(true);
            }

            Debug.Log("오후 4시 이후 → 마감 상태. 모든 버튼 비활성화");
            return;
        }

        if (closingText != null)
            closingText.gameObject.SetActive(false);

        if (closingImage != null)
            closingImage.gameObject.SetActive(false);

        if (specialButton != null)
        {
            specialButton.interactable = (currentDate >= 2);
            Debug.Log(specialButton.interactable
                ? "특별손님 버튼 활성화됨 (2일차 이상)"
                : "특별손님 버튼 비활성화됨 (1일차)");
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
