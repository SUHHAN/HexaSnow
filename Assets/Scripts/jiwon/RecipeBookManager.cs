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

    public Button NextButton;    // NEXT 버튼
    public Button PrevButton;    // PREV 버튼
    public Button RecipeContentIndex; // 목차 돌아가기 버튼

    public GameObject TableOfContentsPanel; // 목차 패널
    public GameObject RecipePagePanel;      // 레시피 페이지 패널
    public Transform buttonContainer;       // 목차 버튼 컨테이너

    public TMP_FontAsset customFont;

    private List<RecipeB> recipes = new List<RecipeB>();
    private bool isTableOfContentsPage = true;
    private bool IsRecipePagePanel = false;

    private List<string> categoryList; // 카테고리 목록
    private string currentCategoryKey;
    private int currentIndex;



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
        UpdateButtonStates();

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

        // 레시피 목록에서 카테고리만 추출하여 고유한 리스트로 저장
        categoryList = recipes.Select(recipe => recipe.category).Distinct().ToList();

        // 카테고리 리스트가 비어있는지 확인
        if (categoryList.Count == 0)
        {
            Debug.LogError("categoryList가 비어있습니다! Recipes 리스트에 카테고리 정보가 없습니다.");
        }
        else
        {
            // categoryList가 비어 있지 않다면, 리스트의 내용을 출력
            Debug.Log("categoryList: " + string.Join(", ", categoryList));
        }


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
                currentCategoryKey = group.Key;
                currentIndex = categoryList.IndexOf(currentCategoryKey);
            });
        }
    }


    public void ShowRecipePage(string key)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
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

        // pref1 내부의 UI 요소 찾기
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

        // UI에 데이터 적용
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
        UpdateButtonStates();
        Debug.Log(RecipePagePanel.activeSelf);
        panel.SetActive(true);
    }


    private List<GameObject> activeBakes = new List<GameObject>();  // 현재 생성된 오브젝트 리스트
    private List<RecipeB> recipe_h = new List<RecipeB>();

    private void ResetBakes()
    {
        // 기존에 생성된 오브젝트 삭제
        foreach (GameObject obj in activeBakes)
        {
            Destroy(obj);
        }
        activeBakes.Clear(); // 리스트 초기화

        Debug.Log("기존의 생성된 오브젝트를 초기화했습니다.");
    }

    private void ShowBakes(string key)
    {
        ResetBakes();
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

    // RecipeContentIndex 버튼 클릭 시 호출되는 메서드
    void OnRecipeContentIndexClicked()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
        RecipePagePanel.SetActive(false);
        TableOfContentsPanel.SetActive(true);
        UpdateButtonStates();
        SetButtonToFront(RecipeContentIndex);

        // 현재 페이지가 목차 페이지임을 표시
        isTableOfContentsPage = true;
    }

    // '다음' 버튼을 클릭했을 때 동작
    void OnNextClicked()
    {
        Debug.Log("클릭");
        Debug.Log(RecipePagePanel.activeSelf);
        // 현재 페이지가 목차 페이지인지 확인 
        if (RecipePagePanel.activeSelf == false)
        {
            // 만약 목차 페이지라면, 첫 번째 카테고리로 이동
            if (categoryList.Count > 0)
            {
                // 첫 번째 카테고리 (인덱스 0)로 이동
                string currentCategory = categoryList[0];

                // 현재 카테고리 키를 첫 번째 카테고리로 업데이트
                currentCategoryKey = currentCategory;

                // 첫 번째 카테고리에 해당하는 레시피 페이지를 보여줌
                ShowRecipePage(currentCategory);  // ShowRecipePage 함수 호출하여 첫 번째 카테고리로 레시피 페이지 표시

                // 버튼 상태를 업데이트
                UpdateButtonStates();

                // 디버그 로그로 이동된 카테고리를 출력
                Debug.Log("첫 번째 카테고리로 이동: " + currentCategory);
                Debug.Log(currentIndex);
            }
            else
            {
                // 카테고리 리스트가 비어있을 경우 에러 로그 출력
                Debug.LogError("카테고리 리스트가 비어있습니다!");
                Debug.Log("categoryList: " + string.Join(", ", categoryList));
            }
        }
        else
        {
            // 카테고리 리스트에서 마지막이 아니면, 다음 카테고리로 이동
            if (currentIndex < categoryList.Count - 1)
            {   
                currentIndex = currentIndex + 1;
                // 다음 카테고리의 키를 가져옴
                string currentCategory = categoryList[currentIndex];
                Debug.Log(currentCategory);
                Debug.Log(currentIndex);

                // 다음 카테고리에 해당하는 레시피 페이지를 보여줌
                ShowRecipePage(currentCategory);  // ShowRecipePage 함수 호출하여 다음 카테고리로 레시피 페이지 표시

                // 현재 카테고리 키를 다음 카테고리로 업데이트
                currentCategoryKey = currentCategory;

                // 버튼 상태를 업데이트
                UpdateButtonStates();

                // 디버그 로그로 이동된 카테고리를 출력
                Debug.Log("다음 카테고리로 이동: " + currentCategory);
            }
            else
            {
                // 마지막 카테고리일 경우, 더 이상 이동할 수 없다는 로그 출력
                Debug.Log("마지막 카테고리입니다. 더 이상 이동할 수 없습니다.");
            }
        }
    }

    // '이전' 버튼 클릭 시 동작
    void OnPrevClicked()
    {
        // 카테고리 리스트에서 첫 번째 카테고리가 아니면 이전 카테고리로 이동합니다.
        if (currentIndex > 0)
        {
            currentIndex = currentIndex - 1;
            // 이전 카테고리 키를 가져옵니다.
            string currentCategory = categoryList[currentIndex];

            // 이전 카테고리에 해당하는 레시피 페이지를 표시합니다.
            ShowRecipePage(currentCategory);

            // currentCategoryKey를 이전 카테고리로 업데이트합니다.
            currentCategoryKey = currentCategory;

            // 디버그 로그로 이동된 카테고리를 출력
            Debug.Log("이전 카테고리로 이동: " + currentCategory);
            UpdateButtonStates();  // 버튼 상태 업데이트
        }
        else
        {
            RecipePagePanel.SetActive(false);
            UpdateButtonStates();
            Debug.Log(RecipePagePanel.activeSelf);
            Debug.Log("목차 페이지로 이동");
        }
    }

    // 버튼의 상태(활성화/비활성화)를 업데이트하는 함수
    void UpdateButtonStates()
    {
        // 목차 페이지에 있는 경우 '이전' 버튼을 비활성화
        PrevButton.interactable = RecipePagePanel.activeSelf;

        // 마지막 카테고리일 경우 '다음' 버튼을 비활성화
        NextButton.interactable = currentIndex < categoryList.Count - 1;
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