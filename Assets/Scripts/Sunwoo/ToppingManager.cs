using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToppingManager : MonoBehaviour
{
    public GameObject toppingPanel; // Topping 전체 패널
    public GameObject startToppingPanel; // 시작 패널
    public GameObject addToppingPanel; // 토핑 선택 패널
    public GameObject resultPanel; // 결과 패널

    public Button startToppingButton; // 토핑 시작 버튼
    public Button finishToppingButton; // 완료 버튼

    public InventoryManager inventoryManager; // 인벤토리 관리

    // 토핑 버튼 미리 할당 (Inspector에서 설정)
    public List<GameObject> refrigeratorButtons;

    private string selectedTopping = null; // 선택된 토핑 (한 개만 가능)

    void Start()
    {
        // 초기 UI 설정
        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        resultPanel.SetActive(false);

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

    // **소지한 토핑만 버튼 활성화**
    private void UpdateToppingButtons()
    {
        Debug.Log("토핑 버튼 업데이트 시작");

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
                    Transform prevImageBefore = btn.transform.Find("Imagebf");
                    Transform prevImageAfter = btn.transform.Find("Imageaft");
                    prevImageBefore.gameObject.SetActive(true);
                    prevImageAfter.gameObject.SetActive(false);
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
            Transform imageBefore = buttonObj.transform.Find("Imagebf");
            Transform imageAfter = buttonObj.transform.Find("Imageaft");

            imageBefore.gameObject.SetActive(false);
            imageAfter.gameObject.SetActive(true);
        }
    }

    // 완료 버튼 클릭 시 실행
    private void FinishToppingSelection()
    {
        Debug.Log($"선택된 토핑: {selectedTopping ?? "없음"}");
        toppingPanel.SetActive(false);
        resultPanel.SetActive(true);
    }
}
