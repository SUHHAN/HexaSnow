using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Drawing;
using System.Linq;
using Unity.Collections;

public class RecipeBookManager : MonoBehaviour
{
    // UI 요소
    public TMP_Text recipeNameText;  // 레시피 이름 표시
    public TMP_Text ingredientsText; // 재료 목록 표시

    public TMP_Text priceText; // 가격 표시시

    public Button NextButton;    // NEXT 버튼
    public Button PrevButton;    // PREV 버튼
    public Button RecipeContentIndex; // 목차 돌아가기 버튼

    public GameObject TableOfContentsPanel; // 목차 패널
    public GameObject RecipePagePanel;      // 레시피 페이지 패널
    public Transform buttonContainer;       // 목차 버튼 컨테이너

    public TMP_FontAsset customFont;

    private List<RecipeB> recipes = new List<RecipeB>();
    private int currentRecipeIndex = 0;
    private bool isTableOfContentsPage = true;



    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject pref2;
    [SerializeField] private Sprite[] MenuSprites;

    [System.Serializable]
    public class RecipeB
    {
        public int index;                 // 레시피 인덱스
        public string menu;                // 메뉴 이름
        public List<string> ingredients;   // 재료 목록타입
        public string category;
        public string subCategory;
        public int coin;

        // ToString 메서드 오버라이드
        public override string ToString()
        {
            string ingredientsList = string.Join(", ", ingredients); // 재료 목록을 문자열로 변환
            return $"Index: {index}, Menu: {menu}, Category: {category}, SubCategory: {subCategory}, Ingredients: {ingredientsList}, Coin: {coin}";
        }
    }

    void Start()
    {
        // CSV 파일 로드
        LoadRecipesFromCSV("Assets/Resources/recipe.csv");

        // UI 초기화
        DisplayTableOfContents();

        // // 버튼 이벤트 연결
        // NextButton.onClick.AddListener(OnNextClicked);
        // PrevButton.onClick.AddListener(OnPrevClicked);
        // RecipeContentIndex.onClick.AddListener(OnRecipeContentIndexClicked);
    }

