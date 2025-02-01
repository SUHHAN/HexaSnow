using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    // 배경음 변수
    [Header("#BGM")]
    public AudioClip[] bgmClips;  // 여러 개의 BGM 리스트
    public float bgmVolume = 0.5f;
    AudioSource[] bgmPlayers;
    private int currentBgmIndex = -1;  // 현재 재생 중인 BGM 인덱스 (-1: 아무것도 재생 안 함)
    public enum Bgm {dead, inside, kitchen_minigame, main};



    // 효과음 변수
    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume = 0.5f;

    // 동시다발적으로 많은 효과음을 내기 위해서 channel 분리
    public int sfxChannels = 4;
    AudioSource[] sfxPlayers;
    int sfxChannelIndex;
    
    public enum Sfx {bouns_card, ingre_fail, oven_suss, oven_fail};


    // 시스템 변수
    [Header("#SYS")]
    public AudioClip[] sysClips;
    public float sysVolume = 0.5f;

    // 동시다발적으로 많은 효과음을 내기 위해서 channel 분리
    public int sysChannels = 3;
    AudioSource[] sysPlayers;
    int sysChannelIndex;
    
    public enum Sys {bell, button, recipe_order};

    void Awake() {
        // 싱글톤 인스턴스 생성
        if (Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(gameObject); // 씬이 전환되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 자신을 파괴
            return;
        }
    }

    void Init() {
        // 1. 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayers");
        bgmObject.transform.parent = transform;
        bgmPlayers = new AudioSource[bgmClips.Length];

        for (int i = 0; i < bgmClips.Length; i++)
        {
            bgmPlayers[i] = bgmObject.AddComponent<AudioSource>();
            bgmPlayers[i].playOnAwake = false;
            bgmPlayers[i].loop = true;
            bgmPlayers[i].volume = bgmVolume;
            bgmPlayers[i].clip = bgmClips[i]; // 각각의 BGM 할당
        }                    

        // 2. 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxChannels = 16;
        sfxPlayers = new AudioSource[sfxChannels];

            // component로 audioSource를 추가하면서 배열을 각각 형성
        for (int index = 0; index < sfxPlayers.Length; index++) {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>(); 
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }

        // 3. 환경음 플레이어 초기화
        GameObject envObject = new GameObject("EnvPlayer");
        envObject.transform.parent = transform;
        sysChannels = 16;
        sysPlayers = new AudioSource[sysChannels];

        for (int index = 0; index < sysPlayers.Length; index++) {
            sysPlayers[index] = envObject.AddComponent<AudioSource>(); // component로 audioSource를 추가하면서 배열을 각각 형성
            sysPlayers[index].playOnAwake = false;
            sysPlayers[index].volume = sysVolume;
        }

        // 3. 볼륨 로드 및 설정
        bgmVolume = PlayerPrefs.GetFloat("BgmVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("EnvVolume", 1f);
        // bgmPlayer.volume = bgmVolume;

        
    }

    public void SetBgmVolume(float volume) {
        bgmVolume = volume;
        foreach (var bgmPlayer in bgmPlayers) {
            bgmPlayer.volume = bgmVolume;
        }
        PlayerPrefs.SetFloat("BgmVolume", bgmVolume); // 볼륨 값 저장
    }

    public void SetSfxVolume(float volume) {
        sfxVolume = volume;
        foreach (var sfxPlayer in sfxPlayers) {
            sfxPlayer.volume = sfxVolume;
        }
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume); // 볼륨 값 저장
    }

    public void SetEnvVolume(float volume) {
        sysVolume = volume;
        foreach (var sysPlayer in sysPlayers) {
            sysPlayer.volume = sysVolume;
        }
        PlayerPrefs.SetFloat("EnvVolume", sysVolume); // 볼륨 값 저장
    }

    public void PlaySfx(Sfx sfx) {
        for (int index = 0; index < sfxPlayers.Length; index++) {
            int loopIndex = (index + sfxChannelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            continue;

            // // 랜덤으로 2가지 이상의 효과음을 선택하고 싶을 때
            int ranIndex = 0;
            // if (sfx == Sfx.Hit || sfx == Sfx.Hit) {
            //       ranIndex = Random.Range(0,2);
            // }

            sfxChannelIndex = loopIndex;
            sfxPlayers[sfxChannelIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[sfxChannelIndex].Play();

            break;
        }
    }

    public void PlayEnv(Sys sys) {
        for (int index = 0; index < sysPlayers.Length; index++) {
            int loopIndex = (index + sysChannelIndex) % sfxPlayers.Length;

            if (sysPlayers[loopIndex].isPlaying)
            continue;

            // 랜덤으로 2가지 이상의 효과음을 선택하고 싶을 때
            int ranIndex = 0;
            // if (sfx == Sfx.Hit || sfx == Sfx.Hit) {
            //       ranIndex = Random.Range(0,2);
            // }

            sysChannelIndex = loopIndex;
            sysPlayers[sysChannelIndex].clip = sfxClips[(int)sys + ranIndex];
            sysPlayers[sysChannelIndex].Play();

            break;
        }
    }

     // 🎵 특정 BGM을 선택하여 재생 (기존 BGM 정지 후 새 BGM 실행)
    public void PlayBgm(Bgm bgmType)
    {
        int index = (int)bgmType;
        
        if (index < 0 || index >= bgmPlayers.Length)
        {
            Debug.LogWarning("BGM 인덱스가 범위를 벗어났습니다.");
            return;
        }

        // 🎯 이미 해당 BGM이 재생 중이면 중복 실행 방지
        if (currentBgmIndex == index && bgmPlayers[index].isPlaying)
        {
            Debug.Log($"BGM {index} ({bgmClips[index].name}) 이미 재생 중");
            return;
        }

        // 🎯 기존 BGM 정지
        StopBgm();

        // 🎯 새로운 BGM 설정 및 재생
        currentBgmIndex = index;
        bgmPlayers[index].Play();

        Debug.Log($"BGM {index} ({bgmClips[index].name}) 재생 시작");
    }

    // 🎵 현재 재생 중인 모든 BGM 정지
    public void StopBgm()
    {
        if (currentBgmIndex != -1 && bgmPlayers[currentBgmIndex].isPlaying)
        {
            bgmPlayers[currentBgmIndex].Stop();
            Debug.Log($"BGM {currentBgmIndex} ({bgmClips[currentBgmIndex].name}) 정지");
        }
        currentBgmIndex = -1;
    }
}

