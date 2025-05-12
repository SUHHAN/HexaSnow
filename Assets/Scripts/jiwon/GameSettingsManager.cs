using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{

    [Header("볼륨 설정")]
    public Slider bgmVolumeSlider;
    public Slider effectVolumeSlider;

    [Header("언어 설정")]
    public Button koreanButton;
    // public Text preparingText;

    private void Start()
    {

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

        if (PlayerPrefs.HasKey(keyPrefix + "_Width") &&
            PlayerPrefs.HasKey(keyPrefix + "_Height") &&
            PlayerPrefs.HasKey(keyPrefix + "_PosX") &&
            PlayerPrefs.HasKey(keyPrefix + "_PosY"))
        {
            float width = PlayerPrefs.GetFloat(keyPrefix + "_Width");
            float height = PlayerPrefs.GetFloat(keyPrefix + "_Height");
            float posX = PlayerPrefs.GetFloat(keyPrefix + "_PosX");
            float posY = PlayerPrefs.GetFloat(keyPrefix + "_PosY");

            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(posX, posY);
        }
        // else: 저장된 값 없으면 에디터 값 그대로 유지
    }

    // 슬라이더 크기와 위치 저장 함수
    public void SaveSliderDimensions(Slider slider, string keyPrefix)
    {
        RectTransform rect = slider.GetComponent<RectTransform>();

        PlayerPrefs.SetFloat(keyPrefix + "_Width", rect.sizeDelta.x);
        PlayerPrefs.SetFloat(keyPrefix + "_Height", rect.sizeDelta.y);
        PlayerPrefs.SetFloat(keyPrefix + "_PosX", rect.anchoredPosition.x);
        PlayerPrefs.SetFloat(keyPrefix + "_PosY", rect.anchoredPosition.y);
        PlayerPrefs.Save();
    }
}