    // CSV 파일에서 레시피 데이터 로드
    void LoadRecipesFromCSV(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            // 커스텀 파서로 CSV 행을 파싱
            var columns = ParseCSVLine(line);


            // 데이터 개수 확인
            if (columns.Length < 6)
            {
                Debug.LogWarning($"잘못된 CSV 데이터: {line}");
                continue;
            }

            int index, coin;

            // 안전한 정수 변환 (TryParse 사용)
            if (!int.TryParse(columns[0], out index)) index = 0;
            if (!int.TryParse(columns[5], out coin)) coin = 0;

            string menu = columns[1];
            

            // Ingredients는 쉼표로 구분된 문자열을 List<string>으로 변환
            string ingredientsStr = columns[2].Trim('"');
            string[] ingredientsArray = ingredientsStr.Split(',');

            List<string> ingredients = new List<string>(ingredientsArray);

            // 메뉴 이름을 공백 기준으로 나누기
            string[] menuParts = menu.Split(' ');  // 공백으로 분리 (예: Original Madeleine)
            string subCategory = menuParts[0];    // 첫 번째 단어는 서브 카테고리로
            string category = menuParts.Length > 1 ? menuParts[1] : "";  // 두 번째 단어는 카테고리로

            // RecipeB 객체 생성
            RecipeB recipe = new RecipeB()
            {
                index = index,
                menu = menu,
                ingredients = ingredients,
                category = category,
                subCategory = subCategory,
                coin = coin
            };

            // RecipeB 객체 출력 (콘솔에)
            Debug.Log(recipe.ToString());

            recipes.Add(recipe);
        }
    }

    // 커스텀 CSV 라인 파서
    string[] ParseCSVLine(string line)
    {
        var columns = new List<string>();
        bool insideQuotes = false;
        string currentColumn = "";

        foreach (char c in line)
        {
            if (c == '"' && !insideQuotes)
            {
                insideQuotes = true;  // 따옴표 안으로 들어감
            }
            else if (c == '"' && insideQuotes)
            {
                insideQuotes = false;  // 따옴표 밖으로 나감
            }
            else if (c == ',' && !insideQuotes)
            {
                columns.Add(currentColumn.Trim());
                currentColumn = "";  // 새로운 컬럼 시작
            }
            else
            {
                currentColumn += c;  // 현재 컬럼에 문자 추가
            }
        }

        // 마지막 컬럼 추가
        if (!string.IsNullOrEmpty(currentColumn))
        {
            columns.Add(currentColumn.Trim());
        }

        return columns.ToArray();
    }


    // 목차 페이지 표시
    void DisplayTableOfContents()
    {
        // 목차 페이지 활성화 및 레시피 페이지 비활성화
        TableOfContentsPanel.SetActive(true);
        RecipePagePanel.SetActive(false);

        // 기존 버튼 제거
        foreach (Transform child in buttonContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // 레시피 그룹화 (카테고리별 그룹화)
        var groupedRecipes = recipes
            .GroupBy(recipe => recipe.category)  // 카테고리별로 그룹화
            .ToList();

        // 카테고리 버튼 생성
        foreach (var group in groupedRecipes)
        {
            // 카테고리 버튼 생성
            GameObject categoryButtonObj = new GameObject(group.Key);  // 카테고리 이름 사용
            categoryButtonObj.transform.SetParent(buttonContainer.transform); // ButtonContainer 아래로 배치

            Debug.Log("카테고리 생성: " + group.Key); // 카테고리명 출력

            // 카테고리 버튼에 TextMeshProUGUI 컴포넌트 추가
            TextMeshProUGUI categoryButtonText = categoryButtonObj.AddComponent<TextMeshProUGUI>();
            categoryButtonText.text = group.Key;

            // 폰트 설정
            if (customFont != null)
            {
                categoryButtonText.font = customFont;
            }
            else
            {
                Debug.LogError("폰트가 설정되지 않았습니다! Unity Inspector에서 customFont를 설정하세요.");
            }
            categoryButtonText.color = UnityEngine.Color.black;
            categoryButtonText.fontSize = 30;

            // 카테고리 버튼 크기 설정 (선택 사항)
            RectTransform rectTransform = categoryButtonObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300, 50);  // 버튼 크기 설정 (300x50)

            // 카테고리 버튼 클릭 시 해당 카테고리의 레시피 페이지로 이동
            categoryButtonObj.AddComponent<Button>().onClick.AddListener(() =>
            {
                ShowRecipePage(group.Key);  // 해당 카테고리에 맞는 레시피 페이지로 이동
            });
        }
    }


    public void ShowRecipePage(string key)
{
    // // ✅ `pref1`이 동적으로 생성되지 않았다면, 새로 생성
    // if (pref1 == null)
    // {
    //     Debug.LogError("pref1이 할당되지 않았습니다! Unity Inspector에서 Prefab을 지정해주세요.");
    //     return;
    // }

    // // ✅ 기존에 생성된 `pref1`이 있다면 삭제
    // GameObject existingPref1 = GameObject.Find(pref1.name + "(Clone)");
    // if (existingPref1 != null)
    // {
    //     Destroy(existingPref1);
    // }

    // ✅ `pref1`을 동적으로 생성하여 UI에 추가
    // GameObject newPref = Instantiate(pref1, RecipePagePanel.transform);  // RecipePagePanel의 자식으로 추가
    // newPref.SetActive(true);  // 활성화
    // Debug.Log("pref1이 생성되었습니다!");

    // ✅ pref1 내부의 UI 요소 찾기
    Transform recipeNametrans = panel.transform.Find("recipeNameText");
    Transform ingredientsTrans = panel.transform.Find("ingredientsText");

    if (recipeNametrans == null || ingredientsTrans == null)
    {
        Debug.LogError("pref1 내부의 UI 요소를 찾을 수 없습니다!");
        return;
    }

    TextMeshProUGUI recipeName = recipeNametrans.GetComponent<TextMeshProUGUI>();
    TextMeshProUGUI ingredientsName = ingredientsTrans.GetComponent<TextMeshProUGUI>();

    if (recipeName == null || ingredientsName == null)
    {
        Debug.LogError("pref1의 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다!");
        return;
    }

    // ✅ UI에 데이터 적용
    recipeName.text = key;  // 제품명 설정

    RecipeB RealRecipe = recipes.FirstOrDefault(recipe => recipe.category == key);

    if (RealRecipe != null)
    {
        ingredientsName.text = "재료: " + string.Join(", ", RealRecipe.ingredients);
    }
    else
    {
        ingredientsName.text = "재료 없음";
        Debug.LogWarning($"[{key}] 카테고리의 레시피를 찾을 수 없습니다!");
    }

    ShowBakes(key);

    RecipePagePanel.SetActive(true);
    panel.SetActive(true);
}


