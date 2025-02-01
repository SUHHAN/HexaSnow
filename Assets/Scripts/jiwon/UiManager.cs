using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가

public class UiManager : MonoBehaviour
{
    public GameObject OrderBook;  // OrderBook 패널
    public Button order_button;    // Order 버튼
    public TMP_Text orderText; // 주문서 텍스트

    public GameObject RecipeBook;
    public Button RecipeButton;

    public TMP_Text calendarText;
    public TMP_FontAsset customFont;

    void Start()
    {
        // OrderButton 클릭 이벤트 등록
        order_button.onClick.AddListener(OnOrderBook);
        RecipeButton.onClick.AddListener(OnRecipeBook);
        LoadCalendarDate();

    }

    void OnOrderBook()
    {
        // 현재 OrderBook의 활성화 상태를 반전 (true ↔ false)
        OrderBook.SetActive(!OrderBook.activeSelf);
        if (OrderBook.activeSelf)
        {
            LoadOrderBook();
        }
    }

    void OnRecipeBook()
    {
        RecipeBook.SetActive(!RecipeBook.activeSelf);
    }

    public void LoadCalendarDate()
    {
        // DataManager.Instance가 null인지 확인
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager.Instance가 null입니다! DataManager가 씬에 존재하는지 확인하세요.");
            return;
        }

        // LoadGameData()를 통해 GameData 가져오기
        GameData dateGD = DataManager.Instance.LoadGameData();


        // 정상적으로 값이 있으면 텍스트 설정
        calendarText.text = $"{dateGD.date}일차";
        Debug.Log($"Calendar updated with date: {dateGD.date}일차");

        // 폰트 설정
        if (customFont != null)
        {
            calendarText.font = customFont; // 커스텀 폰트를 설정
        }
        else
        {
            Debug.LogError("폰트가 설정되지 않았습니다! Unity Inspector에서 customFont를 설정하세요.");
        }

        // 텍스트 속성 설정
        calendarText.color = UnityEngine.Color.black;  // 텍스트 색상 설정
        calendarText.fontSize = 30;  // 폰트 크기 설정
    }

    public void LoadOrderBook()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager가 존재하지 않습니다! DataManager가 씬에 있는지 확인하세요.");
            return;
        }

        GameData data = DataManager.Instance.LoadGameData();

        if (orderText == null)
        {
            Debug.LogError("❌ orderText가 설정되지 않았습니다! Unity Inspector에서 설정하세요.");
            return;
        }

        if (data != null && !string.IsNullOrEmpty(data.serializedDailyOrders))
        {
            orderText.text = data.serializedDailyOrders;
            Debug.Log($"📜 주문서 로드 완료: {data.serializedDailyOrders}");
        }
        else
        {
            orderText.text = "📌 주문서가 없습니다.";
            Debug.LogWarning("⚠️ 저장된 주문서가 없습니다.");
        }
    }
}
