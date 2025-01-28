using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public bool isGuestLoggedIn;
    public string guestId;
    public string lastLoginDate;
    public List<int> ingredientNum = new List<int>(); // 재료 개수 저장 리스트

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
    // 나중에 추가할 게임 진행 정보
}


public class DataManager : MonoBehaviour
{
    private string gameDataPath; // 저장할 JSON 파일 경로
    public static DataManager Instance { get; private set; }
    public GameData gameData = new GameData();

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

    // 게임 데이터를 JSON 파일로 저장
    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(gameDataPath, json);
        Debug.Log("게임 데이터가 저장되었습니다: " + gameDataPath);
    }

    // 저장된 게임 데이터 불러오기
    public GameData LoadGameData()
    {
        if (File.Exists(gameDataPath))
        {
            string json = File.ReadAllText(gameDataPath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("게임 데이터를 로드했습니다: " + gameDataPath);

            
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