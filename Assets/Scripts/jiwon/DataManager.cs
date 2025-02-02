using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;


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


public class DataManager : MonoBehaviour
{
    private string gameDataPath; // 저장할 JSON 파일 경로
    public static DataManager Instance { get; private set; }
    public GameData gameData = new GameData();

    public event Action OnDataChanged;

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
            Destroy(gameObject);
        }
    }

    // 초기 게임 데이터 설정
    private void SetInitialGameData()
    {
        gameData.isGuestLoggedIn = true;
        gameData.date = 1;  
        gameData.money = 5000;  
        gameData.ingredientNum = gameData.ingredientNum = new List<int>{3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0};
        gameData.myBake = new List<MyRecipeList>();
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
       // Debug.Log("게임 데이터가 저장되었습니다: " + gameDataPath);
    }

    // 저장된 게임 데이터 불러오기
    public GameData LoadGameData()
    {
        if (File.Exists(gameDataPath))
        {
            string json = File.ReadAllText(gameDataPath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("게임 데이터를 로드했습니다: " + gameDataPath);
            gameData = data;
            return data;
        }
        else
        {
            Debug.LogWarning("저장된 게임 데이터가 없습니다.");
            return null;
        }
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

