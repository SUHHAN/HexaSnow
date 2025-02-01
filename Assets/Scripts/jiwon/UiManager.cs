using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가

public class UiManager : MonoBehaviour
{
    public GameObject OrderBook;  // OrderBook 패널
    public Button order_button;    // Order 버튼

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
    }

    void OnRecipeBook()
    {
        RecipeBook.SetActive(!RecipeBook.activeSelf);
    }

    public void LoadCalendarDate()
    {
        // 🛠️ DataManager.Instance가 null인지 확인
        if (DataManager.Instance == null)
        {
            Debug.LogError("❌ DataManager.Instance가 null입니다! DataManager가 씬에 존재하는지 확인하세요.");
            return;
        }

        // 🛠️ LoadGameData()를 통해 GameData 가져오기
        GameData dateGD = DataManager.Instance.LoadGameData();

        // 🛠️ GameData가 null인지 확인
        if (dateGD == null)
        {
            Debug.LogError("❌ GameData가 null입니다! LoadGameData()가 데이터를 정상적으로 반환하는지 확인하세요.");
            return;
        }

        // 🛠️ calendarText가 null인지 확인
        if (calendarText == null)
        {
            Debug.LogError("❌ calendarText가 설정되지 않았습니다! Unity Inspector에서 확인하세요.");
            return;
        }

        // 🛠️ 정상적으로 값이 있으면 텍스트 설정
        calendarText.text = $"{dateGD.date}일차";
        Debug.Log($"✅ Calendar updated with date: {dateGD.date}일차");

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
}
