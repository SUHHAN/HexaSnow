using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    [Header("알림 설정")]
    public Toggle notificationToggle;
    public Text notificationStatusText;

    [Header("볼륨 설정")]
    public Slider systemVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider effectVolumeSlider;

    public Text systemVolumeText;
    public Text bgmVolumeText;
    public Text effectVolumeText;

    [Header("언어 설정")]
    public Button koreanButton;
    public Text preparingText;

    private void Start()
    {
        // 알림 설정 초기화
        notificationToggle.onValueChanged.AddListener(SetNotification);
        UpdateNotificationUI();

        // 볼륨 초기화
        systemVolumeSlider.value = 50;
        bgmVolumeSlider.value = 50;
        effectVolumeSlider.value = 50;

        systemVolumeSlider.onValueChanged.AddListener(delegate { SetSystemVolume(); });
        bgmVolumeSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        effectVolumeSlider.onValueChanged.AddListener(delegate { SetEffectVolume(); });

        UpdateVolumeUI();

        // 언어 선택 버튼
        koreanButton.onClick.AddListener(SelectKorean);
        preparingText.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 회색 처리
    }

    // 알림 설정
    private void SetNotification(bool isOn)
    {
        PlayerPrefs.SetInt("NotificationAllowed", isOn ? 1 : 0);
        UpdateNotificationUI();
    }

    private void UpdateNotificationUI()
    {
        bool isAllowed = PlayerPrefs.GetInt("NotificationAllowed", 1) == 1;
        notificationToggle.isOn = isAllowed;
        notificationStatusText.text = isAllowed ? "허용됨" : "비허용됨";
    }

    // 볼륨 설정
    private void UpdateVolumeUI()
    {
        systemVolumeText.text = ((int)systemVolumeSlider.value).ToString();
        bgmVolumeText.text = ((int)bgmVolumeSlider.value).ToString();
        effectVolumeText.text = ((int)effectVolumeSlider.value).ToString();
    }

    public void SetSystemVolume()
    {
        AudioListener.volume = systemVolumeSlider.value / 100f;
        UpdateVolumeUI();
    }

    public void SetBGMVolume()
    {
        // BGM 오디오 소스 볼륨 조절
        UpdateVolumeUI();
    }

    public void SetEffectVolume()
    {
        // 효과음 오디오 소스 볼륨 조절
        UpdateVolumeUI();
    }

    // 언어 설정
    private void SelectKorean()
    {
        Debug.Log("한국어 선택됨");
    }
}
