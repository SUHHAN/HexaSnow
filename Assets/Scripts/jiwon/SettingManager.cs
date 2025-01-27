using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("설정 패널")]
    public GameObject SettingParentPanel; //전체 설정 부모 창
    public GameObject AccountPanel;  // 계정 설정 패널
    public GameObject GameSettingsPanel; // 게임 설정 패널

    [Header("버튼")]
    public Button AccountButton;
    public Button GameSettingsButton;

    private void Start()
    {
        // 버튼 클릭 이벤트 등록
        AccountButton.onClick.AddListener(OpenAccountPanel);
        GameSettingsButton.onClick.AddListener(OpenGameSettingsPanel);

        // 계정 버튼을 앞쪽으로 보내기
        SetButtonToFront(AccountButton);
        // 게임 설정 버튼을 뒤로 보내기
        SetButtonToBack(GameSettingsButton);
        // 기본적으로 AccountPanel을 열도록 설정
        OpenAccountPanel();
    }

    // 설정 창 열기
    public void OpenSettings()
    {
        if (SettingParentPanel != null)
        {
            SettingParentPanel.SetActive(true);
            OpenAccountPanel(); // Open AccountPanel as the default
        }
        else
        {
            Debug.LogError("SettingParentPanel이 할당되지 않았습니다!");
        }
    }

    // 설정 창 닫기
    public void CloseSettings()
    {
        SettingParentPanel.SetActive(false);
    }

    // 계정 설정 패널 열기
    private void OpenAccountPanel()
    {
        AccountPanel.SetActive(true);
        GameSettingsPanel.SetActive(false);

        // 계정 버튼을 앞쪽으로 보내기
        SetButtonToFront(AccountButton);
        // 게임 설정 버튼을 뒤로 보내기
        SetButtonToBack(GameSettingsButton);
    }

    // 게임 설정 패널 열기
    private void OpenGameSettingsPanel()
    {
        AccountPanel.SetActive(false);
        GameSettingsPanel.SetActive(true);

        // 게임 설정 버튼을 앞쪽으로 보내기
        SetButtonToFront(GameSettingsButton);
        // 계정 버튼을 뒤로 보내기
        SetButtonToBack(AccountButton);
    }

    // 버튼을 앞쪽으로 보내는 함수
    private void SetButtonToFront(Button button)
    {
        // 버튼을 부모 객체 내에서 맨 뒤로 보내기
        button.transform.SetAsLastSibling();
    }

    // 버튼을 뒤쪽으로 보내는 함수
    private void SetButtonToBack(Button button)
    {
        // 버튼을 부모 객체 내에서 맨 앞으로 보내기
        button.transform.SetAsFirstSibling();
    }
}


