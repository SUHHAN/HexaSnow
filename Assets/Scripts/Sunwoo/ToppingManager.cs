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

    private int selectedDessertIndex = 1; // 기본값 1
    private int selectedToppingIndex = -1; // 선택한 토핑 인덱스 (-1은 선택 안 함)
    private int finalImageIndex = 1; // 최종 결과 이미지 인덱스
    private string selectedDessert;

    public BakingStartManager bakingStartManager;

    // 토핑 버튼 리스트 (Inspector에서 할당)
    public List<GameObject> toppingButtons;

    // 디저트 인덱스에 맞는 이미지 저장
    public List<Sprite> dessertSprites;

    // 디저트 별 활성화할 토핑 버튼
    private Dictionary<int, List<int>> dessertToppingMap = new Dictionary<int, List<int>>()
    {
        { 1, new List<int> { 4, 5 } }, // Madeleine: 토핑 4, 5만 활성화
        { 4, new List<int> { 4, 6 } }, // Cookie: 토핑 4, 6만 활성화
        { 7, new List<int> { 2, 4 } }, // Muffin: 토핑 2, 4만 활성화
        { 10, new List<int> { 1, 4, 5 } }, // 파운드케이크 (토핑 1, 4, 5 활성화)
        { 14, new List<int>() }        // 바스크 치즈케이크
    };

    // 토핑 선택에 따른 BakingImage 변경 (디저트 인덱스, 토핑 인덱스 → 결과 이미지 인덱스)
    private Dictionary<(int, int), int> toppingImageMap = new Dictionary<(int, int), int>()
    {
        { (1, 4), 2 }, { (1, 5), 3 }, // Madeleine → 2: 토핑4, 3: 토핑5
        { (4, 4), 5 }, { (4, 6), 6 }, // Cookie → 5: 토핑4, 6: 토핑6
        { (7, 2), 8 }, { (7, 4), 9 }, // Muffin → 8: 토핑2, 9: 토핑4
        { (10, 4), 11 }, { (10, 5), 12 }, { (10, 1), 13 } // 파운드케이크
    };

    void Start()
    {
        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(false);

        startToppingButton.onClick.AddListener(OpenToppingSelection);
        finishToppingButton.onClick.AddListener(FinishToppingSelection);
        finishButton.onClick.AddListener(FinishBaking);

        UpdateToppingButtons();
        SetOriginalImages();
    }

    public void SetSelectedDessert(string dessertName, int index)
    {
        selectedDessert = dessertName;
        selectedDessertIndex = index;
        selectedToppingIndex = -1; // 토핑 선택 초기화
        finalImageIndex = index; // 기본적으로 원본 이미지 설정
        SetOriginalImages();
        UpdateToppingButtons();
    }

    private void SetOriginalImages()
    {
        if (selectedDessertIndex < dessertSprites.Count)
        {
            oriImage1.sprite = dessertSprites[selectedDessertIndex];
            oriImage2.sprite = dessertSprites[selectedDessertIndex];
            bakingImage.sprite = dessertSprites[selectedDessertIndex]; // 기본 이미지 설정
        }
        else
        {
            Debug.LogError($"SetOriginalImages: 인덱스 {selectedDessertIndex}에 해당하는 이미지가 없습니다.");
        }
    }

    // 소지한 토핑 버튼 활성화
    private void UpdateToppingButtons()
    {
        foreach (GameObject button in toppingButtons)
        {
            button.SetActive(false); // 모든 버튼 비활성화
            SetButtonOpacity(button, 1f); // 모든 버튼 투명도 초기화
        }

        if (dessertToppingMap.ContainsKey(selectedDessertIndex))
        {
            foreach (int toppingIndex in dessertToppingMap[selectedDessertIndex])
            {
                if (toppingIndex - 1 < toppingButtons.Count)
                {
                    toppingButtons[toppingIndex - 1].SetActive(true);
                    int index = toppingIndex; // 클로저 문제 방지
                    toppingButtons[toppingIndex - 1].GetComponent<Button>().onClick.RemoveAllListeners();
                    toppingButtons[toppingIndex - 1].GetComponent<Button>().onClick.AddListener(() => SelectSingleTopping(index));
                }
            }
        }
    }

    // 토핑 하나만 선택 가능하도록 설정
    private void SelectSingleTopping(int toppingIndex)
    {
        if (selectedToppingIndex == toppingIndex)
        {
            selectedToppingIndex = -1; // 같은 토핑 다시 누르면 선택 해제
            finalImageIndex = selectedDessertIndex; // 원래 디저트 이미지로 되돌림
        }
        else
        {
            selectedToppingIndex = toppingIndex;
            if (toppingImageMap.ContainsKey((selectedDessertIndex, toppingIndex)))
            {
                finalImageIndex = toppingImageMap[(selectedDessertIndex, toppingIndex)];
            }
        }

        UpdateBakingImage();
        UpdateToppingButtonOpacity();
    }

    // 선택한 디저트와 토핑을 반영하여 최종 이미지 업데이트
    private void UpdateBakingImage()
    {
        int newImageIndex = selectedDessertIndex;

        if (toppingImageMap.ContainsKey((selectedDessertIndex, selectedToppingIndex)))
        {
            newImageIndex = toppingImageMap[(selectedDessertIndex, selectedToppingIndex)];
        }

        if (newImageIndex < dessertSprites.Count)
        {
            bakingImage.sprite = dessertSprites[newImageIndex];
        }
        else
        {
            Debug.LogError($"UpdateBakingImage: 인덱스 {newImageIndex}에 해당하는 이미지가 없습니다.");
        }
    }

    // 선택한 버튼은 투명해지게
    private void UpdateToppingButtonOpacity()
    {
        foreach (GameObject button in toppingButtons)
        {
            SetButtonOpacity(button, 1f); // 기본값 1 (불투명)
        }

        if (selectedToppingIndex != -1 && selectedToppingIndex - 1 < toppingButtons.Count)
        {
            SetButtonOpacity(toppingButtons[selectedToppingIndex - 1], 0.3f); // 선택한 버튼 투명도 30%
        }
    }

    // 투명도 설정
    private void SetButtonOpacity(GameObject buttonObj, float alpha)
    {
        Image buttonImage = buttonObj.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha;
            buttonImage.color = color;
        }
    }

    // 토핑 선택 완료
    private void FinishToppingSelection()
    {
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
        UpdateBakingImage();
    }

    // 토핑 선택 패널 열기
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
        UpdateToppingButtons();
    }

    private void FinishBaking()
    {
        SaveBakingResult();
        SceneManager.LoadScene("Bonus");
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

        MyRecipeList newRecipe = new MyRecipeList(
            DataManager.Instance.gameData.myBake.Count + 1,
            finalImageIndex, // 저장되는 최종 이미지 인덱스
            dessertName,
            totalScore,
            false
        );

        DataManager.Instance.gameData.myBake.Add(newRecipe);
        DataManager.Instance.SaveGameData();

        Debug.Log($"저장 완료: {dessertName} | 이미지 인덱스: {finalImageIndex} | 점수: {totalScore}");
    }
}
