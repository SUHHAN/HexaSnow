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
    public GameObject startToppingPanel;
    public GameObject addToppingPanel;
    public GameObject finishBakingPanel;

    // 버튼
    public Button startToppingButton;
    public Button finishToppingButton;
    public Button finishButton;

    // 기타 참조
    public InventoryManager inventoryManager;
    public OvenGameManager ovenGameManager;
    public Image bakingImage;
    public Image oriImage1;
    public Image oriImage2;

    public BakingStartManager bakingStartManager;

    private int selectedToppingIndex = -1; // int 타입으로 변경하여 인덱스로 관리
    private Dictionary<int, string> menuDictionary = new Dictionary<int, string>();

    void Start()
    {
        LoadRecipeCSV();

        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(false);

        startToppingButton.onClick.AddListener(OpenToppingSelection);
        finishToppingButton.onClick.AddListener(FinishToppingSelection);
        finishButton.onClick.AddListener(FinishBaking);

        UpdateToppingButtons();
        SetOriginalImages();
    }

    // CSV에서 레시피 데이터를 불러오는 함수
    private void LoadRecipeCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("recipe");
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: recipe.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length < 2) continue;

            int id;
            if (int.TryParse(fields[0].Trim(), out id))
            {
                string menuName = fields[1].Trim();
                menuDictionary[id] = menuName;
            }
        }
        Debug.Log($"CSV에서 {menuDictionary.Count}개의 메뉴 데이터를 불러왔습니다.");
    }

    // 토핑 선택 패널 열기
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
        UpdateToppingButtons();
    }

    // 소지한 토핑 버튼 활성화
    private void UpdateToppingButtons()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager가 할당되지 않았습니다!");
            return;
        }

        inventoryManager.UpdateRefrigeratorButtons();

        foreach (GameObject buttonObj in inventoryManager.refrigeratorButtons)
        {
            string toppingEname = buttonObj.name.Replace("Button", ""); // 버튼에서 "Button" 제거하여 eName 추출
            int toppingIndex = inventoryManager.GetIngredientIndexFromEname(toppingEname); // eName → index 변환

            if (toppingIndex == -1) continue; // 없는 재료면 무시

            bool hasTopping = inventoryManager.HasIngredient(toppingIndex);

            buttonObj.SetActive(hasTopping);
            if (hasTopping)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSingleTopping(buttonObj, toppingIndex)); // int 타입으로 변경
            }
        }
    }

    // 토핑 하나만 선택 가능하도록 설정
    private void SelectSingleTopping(GameObject buttonObj, int toppingIndex)
    {
        if (selectedToppingIndex != -1) // 기존 선택된 토핑이 있으면 해제
        {
            foreach (GameObject btn in inventoryManager.refrigeratorButtons)
            {
                string btnEname = btn.name.Replace("Button", "");
                int btnIndex = inventoryManager.GetIngredientIndexFromEname(btnEname);
                if (btnIndex == selectedToppingIndex)
                {
                    Image prevImageAfter = btn.transform.Find("Imageaft").GetComponent<Image>();
                    prevImageAfter.color = new Color(1f, 1f, 1f, 1f);
                    break;
                }
            }
        }

        if (selectedToppingIndex == toppingIndex) // 같은 버튼 다시 누르면 선택 해제
        {
            selectedToppingIndex = -1;
        }
        else // 새로운 토핑 선택
        {
            selectedToppingIndex = toppingIndex;
            Image imageAfter = buttonObj.transform.Find("Imageaft").GetComponent<Image>();
            imageAfter.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    // 토핑 선택 완료
    private void FinishToppingSelection()
    {
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
        UpdateBakingImage();
    }

    private void FinishBaking()
    {
        SaveBakingResult();

        SceneManager.LoadScene("Bonus");
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
        string toppingEname = inventoryManager.GetIngredientEname(selectedToppingIndex); // 인덱스를 eName으로 변환
        string imagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_{(toppingEname ?? "original").ToLower()}";

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
        int totalScore = ovenGameManager.GetTotalScore();
        Debug.Log($"최종 총점: {totalScore}");

        bool isBonusGame = false;

        int menuID = GetMenuID(dessertName);
        if (menuID == -1)
        {
            Debug.LogError($"메뉴 ID를 찾을 수 없습니다: {dessertName}");
            return;
        }

        string finalName = selectedToppingIndex != -1
            ? $"{inventoryManager.GetIngredientEname(selectedToppingIndex)} {dessertName}"
            : $"오리지널 {dessertName}";

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

    // 메뉴 ID를 가져오는 함수 추가
    private int GetMenuID(string dessertName)
    {
        foreach (var entry in menuDictionary)
        {
            if (entry.Value == dessertName)
            {
                return entry.Key; // 해당하는 메뉴 ID 반환
            }
        }
        Debug.LogError($"GetMenuID: '{dessertName}'에 해당하는 메뉴 ID를 찾을 수 없습니다.");
        return -1; // 찾지 못한 경우 -1 반환
    }

}