private List<GameObject> activeBakes = new List<GameObject>();  // 현재 생성된 오브젝트 리스트
private List<RecipeB> recipe_h = new List<RecipeB>();

private void ShowBakes(string key)
{
    // ✅ 현재 생성된 개수가 4개 이상이면 생성 중단
    if (activeBakes.Count >= 4)
    {
        Debug.LogWarning("최대 4개까지만 생성할 수 있습니다!");
        return;
    }

    // ✅ key(category)와 같은 menu를 가진 레시피 리스트 추출
    List<RecipeB> filteredRecipes = recipes.Where(recipe => recipe.category == key).ToList();

    if (filteredRecipes.Count == 0)
    {
        Debug.LogWarning($"[{key}] 카테고리에 해당하는 메뉴가 없습니다!");
        return;
    }

    // 🔍 필터링된 레시피 디버그 출력
    Debug.Log($"[{key}] 카테고리에서 필터링된 메뉴 개수: {filteredRecipes.Count}");
    foreach (var r in filteredRecipes)
    {
        Debug.Log($"메뉴: {r.menu}, 코인: {r.coin}");
    }

    // ✅ 4개까지만 추가되도록 설정
    int itemsToCreate = Mathf.Min(filteredRecipes.Count, 4 - activeBakes.Count);

    for (int i = 0; i < itemsToCreate; i++)
    {
        RecipeB recipe = filteredRecipes[i];

        // ✅ `pref2`를 동적으로 생성하여 `panel`의 자식으로 추가
        GameObject newPref = Instantiate(pref2, panel.transform);

        // ✅ UI 요소 찾기
        Transform nameTrans = newPref.transform.Find("bakeName");
        Transform coinTrans = newPref.transform.Find("Money");
        Transform MenuImage = newPref.transform.Find("bakeImage");

        if (nameTrans == null || coinTrans == null)
        {
            Debug.LogError("pref2 내부의 UI 요소를 찾을 수 없습니다! (bakeName, Money 확인)");
            continue;
        }

        TextMeshProUGUI nameText = nameTrans.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI coinText = coinTrans.GetComponent<TextMeshProUGUI>();
        Image menuImg = MenuImage.GetComponent<Image>();

        if (nameText == null || coinText == null)
        {
            Debug.LogError("TextMeshProUGUI 컴포넌트를 찾을 수 없습니다!");
            continue;
        }

        // ✅ UI에 데이터 적용 (menu → bakeName, coin → Money)
        nameText.text = $"{recipe.menu}";
        coinText.text = $"{recipe.coin}";
        menuImg.GetComponent<Image>().sprite = MenuSprites[recipe.index];

        // 🔍 생성된 메뉴 및 코인 정보 디버그 출력
        Debug.Log($"생성된 메뉴: {recipe.menu}, 코인: {recipe.coin}");

        // ✅ y 좌표 계산 (160부터 시작, 110씩 감소)
        int count = activeBakes.Count;
        float posY = 160 - (count * 110);

        // ✅ RectTransform을 사용하여 UI 위치 조정
        RectTransform rectTransform = newPref.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(0, posY);  // X는 고정(0), Y는 순서에 따라 배치
        }
        else
        {
            Debug.LogError("pref2에 RectTransform이 없습니다! UI 요소인지 확인하세요.");
        }

        newPref.SetActive(true);  // 활성화
        activeBakes.Add(newPref);  // 생성된 오브젝트 리스트에 추가
    }

    // 🔍 최종 생성된 오브젝트 수 출력
    Debug.Log($"현재 생성된 오브젝트 수: {activeBakes.Count}");
}




    //레시피 페이지로 이동
