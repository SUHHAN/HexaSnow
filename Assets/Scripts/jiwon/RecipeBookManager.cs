using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Drawing;
using System.Linq;

public class RecipeBookManager : MonoBehaviour
{
    // UI 요소
    public TMP_Text recipeNameText;  // 레시피 이름 표시
    public TMP_Text ingredientsText; // 재료 목록 표시
    public TMP_Text doughText;       // 반죽 타입 표시
    public TMP_Text ovenText;        // 오븐 타입 표시
    public TMP_Text toppingText;     // 토핑 표시

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

    public enum DifficultyLevel
    {
        쉬움 = 1,        
        보통 = 2,         
        어려움 = 3,       
        매우어려움 = 4     
    }

    [System.Serializable]
    public class RecipeB
    {
        public int index;                 // 레시피 인덱스
        public string menu;                // 메뉴 이름
        public List<string> ingredients;   // 재료 목록
        public DifficultyLevel dough;      // 반죽 타입
        public DifficultyLevel oven;       // 오븐 타입
        public string topping;             // 토핑 (없을 수도 있음)
        public string category;
        public string subCategory;

        // ToString 메서드 오버라이드
        public override string ToString()
        {
            string ingredientsList = string.Join(", ", ingredients); // 재료 목록을 문자열로 변환
            return $"Index: {index}, Menu: {menu}, Category: {category}, SubCategory: {subCategory}, Ingredients: {ingredientsList}, Dough: {dough}, Oven: {oven}, Topping: {topping}";
        }
    }

    void Start()
    {
        // CSV 파일 로드
        LoadRecipesFromCSV("Assets/Resources/recipe.csv");

        // UI 초기화
        DisplayTableOfContents();

        // 버튼 이벤트 연결
        NextButton.onClick.AddListener(OnNextClicked);
        PrevButton.onClick.AddListener(OnPrevClicked);
        RecipeContentIndex.onClick.AddListener(OnRecipeContentIndexClicked);
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
            if (columns.Length < 5)
            {
                Debug.LogWarning($"잘못된 CSV 데이터: {line}");
                continue;
            }

            int index, doughType, ovenType;

            // 안전한 정수 변환 (TryParse 사용)
            if (!int.TryParse(columns[0], out index)) index = 0;
            if (!int.TryParse(columns[3], out doughType)) doughType = 0;
            if (!int.TryParse(columns[4], out ovenType)) ovenType = 0;

            DifficultyLevel doughLevel = (DifficultyLevel)doughType;  // dough는 DifficultyLevel으로 변환
            DifficultyLevel ovenLevel = (DifficultyLevel)ovenType;

            string menu = columns[1];
            string topping = columns.Length > 5 ? columns[5].Trim() : "";  // Topping이 없으면 빈 문자열

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
                dough = doughLevel,
                oven = ovenLevel,
                topping = topping,
                category = category,
                subCategory = subCategory
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
                ShowRecipePageForCategory(group);  // 해당 카테고리에 맞는 레시피 페이지로 이동
            });
        }
    }

    // 해당 카테고리의 레시피 페이지를 표시하는 함수
    void ShowRecipePageForCategory(IGrouping<string, RecipeB> group)
    {
        RecipeB firstRecipe = group.First();

        ShowRecipePage(firstRecipe);
    }




    // 레시피 페이지로 이동
    void ShowRecipePage(RecipeB selectedRecipe)
    {
        TableOfContentsPanel.SetActive(false);
        RecipePagePanel.SetActive(true);
        isTableOfContentsPage = false;
        SetButtonToBack(RecipeContentIndex);

        // 레시피 정보 표시
        recipeNameText.text = selectedRecipe.menu;
        ingredientsText.text = "재료: " + string.Join(", ", selectedRecipe.ingredients);
        doughText.text = "반죽 타입: " + selectedRecipe.dough;
        ovenText.text = "오븐 타입: " + selectedRecipe.oven;
        toppingText.text = string.IsNullOrEmpty(selectedRecipe.topping) ? "토핑 없음" : "토핑: " + selectedRecipe.topping;

        UpdateNavigationButtons();
    }

    // RecipeContentIndex 버튼 클릭 시 호출되는 메서드
    void OnRecipeContentIndexClicked()
    {
        RecipePagePanel.SetActive(false);
        TableOfContentsPanel.SetActive(true);
        SetButtonToFront(RecipeContentIndex);

        // 현재 페이지가 목차 페이지임을 표시
        isTableOfContentsPage = true;
    }

    // NEXT 버튼 클릭 시 동작
    void OnNextClicked()
    {
        if (isTableOfContentsPage)
        {
            ShowRecipePage(recipes[0]);
        }
        else
        {
            if (currentRecipeIndex < recipes.Count - 1)
            {
                currentRecipeIndex++;
                ShowRecipePage(recipes[currentRecipeIndex]);
            }
            else
            {
                Debug.Log("마지막 레시피입니다.");
            }
        }
    }

    void OnPrevClicked()
    {
        if (!isTableOfContentsPage)
        {
            if (currentRecipeIndex > 0)
            {
                currentRecipeIndex--;
                ShowRecipePage(recipes[currentRecipeIndex]);
            }
            else
            {
                DisplayTableOfContents(); // 첫 번째 레시피에서 다시 목차로 돌아감
            }
        }
    }

    void UpdateNavigationButtons()
    {
        PrevButton.gameObject.SetActive(!isTableOfContentsPage); // 목차 페이지에서는 비활성화
        NextButton.gameObject.SetActive(true); // 항상 활성화
    }

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
