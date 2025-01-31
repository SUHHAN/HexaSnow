using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
    public List<Ingred> ingreList = new List<Ingred>(); // CSV에서 불러온 재료 리스트
    private GameData gameData; // DataManager에서 불러온 데이터
    private string csvFileName = "ingredient.csv"; // CSV 파일명

    [System.Serializable]
    public class Ingred
    {
        public int index;
        public string name;
        public int type;
        public int price;

        public Ingred(int index, string name, int type, int price)
        {
            this.index = index;
            this.name = name;
            this.type = type;
            this.price = price;
        }
    }

    void Start()
    {
        // CSV에서 재료 정보 불러오기
        LoadIngredientsFromCSV();

        // DataManager에서 저장된 재료 개수 불러오기
        LoadIngredientsFromGameData();
    }

    // GameDataManager에서 재료 개수 불러오기
    private void LoadIngredientsFromGameData()
    {
        gameData = DataManager.Instance?.LoadGameData();

        if (gameData == null)
        {
            Debug.LogError("LoadIngredientsFromGameData: GameData가 null입니다! DataManager에서 데이터를 가져올 수 없습니다.");
            return;
        }

        if (gameData.ingredientNum == null)
        {
            Debug.LogWarning("LoadIngredientsFromGameData: ingredientNum이 null이므로 빈 리스트로 초기화합니다.");
            gameData.ingredientNum = new List<int>(new int[ingreList.Count]); // 빈 리스트 초기화
        }

        // CSV에서 불러온 재료 리스트와 매칭하여 개수 설정
        for (int i = 0; i < ingreList.Count; i++)
        {
            string ingredientName = ingreList[i].name;
            int count = (i < gameData.ingredientNum.Count) ? gameData.ingredientNum[i] : 0;
            ingredientCounts[ingredientName] = count;
        }

        Debug.Log("재료 데이터를 성공적으로 불러왔습니다.");
        PrintCurrentInventory();
    }

    // 재료 개수 저장
    private void SaveIngredients()
    {
        if (gameData == null)
        {
            Debug.LogError("SaveIngredients: GameData가 null이므로 저장할 수 없습니다.");
            return;
        }

        // ingredientCounts 딕셔너리 값을 ingredientNum 리스트로 변환
        List<int> updatedIngredientNum = new List<int>();

        foreach (var ingredient in ingreList)
        {
            if (ingredientCounts.ContainsKey(ingredient.name))
            {
                updatedIngredientNum.Add(ingredientCounts[ingredient.name]);
            }
            else
            {
                updatedIngredientNum.Add(0);
            }
        }

        // 업데이트된 데이터 저장
        gameData.ingredientNum = updatedIngredientNum;
        DataManager.Instance.SaveGameData();
    }

    // CSV에서 재료 목록 불러오기
    private void LoadIngredientsFromCSV()
    {
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>("ingredient"); // `Resources/ingredient.csv`에서 로드
            if (csvFile == null)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다: ingredient.csv");
                return;
            }

            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(헤더) 제외
            {
                string[] fields = lines[i].Split(',');
                if (fields.Length < 4) continue;

                int index = int.Parse(fields[0].Trim());
                string name = fields[1].Trim();
                int type = int.Parse(fields[2].Trim());
                int price = int.Parse(fields[3].Trim());

                ingreList.Add(new Ingred(index, name, type, price));
            }

            Debug.Log($"총 {ingreList.Count}개의 재료를 CSV에서 불러왔습니다.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

    // 재료 추가
    public void AddIngredient(string ingredient, int count = 1)
    {
        if (ingredientCounts.ContainsKey(ingredient))
        {
            ingredientCounts[ingredient] += count;
        }
        else
        {
            ingredientCounts[ingredient] = count;
        }

        SaveIngredients();
    }

    // 재료 개수 감소 (사용)
    public bool UseIngredient(string ingredient)
    {
        if (ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0)
        {
            ingredientCounts[ingredient]--;
            SaveIngredients();
            return true;
        }
        return false;
    }

    // 재료 소지 여부
    public bool HasIngredient(string ingredient)
    {
        return ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0;
    }

    // 현재 소지한 재료 목록 출력
    private void PrintCurrentInventory()
    {
        Debug.Log("현재 소지한 재료:");
        foreach (var ingredient in ingredientCounts)
        {
            Debug.Log($"- {ingredient.Key}: {ingredient.Value}개");
        }
    }
}
