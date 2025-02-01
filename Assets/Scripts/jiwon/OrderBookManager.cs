using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public class OrderB
{
    public List<int> items;
}

// ValueData class for the orders
[System.Serializable]
public class ValueData
{
    public List<OrderB> orders;
}

public class OrderBookManager : MonoBehaviour
{
    public GameObject OrderBook;  // OrderBook 패널
    public Button order_button;   // Order 버튼
    public TMP_Text OrderContent; // 주문서 내용
    public TMP_Text OrderCustomer; // 주문서 고객

    // CSV 데이터를 저장할 Dictionary
    private Dictionary<int, string> mainOrderData = new Dictionary<int, string>();
    private Dictionary<int, string> nicknameOrderData = new Dictionary<int, string>();

    public Button nextButton;  // Next 버튼
    public Button prevButton;  // Previous 버튼

    public TextAsset mainCsvFile;   // Order Statement_main.csv 파일
    public TextAsset nicknameCsvFile;   // Order Statement_nickname.csv 파일

    private List<Tuple<int, int>> orderPairs = new List<Tuple<int, int>>();  // 주문서 데이터
    private int currentOrderIndex = 0;  // 현재 주문서 인덱스

    public TMP_FontAsset customFont;

    void Start()
    {
        nextButton.onClick.AddListener(ShowNextOrder);
        prevButton.onClick.AddListener(ShowPreviousOrder);
        LoadCsvData();
        LoadOrderBook();
        order_button.onClick.AddListener(LoadOrderBook);
    }



    // CSV 파일을 읽고 데이터를 파싱하는 함수
    private void LoadCsvData()
    {
        // "Order Statement_main.csv" 파싱
        if (mainCsvFile != null)
        {
            var lines = mainCsvFile.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int contentId = 0;  // Row index
            foreach (var line in lines)
            {
                var values = line.Split(',');
                if (values.Length >= 2)
                {
                    string content = values[2]; // 내용은 두 번째 컬럼에 있다고 가정
                    if (mainOrderData.ContainsKey(contentId))
                    {
                        Debug.LogError($"[중복 키 발생] {contentId} 이미 존재함!");
                    }
                    else
                    {
                        mainOrderData[contentId] = content;
                        Debug.Log($"[추가됨] mainOrderData[{contentId}] = {content}");
                    }
                }
                contentId++;
            }
        }
        else
        {
            Debug.LogError("Order Statement_main.csv 파일이 할당되지 않았습니다.");
        }

        // "Order Statement_nickname.csv" 파싱
        if (nicknameCsvFile != null)
        {
            var lines = nicknameCsvFile.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int customerId = 0;  // Row index
            foreach (var line in lines)
            {
                var values = line.Split(',');
                if (values.Length >= 2)
                {
                    string nickname = values[1]; // 닉네임은 두 번째 컬럼에 있다고 가정
                    nicknameOrderData[customerId] = nickname;
                }
                customerId++;
            }
        }
        else
        {
            Debug.LogError("Order Statement_nickname.csv 파일이 할당되지 않았습니다.");
        }
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
            try
            {
                // 주문서 JSON 데이터를 파싱 (SerializableDictionary을 바로 파싱)
                SerializableDictionary<int, ValueData> orderDictionary =
                    JsonUtility.FromJson<SerializableDictionary<int, ValueData>>(data.serializedDailyOrders);

                if (orderDictionary == null || orderDictionary.keyValuePairs.Count == 0)
                {
                    Debug.LogWarning("파싱된 주문서 데이터가 없습니다.");
                    OrderContent.text = "주문서가 없습니다.";
                    OrderCustomer.text = "주문서가 없습니다.";
                    return;
                }

                // "orders" 배열에서 "items" 값만 추출
                orderPairs.Clear();  // 기존 데이터 초기화
                foreach (var pair in orderDictionary.keyValuePairs)
                {
                    foreach (var order in pair.Value.orders)
                    {
                        if (order.items.Count == 2) // [Content, Customer] 쌍이어야 함
                        {
                            orderPairs.Add(new Tuple<int, int>(order.items[0], order.items[1]));
                        }
                    }
                }

                if (orderPairs.Count == 0)
                {
                    Debug.LogWarning("주문서 데이터가 없습니다.");
                    OrderContent.text = "주문서가 없습니다.";
                    OrderCustomer.text = "주문서가 없습니다.";
                    return;
                }

                Debug.Log(currentOrderIndex);
                ShowOrder(currentOrderIndex);

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

    

    void ShowOrder(int orderIndex)
    {
        Debug.Log("글자 보여주기");
        if (orderPairs.Count == 0)
        {
            return;  // 데이터가 없으면 처리 안 함
        }

        // 현재 주문서 내용과 고객 이름을 UI에 표시
        Tuple<int, int> currentOrder = orderPairs[orderIndex];
        // 현재 주문에 해당하는 content와 customer 값
        int contentId = currentOrder.Item1;
        int customerId = currentOrder.Item2;

        Debug.Log($"[ShowOrder] contentId 타입: {contentId.GetType()}, 값: {contentId}");
        foreach (var key in mainOrderData.Keys)
        {
            Debug.Log($"[ShowOrder] 저장된 키: {key}, 타입: {key.GetType()}");
        }

        // CSV에서 contentId와 customerId로 각 항목을 찾아서 텍스트에 할당
        string contentText = mainOrderData[contentId];
        string customerText = nicknameOrderData[customerId];

        Debug.Log($"contentId: {contentId}, customerId: {customerId}");
        Debug.Log($"Found Content: {contentText}");
        Debug.Log($"Found Customer: {customerText}");
        OrderContent.text = $"{contentText}";
        OrderCustomer.text = $"{customerText}";

        // 폰트 설정
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
        OrderCustomer.fontSize = 25;

        // 페이지 버튼 활성화 여부 조정
        prevButton.interactable = orderIndex > 0;  // 첫 페이지가 아니면 "이전" 버튼 활성화
        nextButton.interactable = orderIndex < orderPairs.Count - 1;  // 마지막 페이지가 아니면 "다음" 버튼 활성화
    }

    void ShowNextOrder()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
        if (currentOrderIndex < orderPairs.Count - 1)
        {
            currentOrderIndex++;
            ShowOrder(currentOrderIndex);
        }
    }

    void ShowPreviousOrder()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
        if (currentOrderIndex > 0)
        {
            currentOrderIndex--;
            ShowOrder(currentOrderIndex);
        }
    }
}
