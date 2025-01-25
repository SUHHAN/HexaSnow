using UnityEngine;
using System.IO;

[System.Serializable]
public class GameData
{
    public bool isGuestLoggedIn;
    public string guestId;
    public string lastLoginDate;
    // 나중에 추가할 게임 진행 정보
}

public class DataManager : MonoBehaviour
{
    private string gameDataPath; // 저장할 JSON 파일 경로

    // DataManager가 시작될 때 게임 데이터 파일 경로 설정
    private void Awake()
    {
        gameDataPath = Path.Combine(Application.persistentDataPath, "GameData.json");
    }

    // 게임 데이터를 JSON 파일로 저장
    public void SaveGameData(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
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
