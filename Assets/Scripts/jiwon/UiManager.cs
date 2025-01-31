using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가

public class UiManager : MonoBehaviour
{
    public GameObject OrderBook;  // OrderBook 패널
    public Button order_button;    // Order 버튼

    public GameObject RecipeBook;
    public Button RecipeButton;

    void Start()
    {
        // OrderButton 클릭 이벤트 등록
        order_button.onClick.AddListener(OnOrderBook);
        RecipeButton.onClick.AddListener(OnRecipeBook);
        
    }

    void OnOrderBook()
    {
        // 현재 OrderBook의 활성화 상태를 반전 (true ↔ false)
        OrderBook.SetActive(!OrderBook.activeSelf);
    }

    void OnRecipeBook()
    {
        RecipeBook.SetActive(!RecipeBook.activeSelf);
    }
}
