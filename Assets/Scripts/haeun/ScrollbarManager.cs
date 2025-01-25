using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScrollbarManager : MonoBehaviour
{
    public static ScrollbarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScrollbarManager>();
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class Ingredient
    {
        public int index;
        public string name;
        public int type;
        public int price;

        public Ingredient(int index, string name, int type, int price)
        {
            this.index = index;
            this.name = name;
            this.type = type;
            this.price = price;
        }
    }

    private string csvFileName = "ingredient.csv"; // CSV 파일명
    public List<Ingredient> ingredientList = new List<Ingredient>(); // 재료 리스트
    public List<int> ingre_Num = new List<int>();

    public GameObject itemPrefab; // 아이템 Prefab
    public Transform content; // Content 오브젝트

    [SerializeField] private TextMeshProUGUI UseScText;

    private static ScrollbarManager _instance;

    private int FinalSc = 0; // 초기 점수
    private int SumSc;

    void Start()
    {
        LoadIngredientsFromCSV();

        AddItems(); // 10개의 아이템 추가
        CalculateNum(); // 초기 SumSc 계산

        // PrintIngredients(); // 확인용
    }

    public void AddItems()
    {
        try {
            foreach (var ingre in ingredientList)
            {
                if(ingre.type == 1) {
                    GameObject item = Instantiate(itemPrefab, content);

                    // 카드 스프라이트의 인덱스 지정 변수
                    Ingredient_h ingredient = item.GetComponent<Ingredient_h>();
                    ingredient.SetIngredientID(ingre.index);
                    ingredient.SetPrice(ingre.price);

                    // ingre_Num 리스트와 연동하여 currentNum 설정
                    ingredient.currentNum = ingre_Num[ingre.index];
                    ingredient.InitNum(); // UI 업데이트

                    item.GetComponentInChildren<TextMeshProUGUI>().text = $"{ingre.index} | {ingre.name}";
                }
            }
        }
        
        catch (System.Exception ex)
        {
            Debug.LogError($"뭔가 이상이상: {ex.Message}");
        }
    }

    public void SetFinalScore(int score)
    {
        FinalSc = score;
        CalculateNum();
    }

    private void CalculateNum()
    {
        SumSc = FinalSc;
        UpdateScoreDisplay();
    }

    public bool TryPurchase(int requiredScore)
    {
        if (SumSc >= requiredScore)
        {
            SumSc -= requiredScore;
            UpdateScoreDisplay();
            return true;
        }
        else
        {
            UpdateScoreDisplay();
            return false;
        }
    }

    public void InitAllNum()
    {
        // 모든 아이템의 currentNum을 초기화
        foreach (Transform child in content)
        {
            Ingredient_h ingredient = child.GetComponent<Ingredient_h>();
            if (ingredient != null)
            {
                ingredient.currentNum = 0;
                ingredient.InitNum(); // UI 갱신
            }
        }

        // 사용 가능 점수를 초기 상태로 복구
        CalculateNum();

        Debug.Log("모든 currentNum이 초기화되고 점수가 복구되었습니다.");
    }

    private void UpdateScoreDisplay()
    {
        UseScText.text = $"사용 가능 포인트 : {SumSc}";
    }

    private void LoadIngredientsFromCSV()
    {
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(csvFileName));
            if (csvFile == null)
            {
                Debug.LogError($"CSV 파일을 찾을 수 없습니다: {csvFileName}");
                return;
            }

            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // 첫 번째 줄 (헤더) 무시하고 읽기
            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(',');
                if (fields.Length < 4) continue;

                int index = int.Parse(fields[0].Trim());
                string name = fields[1].Trim();
                int type = int.Parse(fields[2].Trim());
                int price = int.Parse(fields[3].Trim());

                ingredientList.Add(new Ingredient(index, name, type, price));
            }

            // ingre_Num 리스트를 재료 리스트 크기만큼 0으로 초기화
            ingre_Num = new List<int>(new int[ingredientList.Count + 1]);

            Debug.Log($"총 {ingredientList.Count}개의 재료를 불러왔습니다.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

    private void PrintIngredients()
    {
        foreach (var ingredient in ingredientList)
        {
            Debug.Log($"Index: {ingredient.index}, Name: {ingredient.name}, Type: {ingredient.type}, Price: {ingredient.price}");
        }
    }


    public void SaveIngreData()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.gameData.SetIngredient(ingre_Num);
            DataManager.Instance.SaveGameData(); // 저장 함수 호출
            Debug.Log("GameData에 ingre_Num 저장 완료!");
        }
        else
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    
}
