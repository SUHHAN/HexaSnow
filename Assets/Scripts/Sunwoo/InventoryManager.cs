using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<int, int> ingredientCounts = new Dictionary<int, int>(); // 인덱스 기반으로 재료 개수 관리
    public List<Ingred> ingreList = new List<Ingred>(); // CSV에서 불러온 재료 리스트 (토핑 제외)
    private GameData gameData; // DataManager에서 불러온 데이터
    private string csvFileName = "ingredient.csv"; // CSV 파일명

    [System.Serializable]
    public class Ingred
    {
        public int index;
        public string name;
        public int type;
        public int price;
        public string ename; // 영어 이름 추가

        public Ingred(int index, string name, int type, int price, string ename)
        {
            this.index = index;
            this.name = name;
            this.type = type;
            this.price = price;
            this.ename = ename;
        }
    }

    void Start()
    {
        LoadIngredientsFromCSV();
        Debug.Log($"ingreList에 로드된 재료 개수: {ingreList.Count}");
        LoadIngredientsFromGameData();
        PrintIngredientList();
    }

    private void LoadIngredientsFromGameData()
    {
        gameData = DataManager.Instance.gameData;

        if (gameData == null)
        {
            Debug.LogError("LoadIngredientsFromGameData: GameData가 null입니다! DataManager에서 데이터를 가져올 수 없습니다.");
            return;
        }

        if (gameData.ingredientNum == null)
        {
            Debug.LogWarning("LoadIngredientsFromGameData: ingredientNum이 null이므로 빈 리스트로 초기화합니다.");
            gameData.ingredientNum = new List<int>(new int[ingreList.Count]);
        }

        for (int i = 0; i < ingreList.Count; i++)
        {
            int ingredientIndex = ingreList[i].index;
            int count = (i < gameData.ingredientNum.Count) ? gameData.ingredientNum[i] : 0;
            ingredientCounts[ingredientIndex] = count;
        }

        Debug.Log("재료 데이터를 성공적으로 불러왔습니다.");
        PrintCurrentInventory();
    }

    private void SaveIngredients()
    {
        if (DataManager.Instance?.gameData == null)
        {
            Debug.LogError("SaveIngredients: GameData가 null이므로 저장할 수 없습니다.");
            return;
        }

        List<int> ingredientNumList = DataManager.Instance.gameData.ingredientNum;

        if (ingredientNumList == null || ingredientNumList.Count < ingreList.Count)
        {
            ingredientNumList = new List<int>(new int[ingreList.Count]);
            DataManager.Instance.gameData.ingredientNum = ingredientNumList;
        }

        for (int i = 0; i < ingreList.Count; i++)
        {
            int index = ingreList[i].index;
            if (ingredientCounts.ContainsKey(index))
            {
                ingredientNumList[i] = ingredientCounts[index];
            }
            else
            {
                ingredientNumList[i] = 0;
            }
        }

        DataManager.Instance.SaveGameData();
        Debug.Log("GameData에 재료 수량 저장 완료!");
    }

    private void LoadIngredientsFromCSV()
    {
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>("ingredient");
            if (csvFile == null)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다: ingredient.csv");
                return;
            }

            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(',');
                if (fields.Length < 5) continue;

                int index = int.Parse(fields[0].Trim());
                if (index >= 16) continue; // 토핑 제외

                string name = fields[1].Trim();
                int type = int.Parse(fields[2].Trim());
                int price = int.Parse(fields[3].Trim());
                string ename = fields[4].Trim().ToLower();

                ingreList.Add(new Ingred(index, name, type, price, ename));
                Debug.Log($"로드됨: {index}, {name}, {ename}");
            }

            Debug.Log($"총 {ingreList.Count}개의 재료를 CSV에서 불러왔습니다.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

    public void AddIngredient(int ingredientIndex, int count = 1)
    {
        if (ingredientCounts.ContainsKey(ingredientIndex))
        {
            ingredientCounts[ingredientIndex] += count;
        }
        else
        {
            ingredientCounts[ingredientIndex] = count;
        }

        SaveIngredients();
    }

    public bool UseIngredient(int ingredientIndex)
    {
        if (ingredientCounts.ContainsKey(ingredientIndex) && ingredientCounts[ingredientIndex] > 0)
        {
            ingredientCounts[ingredientIndex]--;
            SaveIngredients();
            return true;
        }
        return false;
    }

    public bool HasIngredient(int ingredientIndex)
    {
        return ingredientCounts.ContainsKey(ingredientIndex) && ingredientCounts[ingredientIndex] > 0;
    }

    public int GetIngredientIndexFromEname(string ename)
    {
        ename = ename.Trim().ToLower();
        Debug.Log($"Searching for Ingredient: '{ename}'");

        foreach (var ingredient in ingreList)
        {
            if (ingredient.ename == ename)
            {
                return ingredient.index;
            }
        }

        return -1;
    }

    public string GetIngredientEname(int index)
    {
        foreach (var ingredient in ingreList)
        {
            if (ingredient.index == index)
            {
                return ingredient.ename;
            }
        }
        Debug.LogError($"GetIngredientEname: 재료 인덱스 '{index}'을 찾을 수 없습니다.");
        return null;
    }

    public void PrintCurrentInventory()
    {
        Debug.Log("현재 소지한 재료:");
        foreach (var ingredient in ingredientCounts)
        {
            string ename = GetIngredientEname(ingredient.Key);
            Debug.Log($"- {ename} ({ingredient.Key}): {ingredient.Value}개");
        }
    }

    private void PrintIngredientList()
    {
        Debug.Log("현재 ingreList의 재료 목록:");
        foreach (var ingredient in ingreList)
        {
            Debug.Log($"- Index: {ingredient.index}, Name: {ingredient.name}, eName: {ingredient.ename}");
        }
    }
}
