using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    [Header("알림 설정")]
    public Toggle notificationToggle;
    public Text notificationStatusText;

    // 두 개의 스프라이트 설정 (on과 off 상태)
    public Sprite notificationOnImage;  // 알림 켜졌을 때 이미지
    public Sprite notificationOffImage; // 알림 꺼졌을 때 이미지

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

        // 슬라이더 크기와 위치 설정
        SetSliderDimensions(systemVolumeSlider, "SystemVolume"); // 시스템 볼륨 슬라이더
        SetSliderDimensions(bgmVolumeSlider, "BGMVolume"); // BGM 볼륨 슬라이더
        SetSliderDimensions(effectVolumeSlider, "EffectVolume"); // 효과음 볼륨 슬라이더
    }

    // 알림 설정
    private void SetNotification(bool isOn)
    {
        Debug.Log("Toggle Changed: " + isOn); // 이벤트가 호출되는지 로그로 확인
        PlayerPrefs.SetInt("NotificationAllowed", isOn ? 1 : 0);
        UpdateNotificationUI();
    }

    private void UpdateNotificationUI()
    {
        bool isAllowed = PlayerPrefs.GetInt("NotificationAllowed", 1) == 1;
        notificationToggle.isOn = isAllowed;
        notificationStatusText.text = isAllowed ? "허용됨" : "비허용됨";

        // Toggle 상태에 따라 이미지 변경
        UpdateNotificationImage(isAllowed);
    }

    private void UpdateNotificationImage(bool isOn)
    {
        // Toggle의 이미지 컴포넌트 가져오기
        Image toggleImage = notificationToggle.GetComponent<Image>();

        // Toggle 상태에 따라 이미지 변경
        if (isOn)
        {
            toggleImage.sprite = notificationOnImage; // 켜졌을 때 이미지
        }
        else
        {
            toggleImage.sprite = notificationOffImage; // 꺼졌을 때 이미지
        }
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

    // 슬라이더 크기와 위치 설정 함수
    private void SetSliderDimensions(Slider slider, string keyPrefix)
    {
        RectTransform rect = slider.GetComponent<RectTransform>();

        // **슬라이더 크기와 위치 초기화**
        // PlayerPrefs에서 저장된 크기와 위치 값을 불러옴 (저장된 값이 없으면 기본값 사용)
        float width = PlayerPrefs.GetFloat(keyPrefix + "_Width", 29f);         // 기본값: 29f (너비)
        float bottom = PlayerPrefs.GetFloat(keyPrefix + "_Bottom", -6.5f);      // 기본값: -6.5f (높이)
        float posX = PlayerPrefs.GetFloat(keyPrefix + "_PosX", -5f);          // 기본값: 135f (X 위치)
        float top = PlayerPrefs.GetFloat(keyPrefix + "_Top", -6.5f);            // 기본값: -6.5f (Y 위치)

        // **슬라이더 크기와 위치 적용**
        rect.sizeDelta = new Vector2(width, bottom); // 크기 설정: 너비와 높이
        rect.anchoredPosition = new Vector2(posX, top); // 위치 설정: X, Y 좌표

        // 주석: 위 코드는 슬라이더의 크기와 위치를 저장된 값으로 설정합니다.
    }

    // 슬라이더 크기와 위치 저장 함수
    public void SaveSliderDimensions(Slider slider, string keyPrefix)
    {
        RectTransform rect = slider.GetComponent<RectTransform>();

        // **슬라이더 크기와 위치 저장**
        PlayerPrefs.SetFloat(keyPrefix + "_Width", rect.sizeDelta.x);   // 크기 저장: 너비
        PlayerPrefs.SetFloat(keyPrefix + "_Bottom", rect.sizeDelta.y);  // 크기 저장: 높이
        PlayerPrefs.SetFloat(keyPrefix + "_PosX", rect.anchoredPosition.x); // 위치 저장: X 좌표
        PlayerPrefs.SetFloat(keyPrefix + "_Top", rect.anchoredPosition.y); // 위치 저장: Y 좌표

        // 주석: 위 코드는 슬라이더의 크기와 위치를 PlayerPrefs에 저장합니다.
        PlayerPrefs.Save(); // 저장을 확실하게 반영
    }
}

