using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]

public class MyRecipeList
{
    public int index;
    public int menuID;
    public string name;
    public int score;
    public bool bonus;

    public MyRecipeList(int index, int menuID, string name, int score, bool bonus)
    {
        this.index = index;
        this.menuID = menuID;
        this.name = name;
        this.score = score;
        this.bonus = bonus;
    }
}

[System.Serializable]
public class GameData
{
    public bool isGuestLoggedIn;
    public int date;
    public int money;  
    public float time;
    public List<int> ingredientNum = new List<int>(); // 재료 개수 저장 리스트
    public List<MyRecipeList> myBake = new List<MyRecipeList>(); // 내가 만든 요리~
    public string serializedDailyOrders; //주문서
    public string currentScene;
    public bool hasCompletedBakingTutorial = false;    // 베이킹 튜토리얼 완료 여부
    public bool hasCompletedIngredientTutorial; // 재료 튜토리얼 완료 여부
    public bool hasCompletedBonusTutorial;     // 보너스 튜토리얼 완료 여부
    public int EndingCount=0;
    public bool Spe_visitDone=false;
    public bool Spe_BakingOn = false;

    public void SetIngredient(List<int> newIngredientNum)
    {
        if (newIngredientNum == null)
        {
            Debug.LogError("SetIngredient: 입력된 리스트가 null 입니다!");
            return;
        }

        ingredientNum = new List<int>(newIngredientNum); // 리스트 복사하여 저장
        Debug.Log($"GameData에 ingredientNum 저장 완료: [{string.Join(", ", ingredientNum)}]");
    }

    public void SetMyBake(List<MyRecipeList> newMyBake)
    {
        if (newMyBake == null)
        {
            Debug.LogError("SetMyBake: 입력된 리스트가 null 입니다!");
            return;
        }

        myBake = new List<MyRecipeList>(newMyBake); // 리스트 복사하여 저장
        Debug.Log($"GameData에 myBake 저장 완료: [{string.Join(", ", myBake)}]");
    }


    // 나중에 추가할 게임 진행 정보
    

}

[System.Serializable]
public class SlotData
{
    public GameData gameData;
    public string sceneName;
    public string fileName = "ContinueSlot.json";

    public string GetFullPath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public SlotData(GameData data, string sceneName)
    {
        this.gameData = data;
        this.sceneName = sceneName;
    }
}


public class DataManager : MonoBehaviour
{
    private string gameDataPath; // 저장할 JSON 파일 경로
    public static DataManager Instance { get; private set; }
    public GameData gameData = new GameData();
    public SlotData continueSlot = new SlotData(new GameData(), "StartScene");

    private void Awake()
    {
        gameDataPath = Path.Combine(Application.persistentDataPath, "GameData.json");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("중복된 DataManager가 생성되어 삭제되었습니다. 씬에 하나만 있어야 합니다.");
            Destroy(gameObject);
        }
    }

    // 초기 게임 데이터 설정
    private void SetInitialGameData()
    {
        gameData.isGuestLoggedIn = true;
        gameData.date = 1;
        gameData.money = 5000;
        gameData.ingredientNum = gameData.ingredientNum = new List<int> { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
        gameData.myBake = new List<MyRecipeList>{};


        Debug.Log("초기 게임 데이터 설정 완료");
    }

    // 초기 게임 데이터를 JSON 파일로 저장 (새로 시작할 때만 호출)
    public void SaveInitialGameData()
    {
        SetInitialGameData();  // 초기 게임 데이터 설정
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(gameDataPath, json);
        Debug.Log("초기 게임 데이터가 저장되었습니다: " + gameDataPath);
    }

    // 게임 데이터를 JSON 파일로 저장
    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(gameDataPath, json);
        //Debug.Log("게임 데이터가 저장되었습니다: " + gameDataPath);
    }

    // 저장된 게임 데이터 불러오기
    public GameData LoadGameData()
    {
        if (File.Exists(gameDataPath))
        {
            string json = File.ReadAllText(gameDataPath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            //Debug.Log("게임 데이터를 로드했습니다: " + gameDataPath);
            gameData = data;
            return data;
        }
        else
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다.");
            return null;
        }
    }

    public void SaveSlotData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        gameData.currentScene = sceneName;

        continueSlot = new SlotData(gameData, sceneName);
        string json = JsonUtility.ToJson(continueSlot, true);
        File.WriteAllText(continueSlot.GetFullPath(), json);

        Debug.Log($"[저장] 이어하기 슬롯 저장 완료: {sceneName}");
    }

    public SlotData LoadSlotData()
    {
        string path = continueSlot.GetFullPath();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[불러오기] 이어하기 슬롯 파일이 없습니다.");
        }

        string json = File.ReadAllText(path);
        continueSlot = JsonUtility.FromJson<SlotData>(json);
        gameData = continueSlot.gameData;
        SaveGameData();

        Debug.Log($"[불러오기] 불러온 날짜: {gameData.date}, 돈: {gameData.money}, 시간: {gameData.time}");
        Debug.Log($"[불러오기] 이어하기 슬롯 로드 완료, 씬 이동: {continueSlot.sceneName}");
        SceneManager.LoadScene(continueSlot.sceneName);
        return continueSlot;

    }

    public void OnClick_ContinueSlot()
    {
        LoadSlotData();
    }

    // 게임 데이터의 게스트 로그인 상태 확인
    public bool IsGuestLoggedIn()
    {
        GameData data = LoadGameData();
        return data != null && data.isGuestLoggedIn;
    }
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [System.Serializable]
    public struct KeyValuePair
    {
        public TKey Key;
        public TValue Value;
    }

    public List<KeyValuePair> keyValuePairs = new List<KeyValuePair>();

    public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
    {
        foreach (var pair in dictionary)
        {
            keyValuePairs.Add(new KeyValuePair { Key = pair.Key, Value = pair.Value });
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        foreach (var pair in keyValuePairs)
        {
            dictionary[pair.Key] = pair.Value;  // Key와 Value를 제대로 할당
        }

        return dictionary;
    }
}

