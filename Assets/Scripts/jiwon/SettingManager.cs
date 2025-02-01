using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("설정 패널")]
    public GameObject SettingParentPanel; // 전체 설정 부모 창
    public GameObject AccountPanel;  // 계정 설정 패널
    public GameObject GameSettingsPanel; // 게임 설정 패널
    public GameObject SavePanel; // 저장 설정 패널 (새로 추가)

    [Header("버튼")]
    public Button AccountButton;
    public Button GameSettingsButton;
    public Button SaveButton; // 새 저장 버튼

    private void Start()
    {
        // 버튼 클릭 이벤트 등록
        AccountButton.onClick.AddListener(OpenAccountPanel);
        GameSettingsButton.onClick.AddListener(OpenGameSettingsPanel);
        SaveButton.onClick.AddListener(OpenSavePanel); // 저장 버튼 클릭 등록

        // 계정 버튼을 앞쪽으로 보내기
        SetButtonToFront(AccountButton);
        // 나머지 버튼을 뒤로 보내기
        SetButtonToBack(GameSettingsButton);
        SetButtonToBack(SaveButton);
        AccountPanel.SetActive(true);
        GameSettingsPanel.SetActive(false);
        SavePanel.SetActive(false); // 저장 패널 열기
    }

    // 설정 창 열기
    public void OpenSettings()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        if (SettingParentPanel != null)
        {
            SettingParentPanel.SetActive(true);
            OpenAccountPanel(); // 기본적으로 AccountPanel 열기
        }
        else
        {
            Debug.LogError("SettingParentPanel이 할당되지 않았습니다!");
        }
    }


    // 설정 창 닫기
    public void CloseSettings()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        if (SettingParentPanel != null)
        {
            Debug.Log("설정 창 닫기 실행됨.");
            SettingParentPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("SettingParentPanel이 null입니다! Inspector에서 올바른 오브젝트를 할당하세요.");
        }
    }

    // 계정 설정 패널 열기
    private void OpenAccountPanel()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        AccountPanel.SetActive(true);
        GameSettingsPanel.SetActive(false);
        SavePanel.SetActive(false); // AccountPanel이 열릴 때 SavePanel은 닫기

        // 계정 버튼을 앞쪽으로 보내기
        SetButtonToFront(AccountButton);
        // 나머지 버튼을 뒤로 보내기
        SetButtonToBack(GameSettingsButton);
        SetButtonToBack(SaveButton);
    }

    // 게임 설정 패널 열기
    private void OpenGameSettingsPanel()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        AccountPanel.SetActive(false);
        GameSettingsPanel.SetActive(true);
        SavePanel.SetActive(false); // GameSettingsPanel이 열릴 때 SavePanel은 닫기

        // 게임 설정 버튼을 앞쪽으로 보내기
        SetButtonToFront(GameSettingsButton);
        // 나머지 버튼을 뒤로 보내기
        SetButtonToBack(AccountButton);
        SetButtonToBack(SaveButton);
    }

    // 저장 패널 열기 (새로 추가된 기능)
    private void OpenSavePanel()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        AccountPanel.SetActive(false);
        GameSettingsPanel.SetActive(false);
        SavePanel.SetActive(true); // 저장 패널 열기

        // 저장 버튼을 앞쪽으로 보내기
        SetButtonToFront(SaveButton);
        // 다른 버튼들을 뒤로 보내기
        SetButtonToBack(AccountButton);
        SetButtonToBack(GameSettingsButton);
    }

    // 버튼을 앞쪽으로 보내기
    private void SetButtonToFront(Button button)
    {
        button.transform.SetAsLastSibling();
    }

    // 버튼을 뒤로 보내기
    private void SetButtonToBack(Button button)
    {
        button.transform.SetAsFirstSibling();
    }
}



