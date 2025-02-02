using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    [Header("알림 설정")]
    public Toggle notificationToggle;

    // 두 개의 스프라이트 설정 (on과 off 상태)
    public Sprite notificationOnImage;  // 알림 켜졌을 때 이미지
    public Sprite notificationOffImage; // 알림 꺼졌을 때 이미지

    [Header("볼륨 설정")]
    public Slider bgmVolumeSlider;
    public Slider effectVolumeSlider;

    [Header("언어 설정")]
    public Button koreanButton;
    // public Text preparingText;

    private void Start()
    {
        // **저장된 알림 설정 불러오기** (기본값: 1 = 허용)
        bool isAllowed = PlayerPrefs.GetInt("NotificationAllowed", 1) == 1;
        notificationToggle.isOn = isAllowed;
        UpdateNotificationUI();

        // **토글 이벤트 리스너 등록**
        notificationToggle.onValueChanged.AddListener(SetNotification);

        // **저장된 볼륨 값 불러오기** (저장된 값이 없으면 기본값 50 사용)
        // systemVolumeSlider.value = PlayerPrefs.GetFloat("SystemVolume", 50);
        // bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 50);
        // effectVolumeSlider.value = PlayerPrefs.GetFloat("EffectVolume", 50);

        bgmVolumeSlider.value = AudioManager.Instance.bgmVolume;
        effectVolumeSlider.value = AudioManager.Instance.sfxVolume;
        // effectVolumeSlider.value = AudioManager.Instance.sysVolume;

        // **볼륨 조절 슬라이더 이벤트 리스너 등록**
        // systemVolumeSlider.onValueChanged.AddListener(delegate { SetSystemVolume(); });
        // bgmVolumeSlider.onValueChanged.AddListener(delegate { SetBGMVolume(); });
        // effectVolumeSlider.onValueChanged.AddListener(delegate { SetEffectVolume(); });

        // systemVolumeSlider.onValueChanged.AddListener(SetSystemVolume);
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        effectVolumeSlider.onValueChanged.AddListener(SetEffectVolume);

        // **UI 업데이트**
        UpdateVolumeUI();

        // **언어 선택 버튼 설정**
        koreanButton.onClick.AddListener(SelectKorean);
        // preparingText.color = new Color(0.5f, 0.5f, 0.5f, 1F); // 회색 처리

        // **슬라이더 크기 및 위치 설정 (선택 사항)**
        // SetSliderDimensions(systemVolumeSlider, "SystemVolume");
        SetSliderDimensions(bgmVolumeSlider, "BGMVolume");
        SetSliderDimensions(effectVolumeSlider, "EffectVolume");
    }

    // 알림 설정
    private void SetNotification(bool isOn)
    {
        Debug.Log("Toggle Changed: " + isOn); // 이벤트가 호출되는지 로그로 확인
        PlayerPrefs.SetInt("NotificationAllowed", isOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateNotificationUI();
    }

    private void UpdateNotificationUI()
    {
        bool isAllowed = PlayerPrefs.GetInt("NotificationAllowed", 1) == 1;
        notificationToggle.isOn = isAllowed;

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
        // systemVolumeText.text = ((int)systemVolumeSlider.value).ToString();
        // bgmVolumeText.text = ((int)bgmVolumeSlider.value).ToString();
        // effectVolumeText.text = ((int)effectVolumeSlider.value).ToString();
    }

    // public void SetSystemVolume(float volume)
    // {
    //     // AudioListener.volume = systemVolumeSlider.value / 100f;
    //     AudioManager.Instance.SetSystemVolume(volume);

    // }

    public void SetBGMVolume(float volume)
    {
        // BGM 오디오 소스 볼륨 조절
        AudioManager.Instance.SetBgmVolume(volume);
    }

    public void SetEffectVolume(float volume)
    {
        // 효과음 오디오 소스 볼륨 조절
        AudioManager.Instance.SetSfxVolume(volume);

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