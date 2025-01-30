using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToppingManager : MonoBehaviour
{
    public GameObject toppingPanel; // Topping 전체 패널
    public GameObject startToppingPanel; // 시작 패널
    public GameObject addToppingPanel; // 토핑 선택 패널
    public GameObject finishBakingPanel; // 결과 패널

    public Button startToppingButton; // 토핑 시작 버튼
    public Button finishToppingButton; // 완료 버튼

    public InventoryManager inventoryManager; // 인벤토리 관리
    public Image bakingImage;

    // 토핑 버튼 미리 할당
    public List<GameObject> refrigeratorButtons;

    private string selectedTopping = null; // 선택된 토핑

    public BakingStartManager bakingStartManager; // BakingStartManager 참조

    void Start()
    {
        // 초기 UI 설정
        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(false);

        // 버튼 이벤트 등록
        startToppingButton.onClick.AddListener(OpenToppingSelection);
        finishToppingButton.onClick.AddListener(FinishToppingSelection);

        UpdateToppingButtons(); // 소지한 재료만 활성화
    }

    // StartToppingButton 클릭 시 실행
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
        UpdateToppingButtons(); // 다시 업데이트
    }

    // 소지한 토핑만 버튼 활성화
    private void UpdateToppingButtons()
    {
        foreach (GameObject buttonObj in refrigeratorButtons)
        {
            string toppingName = buttonObj.name.Replace("Button", ""); // "BananaButton" -> "Banana"
            bool hasTopping = inventoryManager.HasIngredient(toppingName);

            buttonObj.SetActive(hasTopping);
            Debug.Log($"토핑: {toppingName}, 소지 여부: {hasTopping}");

            if (hasTopping)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSingleTopping(buttonObj, toppingName));
            }
        }
    }

    // 토핑 버튼 클릭 시, 하나의 토핑만 선택 가능하게 설정
    private void SelectSingleTopping(GameObject buttonObj, string toppingName)
    {
        // 기존 선택된 토핑 해제
        if (selectedTopping != null)
        {
            foreach (GameObject btn in refrigeratorButtons)
            {
                if (btn.name.Replace("Button", "") == selectedTopping)
                {
                    Image prevImageAfter = btn.transform.Find("Imageaft").GetComponent<Image>();
                    prevImageAfter.color = new Color(1f, 1f, 1f, 1f);
                    break;
                }
            }
        }

        // 새롭게 선택한 토핑 적용
        if (selectedTopping == toppingName)
        {
            // 같은 버튼을 다시 눌렀다면 선택 해제
            selectedTopping = null;
        }
        else
        {
            selectedTopping = toppingName;
            Image imageAfter = buttonObj.transform.Find("Imageaft").GetComponent<Image>();
            imageAfter.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    // 완료 버튼 클릭 시 실행
    private void FinishToppingSelection()
    {
        Debug.Log($"선택된 토핑: {selectedTopping ?? "없음"}");
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
        UpdateBakingImage();
    }

    private void UpdateBakingImage()
    {
        string selectedDessert = bakingStartManager.GetSelectedDessert();
        string imagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_{(selectedTopping ?? "original").ToLower()}";
        bakingImage.sprite = Resources.Load<Sprite>(imagePath);
    }
}
