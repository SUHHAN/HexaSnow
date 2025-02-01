using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가
using System.Collections.Generic;
using System;

public class UiManager : MonoBehaviour
{
    public GameObject OrderBook;  // OrderBook 패널
    public Button order_button;    // Order 버튼
    public TMP_Text OrderContent; // 주문서 내용
    public TMP_Text OrderCustomer; // 주문서 고객

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
            Debug.LogError("DataManager가 존재하지 않습니다! DataManager가 씬에 있는지 확인하세요.");
            return;
        }

        GameData data = DataManager.Instance.LoadGameData();

        if (OrderContent == null || OrderCustomer == null)
        {
            Debug.LogError("OrderContent 또는 OrderCustomer가 설정되지 않았습니다! Unity Inspector에서 설정하세요.");
            return;
        }

        if (data != null && !string.IsNullOrEmpty(data.serializedDailyOrders))
        {
            // 주문서 데이터를 파싱하기
            try
            {
                // Content와 Customer를 각각 파싱
                var orderPairs = ParseOrderPairs(data.serializedDailyOrders);

                OrderContent.text = "";
                OrderCustomer.text = "";

             
                List<int> contentList = new List<int>();
                List<int> customerList = new List<int>();

                foreach (var pair in orderPairs)
                {
                    contentList.Add(pair.Item1);
                    customerList.Add(pair.Item2);
                }

                for (int i = 0; i < contentList.Count; i++)
                {

                    OrderContent.text += $"Content: {contentList[i]}\n";
                    OrderCustomer.text += $"Customer: {customerList[i]}\n";
                }

                if (customFont != null) 
                {
                    OrderContent.font = customFont;  
                    OrderCustomer.font = customFont;  
                }
                else
                {
                    Debug.LogError("The font is not set! Set CustomFont in Unity Inspector.");
                }

                OrderContent.color = UnityEngine.Color.black; 
                OrderCustomer.color = UnityEngine.Color.black;  

                OrderContent.fontSize = 36;  
                OrderCustomer.fontSize = 30;  

                Debug.Log($"주문서 로드 완료:\nOrderContent:\n{OrderContent.text}\nOrderCustomer:\n{OrderCustomer.text}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"주문서 파싱 중 오류 발생: {ex.Message}");
                OrderContent.text = "주문서를 불러오는 데 오류가 발생했습니다.";
                OrderCustomer.text = "주문서를 불러오는 데 오류가 발생했습니다.";
            }
        }
        else
        {
            OrderContent.text = "주문서가 없습니다.";
            OrderCustomer.text = "주문서가 없습니다.";
            Debug.LogWarning("저장된 주문서가 없습니다.");
        }
    }

    // 주문 내용 (Content)와 고객 정보 (Customer)를 함께 파싱하는 메소드
    private List<Tuple<int, int>> ParseOrderPairs(string serializedData)
    {
        List<Tuple<int, int>> orderPairs = new List<Tuple<int, int>>();

        // 예시 데이터를 공백이나 쉼표로 구분하여 파싱
        string[] dateOrders = serializedData.Split(new string[] { "Date " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var dateOrder in dateOrders)
        {
            // 각 날짜의 주문 데이터를 추출하여 내용과 고객을 처리
            string[] orderPairsStrings = dateOrder.Split(new string[] { "], [" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var orderPairString in orderPairsStrings)
            {
                // "], [" 가 제거된 쌍을 처리
                string cleanedPair = orderPairString.Replace("[", "").Replace("]", "");
                string[] pairValues = cleanedPair.Split(',');

                if (pairValues.Length == 2)
                {
                    int content = int.Parse(pairValues[0].Trim());
                    int customer = int.Parse(pairValues[1].Trim());

                    // Tuple로 묶어서 추가
                    orderPairs.Add(new Tuple<int, int>(content, customer));
                }
            }
        }

        return orderPairs;
    }

}
