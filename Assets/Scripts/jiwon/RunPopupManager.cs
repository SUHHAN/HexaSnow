using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RunPopupManager : MonoBehaviour

{
    public GameObject runPopupPanel; // Run Story 팝업 패널
    public GameObject backgroundClickArea;
    public Button ContinueButton;

    void Start()
    {
        runPopupPanel.SetActive(false); // 시작 시 Run 팝업 비활성화

        GameData loadedData = DataManager.Instance.LoadGameData();
        ContinueButton.interactable = (loadedData != null);
    }

    // Run Story 팝업 표시
    public void ShowRunPopup()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        runPopupPanel.SetActive(true);
        backgroundClickArea.SetActive(true);
    }

    // Run Story 팝업 숨기기
    public void HideRunPopup()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        runPopupPanel.SetActive(false);
        backgroundClickArea.SetActive(false);
    }

    // 새로운 게임 시작 (기존 데이터 삭제 후 새로 시작)
    public void StartNewGame()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        DataManager.Instance.gameData = new GameData(); // 새로운 데이터로 초기화
        DataManager.Instance.SaveInitialGameData(); // 저장
        SceneManager.LoadScene("tutorial"); // 새 게임 씬으로 이동
        Debug.Log("새로하기 초기화-튜토리얼 씬으로 이동");
    }

    // 이어하기 (저장된 데이터 불러와 게임 시작)
    public void ContinueGame()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        // 이어하기 슬롯을 통해 씬과 데이터를 모두 불러옴
        var slot = DataManager.Instance.LoadSlotData();

        if (slot == null)
        {
            Debug.LogWarning("저장된 이어하기 슬롯이 없습니다.");
            // 필요하다면 새 게임으로 유도하는 UI 띄우기
        }
    }
}

