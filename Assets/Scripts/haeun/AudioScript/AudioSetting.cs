using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AudioSettings : MonoBehaviour 
{
    public Slider bgmSlider; // 배경음 슬라이더
    public Slider sfxSlider; // 효과음 슬라이더

    void Start()
    {
        // 씬에서 슬라이더 자동 검색
        if (bgmSlider == null || sfxSlider == null)
        {
            FindAndAssignSliders();
        }

        if (bgmSlider == null || sfxSlider == null)
        {
            Debug.LogError("슬라이더를 찾을 수 없습니다.");
            return;
        }

        // 슬라이더 초기값 설정
        bgmSlider.value = AudioManager.Instance.bgmVolume;
        sfxSlider.value = AudioManager.Instance.sfxVolume;

        // 슬라이더의 OnValueChanged 이벤트에 메서드 연결
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    void FindAndAssignSliders()
    {
        // 모든 씬의 활성화된 슬라이더를 찾음
        Slider[] allSliders = FindObjectsOfType<Slider>(true);

        foreach (Slider slider in allSliders)
        {
            if (slider.name.Contains("BGM")) 
            {
                bgmSlider = slider;
            }
            else if (slider.name.Contains("Effect")) 
            {
                sfxSlider = slider;
            }
        }
    }

    public void SetBgmVolume(float volume)
    {
        AudioManager.Instance.SetBgmVolume(volume);
    }

    public void SetSfxVolume(float volume)
    {
        AudioManager.Instance.SetSfxVolume(volume);
    }
}