//     void ShowRecipePage(string category, List<RecipeB> categoryRecipes)
// {
//     TableOfContentsPanel.SetActive(false);
//     RecipePagePanel.SetActive(true);
//     isTableOfContentsPage = false;

//     recipeNameText.text = $"[{category}] 레시피 목록";

//     // 기존 레시피 항목 제거
//     foreach (Transform child in buttonContainer)
//     {
//         Destroy(child.gameObject);
//     }

//     for (int i = 0; i < categoryRecipes.Count; i++)
//     {
//         RecipeB recipe = categoryRecipes[i];
//         GameObject newRecipeEntry = Instantiate(recipeEntryPrefab, buttonContainer);
//         TMP_Text[] textComponents = newRecipeEntry.GetComponentsInChildren<TMP_Text>();

//         if (textComponents.Length >= 3)
//         {
//             textComponents[0].text = $"<b>{recipe.menu}</b>";  // 제품명

//             if (i == 0) // 카테고리의 첫 번째 레시피만 재료 표시
//             {
//                 textComponents[1].text = $"재료: {string.Join(", ", recipe.ingredients)}";  
//             }
//             else
//             {
//                 textComponents[1].text = ""; // 나머지는 빈 칸
//             }

//             textComponents[2].text = $"가격: ???";  // 가격 (데이터가 있으면 여기에 넣으면 됨)
//         }
//     }

//     // RecipeContentIndex 버튼 클릭 시 호출되는 메서드
//     void OnRecipeContentIndexClicked()
//     {
//         RecipePagePanel.SetActive(false);
//         TableOfContentsPanel.SetActive(true);
//         SetButtonToFront(RecipeContentIndex);

//         // 현재 페이지가 목차 페이지임을 표시
//         isTableOfContentsPage = true;
//     }

    //     // ✅ NEXT 버튼 클릭 시 다음 카테고리의 레시피 페이지로 이동
    // void OnNextClicked()
    // {
    //     if (currentCategoryIndex < categoryList.Count - 1)
    //     {
    //         currentCategoryIndex++;
    //         ShowRecipePage(categoryList[currentCategoryIndex]);
    //     }
    // }

    // // ✅ PREV 버튼 클릭 시 이전 카테고리의 레시피 페이지로 이동
    // void OnPrevClicked()
    // {
    //     if (currentCategoryIndex > 0)
    //     {
    //         currentCategoryIndex--;
    //         ShowRecipePage(categoryList[currentCategoryIndex]);
    //     }
    // }

    // // ✅ 페이지 이동 버튼 활성화/비활성화
    // void UpdateNavigationButtons()
    // {
    //     PrevButton.gameObject.SetActive(currentCategoryIndex > 0);
    //     NextButton.gameObject.SetActive(currentCategoryIndex < categoryList.Count - 1);
    // }

    

    // 버튼을 앞쪽으로 보내기
    private void SetButtonToFront(Button button)
    {
        button.transform.SetAsLastSibling();
    }

    // 버튼을 뒤로 보내기
    private void SetButtonToBack(Button button)
    {
        button.transform.SetAsFirstSibling();
    }
}
