using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<int, int> ingredientCounts = new Dictionary<int, int>(); // 인덱스 기반으로 재료 개수 관리
    public List<Ingred> ingreList = new List<Ingred>(); // CSV에서 불러온 재료 리스트
    private GameData gameData; // DataManager에서 불러온 데이터
    private string csvFileName = "ingredient.csv"; // CSV 파일명

    // 냉장고에 있는 재료 버튼 리스트 추가
    public List<GameObject> refrigeratorButtons = new List<GameObject>();

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
        // CSV에서 재료 정보 불러오기
        LoadIngredientsFromCSV();

        Debug.Log($"ingreList에 로드된 재료 개수: {ingreList.Count}");

        // DataManager에서 저장된 재료 개수 불러오기
        LoadIngredientsFromGameData();

        // 냉장고 재료 버튼 업데이트
        UpdateRefrigeratorButtons();

        PrintIngredientList();
    }

    // 냉장고 재료 버튼 업데이트 (index 기반으로 버튼 활성화)
    public void UpdateRefrigeratorButtons()
    {
        foreach (GameObject buttonObj in refrigeratorButtons)
        {
            string buttonName = buttonObj.name.Replace("Button", ""); // 버튼 이름에서 "Button" 제거
            int ingredientIndex = GetIngredientIndexFromEname(buttonName); // ename을 index로 변환

            bool hasIngredient = HasIngredient(ingredientIndex);

            buttonObj.SetActive(hasIngredient);
            Debug.Log($"재료 버튼 업데이트: {buttonName} (Index: {ingredientIndex}), 소지 여부: {hasIngredient}");
        }
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
            int ingredientIndex = ingreList[i].index;
            int count = (i < gameData.ingredientNum.Count) ? gameData.ingredientNum[i] : 0;
            ingredientCounts[ingredientIndex] = count;
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
            if (ingredientCounts.ContainsKey(ingredient.index))
            {
                updatedIngredientNum.Add(ingredientCounts[ingredient.index]);
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
            TextAsset csvFile = Resources.Load<TextAsset>("ingredient");
            if (csvFile == null)
            {
                Debug.LogError("CSV 파일을 찾을 수 없습니다: ingredient.csv");
                return;
            }

            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(헤더) 제외
            {
                string[] fields = lines[i].Split(',');
                if (fields.Length < 5) continue; // ename까지 있는지 확인

                int index = int.Parse(fields[0].Trim());
                string name = fields[1].Trim();
                int type = int.Parse(fields[2].Trim());
                int price = int.Parse(fields[3].Trim());
                string ename = fields[4].Trim().ToLower(); // ename 추가

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

    // 재료 추가
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
        UpdateRefrigeratorButtons(); // 재료가 추가되었으므로 버튼 업데이트
    }

    // 재료 개수 감소 (사용)
    public bool UseIngredient(int ingredientIndex)
    {
        if (ingredientCounts.ContainsKey(ingredientIndex) && ingredientCounts[ingredientIndex] > 0)
        {
            ingredientCounts[ingredientIndex]--;
            SaveIngredients();
            UpdateRefrigeratorButtons(); // 재료가 감소했으므로 버튼 업데이트
            return true;
        }
        return false;
    }

    // 재료 소지 여부
    public bool HasIngredient(int ingredientIndex)
    {
        return ingredientCounts.ContainsKey(ingredientIndex) && ingredientCounts[ingredientIndex] > 0;
    }

    // 영어 이름(ename)으로 인덱스 찾기
    public int GetIngredientIndexFromEname(string ename)
    {
        ename = ename.Trim().ToLower();
        Debug.Log($"Searching for Ingredient: '{ename}'");

        foreach (var ingredient in ingreList)
        {
            if (ingredient.ename == ename) // ename 기준으로 조회
            {
                return ingredient.index;
            }
        }

        //Debug.LogError($"GetIngredientIndexFromEname: 재료 영어 이름 '{ename}'을 찾을 수 없습니다. 현재 ingreList의 ename 목록: ");
        /*foreach (var ingredient in ingreList)
        {
            Debug.Log($"- {ingredient.ename}");
        }*/

        return -1;
    }

    // 인덱스로 재료 이름 찾기
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

    // 현재 소지한 재료 목록 출력
    private void PrintCurrentInventory()
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
