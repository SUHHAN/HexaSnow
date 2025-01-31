using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class ToppingManager : MonoBehaviour
{
    // 토핑 패널
    public GameObject toppingPanel;
    public GameObject startToppingPanel; // 토핑 시작 패널
    public GameObject addToppingPanel; // 토핑 선택 패널
    public GameObject finishBakingPanel; // 베이킹 완료 패널

    // 버튼
    public Button startToppingButton; // 토핑 시작 버튼
    public Button finishToppingButton; // 토핑 완료 버튼

    // 기타 참조
    public InventoryManager inventoryManager; // 인벤토리 매니저
    public OvenGameManager ovenGameManager; // 오븐 게임 매니저
    public Image bakingImage; // 최종 베이킹 이미지
    public Image oriImage1; // 원본 이미지 1
    public Image oriImage2; // 원본 이미지 2

    public BakingStartManager bakingStartManager; // 베이킹 시작 매니저

    private string selectedTopping = null; // 선택된 토핑
    private Dictionary<int, string> menuDictionary = new Dictionary<int, string>(); // 메뉴 데이터를 저장하는 딕셔너리
    private Dictionary<string, int> priceTable = new Dictionary<string, int>() // 디저트 가격 정보
    {
        { "마들렌", 2000 }, { "머핀", 2500 }, { "쿠키", 1700 }, { "파운드케이크", 3000 },
        { "케이크", 5000 }, { "도넛", 4000 }, { "바스크 치즈케이크", 5000 }, { "휘낭시에", 2200 },
        { "스콘", 2500 }, { "타르트", 4000 }, { "마카롱", 2300 }
    };

    void Start()
    {
        LoadRecipeCSV(); // 레시피 데이터를 CSV에서 불러오기

        startToppingPanel.SetActive(true); // 토핑 시작 패널 활성화
        addToppingPanel.SetActive(false); // 토핑 선택 패널 비활성화
        finishBakingPanel.SetActive(false); // 베이킹 완료 패널 비활성화

        startToppingButton.onClick.AddListener(OpenToppingSelection); // 토핑 시작 버튼 이벤트 등록
        finishToppingButton.onClick.AddListener(FinishToppingSelection); // 토핑 완료 버튼 이벤트 등록

        UpdateToppingButtons(); // 토핑 버튼 업데이트
        SetOriginalImages(); // 선택한 디저트의 원본 이미지 설정
    }

    // CSV에서 레시피 데이터를 불러오는 함수
    private void LoadRecipeCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("recipe"); // Resources 폴더에서 "recipe.csv" 불러오기
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: recipe.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n'); // CSV 파일을 줄 단위로 나누기
        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(헤더) 제외하고 읽기
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length < 2) continue; // 데이터 부족하면 무시

            int id;
            if (int.TryParse(fields[0].Trim(), out id)) // 첫 번째 열은 메뉴 ID
            {
                string menuName = fields[1].Trim(); // 두 번째 열은 메뉴 이름
                menuDictionary[id] = menuName; // 딕셔너리에 저장
            }
        }
        Debug.Log($"CSV에서 {menuDictionary.Count}개의 메뉴 데이터를 불러왔습니다.");
    }

    // 토핑 선택 패널 열기
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false); // 시작 패널 비활성화
        addToppingPanel.SetActive(true); // 토핑 선택 패널 활성화
        UpdateToppingButtons(); // 토핑 버튼 상태 업데이트
    }

    // 소지한 토핑 버튼 활성화
    private void UpdateToppingButtons()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager가 할당되지 않았습니다!");
            return;
        }

        inventoryManager.UpdateRefrigeratorButtons(); // 인벤토리에서 재료 업데이트

        foreach (GameObject buttonObj in inventoryManager.refrigeratorButtons)
        {
            string toppingName = buttonObj.name.Replace("Button", ""); // 버튼 이름에서 "Button" 제거하여 토핑 이름 추출
            bool hasTopping = inventoryManager.HasIngredient(toppingName); // 해당 토핑이 있는지 확인

            buttonObj.SetActive(hasTopping); // 보유한 토핑만 버튼 활성화
            if (hasTopping)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSingleTopping(buttonObj, toppingName)); // 클릭 이벤트 등록
            }
        }
    }

    // 토핑 하나만 선택 가능하도록 설정
    private void SelectSingleTopping(GameObject buttonObj, string toppingName)
    {
        if (selectedTopping != null) // 이전에 선택한 토핑이 있다면 선택 해제
        {
            foreach (GameObject btn in inventoryManager.refrigeratorButtons)
            {
                if (btn.name.Replace("Button", "") == selectedTopping)
                {
                    Image prevImageAfter = btn.transform.Find("Imageaft").GetComponent<Image>();
                    prevImageAfter.color = new Color(1f, 1f, 1f, 1f); // 원래 불투명 상태로 변경
                    break;
                }
            }
        }

        if (selectedTopping == toppingName) // 같은 버튼을 다시 누르면 선택 해제
        {
            selectedTopping = null;
        }
        else // 새로운 토핑 선택
        {
            selectedTopping = toppingName;
            Image imageAfter = buttonObj.transform.Find("Imageaft").GetComponent<Image>();
            imageAfter.color = new Color(1f, 1f, 1f, 0.3f); // 투명도 조절
        }
    }

    // 토핑 선택 완료
    private void FinishToppingSelection()
    {
        addToppingPanel.SetActive(false); // 토핑 선택 패널 비활성화
        finishBakingPanel.SetActive(true); // 베이킹 완료 패널 활성화
        UpdateBakingImage(); // 최종 이미지 업데이트
        SaveBakingResult(); // 결과 저장

        SceneManager.LoadScene("Bonus"); // 보너스 씬으로 이동
    }

    // 선택한 디저트의 원본 이미지 설정
    private void SetOriginalImages()
    {
        string selectedDessert = bakingStartManager.GetSelectedDessert();
        string originalImagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_original";

        Sprite originalSprite = Resources.Load<Sprite>(originalImagePath);
        if (originalSprite != null)
        {
            oriImage1.sprite = originalSprite;
            oriImage2.sprite = originalSprite;
        }
    }

    // 선택한 디저트와 토핑을 반영하여 최종 이미지 업데이트
    private void UpdateBakingImage()
    {
        string selectedDessert = bakingStartManager.GetSelectedDessert();
        string imagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_{(selectedTopping ?? "original").ToLower()}";

        Sprite newSprite = Resources.Load<Sprite>(imagePath);
        if (newSprite != null)
        {
            bakingImage.sprite = newSprite;
        }
    }

    // 베이킹 결과 저장
    private void SaveBakingResult()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        string dessertName = bakingStartManager.GetSelectedDessert();
        int totalScore = ovenGameManager.GetTotalScore(); // 최종 점수 가져오기
        Debug.Log($"최종 총점: {totalScore}");

        bool isBonusGame = false; // 보너스 여부는 보너스 씬에서 결정됨

        int menuID = GetMenuID(dessertName);
        if (menuID == -1)
        {
            Debug.LogError($"메뉴 ID를 찾을 수 없습니다: {dessertName}");
            return;
        }

        string finalName = selectedTopping != null ? $"{selectedTopping} {dessertName}" : $"오리지널 {dessertName}";

        MyRecipeList newRecipe = new MyRecipeList(
            DataManager.Instance.gameData.myBake.Count + 1,
            menuID,
            finalName,
            totalScore,
            isBonusGame
        );

        DataManager.Instance.gameData.myBake.Add(newRecipe);
        DataManager.Instance.SaveGameData();

        Debug.Log($"저장 완료: {finalName} | 점수: {totalScore} | 보너스 여부: {isBonusGame}");
    }

    // 디저트 이름으로 메뉴 ID 가져오기
    private int GetMenuID(string dessertName)
    {
        foreach (var entry in menuDictionary)
        {
            if (entry.Value == dessertName)
            {
                return entry.Key;
            }
        }
        return -1;
    }
}
