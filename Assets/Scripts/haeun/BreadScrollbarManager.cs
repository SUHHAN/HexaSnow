
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Microsoft.Unity.VisualStudio.Editor;

public class BreadScrollbarManager : MonoBehaviour
{
    private static BreadScrollbarManager _instance;
    public static BreadScrollbarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BreadScrollbarManager>();
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class Recipe
    {
        public int index;
        public string name;

        public Recipe(int index, string name)
        {
            this.index = index;
            this.name = name;
        }
    }

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


    [SerializeField] private GameData ingredientGD = new GameData();

    private string csvFileName = "recipe.csv"; // CSV 파일명
    // public List<Recipe> MenuList = new List<Recipe>(); // 재료 리스트
    public List<MyRecipeList> MyList = new List<MyRecipeList>(); // 재료 리스트
    // public List<int> Menu_Num = new List<int>();
    [SerializeField] private Sprite[] MenuSprites;

    public GameObject itemPrefab; // 아이템 Prefab
    public Transform content; // Content 오브젝트
    [SerializeField] private GameObject BonusPanel;



    private int FinalSc = 0; // 초기 점수
    private int SumSc;

    

    void Start()
    {

        BonusPanel.SetActive(false);

        // LoadIngredientsFromCSV();
        // test 코드
        AddDummyData();


        AddItems(); // 10개의 아이템 추가

        // PrintIngredients(); // 확인용
    }

    public void AddItems()
    {
        // try {
        //     foreach (var me in MyList)
        //     {
        //         if(me.bonus == false) {
        //             GameObject item = Instantiate(itemPrefab, content);

        //             // 카드 스프라이트의 인덱스 지정 변수
        //             Bread_h menu = item.GetComponent<Bread_h>();
        //             menu.SetMenuID(me.menuID);
        //             menu.SetIndex(me.index);
        //             menu.SetScore(me.score);
        //             menu.SetBonus(me.bonus);

        //             Transform menuImage = item.transform.Find("menuImage");
        //             menuImage.GetComponent<UnityEngine.UI.Image>().sprite = MenuSprites[me.menuID];

        //             //item의 색상을 각 등급에 맞는 색으로 지정하는 함수 작성하기
        //             menu.SetMenuColor();
        //             menu.SetButtonActive();
        //         }
        //     }
        // }
        // catch (System.Exception ex)
        // {
        //     Debug.LogError($"뭔가 이상이상: {ex.Message}");
        // }

        foreach (var me in MyList)
        {
            
            GameObject item = Instantiate(itemPrefab, content);

            // 카드 스프라이트의 인덱스 지정 변수
            Bread_h menu = item.GetComponent<Bread_h>();
            menu.SetMenuID(me.menuID);
            menu.SetIndex(me.index);
            menu.SetScore(me.score);
            menu.SetBonus(me.bonus);

            Transform menuImage = item.transform.Find("menuImage");
            menuImage.GetComponent<UnityEngine.UI.Image>().sprite = MenuSprites[me.menuID];

            //item의 색상을 각 등급에 맞는 색으로 지정하는 함수 작성하기
            menu.SetMenuColor();
            menu.SetButtonActive();
            
        }
    }

    public void SlotClick()
    {
        BonusPanel.SetActive(true);
    }

    public void YesButtonClick()
    {       
        SceneManager.LoadScene("Match");
    }

    public void NoButtonClick()
    {
        BonusPanel.SetActive(false);

        PlayerPrefs.DeleteKey("SelectedMenuIndex");
        PlayerPrefs.Save(); // 변경 사항 저장

        // ✅ 키가 존재하는지 확인
        if (PlayerPrefs.HasKey("SelectedMenuIndex"))
        {
            int savedIndex = PlayerPrefs.GetInt("SelectedMenuIndex");
            Debug.Log($"✔ 'SelectedMenuIndex' 키가 존재합니다. 저장된 값: {savedIndex}");
        }
        else
        {
            Debug.LogWarning("⚠ 'SelectedMenuIndex' 키가 존재하지 않습니다.");
        }
    }

    // ✅ 더미 데이터 추가 함수
    private void AddDummyData()
    {
        MyList.Add(new MyRecipeList(0, 1, "초코 브레드", 60, false));
        MyList.Add(new MyRecipeList(1, 2, "치즈 브레드", 40, false));
        MyList.Add(new MyRecipeList(2, 3, "바게트", 20, true));
        MyList.Add(new MyRecipeList(3, 4, "마늘 바게트", 10, false));
        MyList.Add(new MyRecipeList(4, 5, "소시지 롤", 15, false));

        Debug.Log("더미 데이터가 정상적으로 추가되었습니다.");
    }

    

    // public void InitAllNum()
    // {
    //     // 모든 아이템의 currentNum을 초기화
    //     foreach (Transform child in content)
    //     {
    //         Ingredient_h ingredient = child.GetComponent<Ingredient_h>();
    //         if (ingredient != null)
    //         {
    //             ingredient.currentNum = 0;
    //             ingredient.InitNum(); // UI 갱신
    //         }
    //     }

    //     // 사용 가능 점수를 초기 상태로 복구
    //     CalculateNum();

    //     Debug.Log("모든 currentNum이 초기화되고 점수가 복구되었습니다.");
    // }

    // 이건 혹시라도 내가 메뉴의 이름을 표시할 필요성이 있다면, csv가 필요하게 됨. -> 지금 당장은 필요가 없음
    // private void LoadIngredientsFromCSV()
    // {
    //     try
    //     {
    //         TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(csvFileName));
    //         if (csvFile == null)
    //         {
    //             Debug.LogError($"CSV 파일을 찾을 수 없습니다: {csvFileName}");
    //             return;
    //         }

    //         string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

    //         // 첫 번째 줄 (헤더) 무시하고 읽기
    //         for (int i = 1; i < lines.Length; i++)
    //         {
    //             string[] fields = lines[i].Split(',');
    //             if (fields.Length < 4) continue;

    //             int index = int.Parse(fields[0].Trim());
    //             string name = fields[1].Trim();

    //             MenuList.Add(new Recipe(index, name));
    //         }

    //         // Menu_Num 리스트를 재료 리스트 크기만큼 0으로 초기화
    //         Menu_Num = new List<int>(new int[MenuList.Count + 1]);

    //         Debug.Log($"총 {MenuList.Count}개의 레시피 리스트를 불러왔습니다.");
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
    //     }
    // }

}

