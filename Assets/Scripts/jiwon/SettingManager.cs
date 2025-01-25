using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("설정 패널")]
    public GameObject settingsPanel; // 전체 설정 창
    public GameObject accountPanel;  // 계정 설정 패널
    public GameObject gameSettingsPanel; // 게임 설정 패널

    [Header("버튼")]
    public Button accountButton;
    public Button gameSettingsButton;

    private void Start()
    {
        // 버튼 클릭 이벤트 등록
        accountButton.onClick.AddListener(OpenAccountPanel);
        gameSettingsButton.onClick.AddListener(OpenGameSettingsPanel);

        // 기본적으로 AccountPanel이 열려 있도록 설정
        OpenAccountPanel();
    }

    // 설정 창 열기
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        OpenAccountPanel(); // 기본값으로 계정 패널 활성화
    }

    // 설정 창 닫기
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // 계정 패널 열기
    private void OpenAccountPanel()
    {
        accountPanel.SetActive(true);
        gameSettingsPanel.SetActive(false);
    }

    // 게임 설정 패널 열기
    private void OpenGameSettingsPanel()
    {
        accountPanel.SetActive(false);
        gameSettingsPanel.SetActive(true);
    }
}

