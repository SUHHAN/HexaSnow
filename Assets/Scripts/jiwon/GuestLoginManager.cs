using UnityEngine;
using System.IO;

// 게스트 데이터를 저장할 클래스
[System.Serializable]
public class GuestData
{
    public bool isLoggedIn;   // 로그인 상태
    public string guestId;    // 고유 게스트 ID
    public string lastLoginDate;  // 마지막 로그인 날짜
}

public class GuestLoginManager : MonoBehaviour
{
    private string guestDataPath; // JSON 파일 경로

    private void Awake()
    {
        // JSON 파일 경로 설정
        guestDataPath = Path.Combine(Application.persistentDataPath, "GuestData.json");
    }

    // 게스트 데이터 저장
    public void SaveGuestData(GuestData data)
    {
        string json = JsonUtility.ToJson(data, true); // 데이터를 JSON 형식으로 변환
        File.WriteAllText(guestDataPath, json); // JSON 파일로 저장
        Debug.Log("게스트 데이터가 저장되었습니다: " + guestDataPath);
    }

    // 저장된 게스트 데이터 불러오기
    public GuestData LoadGuestData()
    {
        if (File.Exists(guestDataPath))
        {
            string json = File.ReadAllText(guestDataPath); // JSON 파일 읽기
            GuestData data = JsonUtility.FromJson<GuestData>(json); // JSON 데이터를 객체로 변환
            Debug.Log("게스트 데이터를 로드했습니다: " + guestDataPath);
            return data;
        }
        else
        {
            Debug.LogWarning("저장된 게스트 데이터가 없습니다. 기본 데이터를 생성합니다.");

            // 기본 데이터를 생성하여 반환
            GuestData defaultData = new GuestData
            {
                isLoggedIn = false,
                guestId = string.Empty,
                lastLoginDate = string.Empty
            };

            SaveGuestData(defaultData); // 기본 데이터를 저장
            return defaultData;
        }
    }

    // 게스트 로그인 상태 확인
    public bool IsGuestLoggedIn()
    {
        GuestData data = LoadGuestData(); // 데이터 로드
        return data != null && data.isLoggedIn; // 로그인 여부 반환
    }

    // 게스트 로그인 처리
    public void GuestLogin()
    {
        GuestData data = new GuestData
        {
            isLoggedIn = true,
            guestId = System.Guid.NewGuid().ToString(), // 고유 ID 생성
            lastLoginDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // 현재 시간 저장
        };

        SaveGuestData(data); // 로그인 데이터 저장
        Debug.Log("게스트 로그인 성공! ID: " + data.guestId);
    }

    // 게스트 로그아웃 처리
    public void GuestLogout()
    {
        GuestData data = LoadGuestData();
        if (data != null)
        {
            data.isLoggedIn = false; // 로그아웃 처리
            SaveGuestData(data); // 상태 저장
            Debug.Log("게스트 로그아웃 완료.");
        }
        else
        {
            Debug.LogWarning("로그아웃할 게스트 데이터가 없습니다.");
        }
    }
}

