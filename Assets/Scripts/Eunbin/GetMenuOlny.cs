using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Data.SqlTypes;
using UnityEngine.SceneManagement;

public class getMenuOnly : MonoBehaviour
{
    public CharacterManager characterManager; 
    public GameObject man;
    public GameObject shortgirl;
    public GameObject girl;
    public Button none;
    private Dictionary<int, List<List<int>>> dailyOrders = new Dictionary<int, List<List<int>>>(); // 일별 주문 저장
    private List<GameObject> customers = new List<GameObject>();
    public string csvFileName = "orderStatement_main.csv";
    public string csvFileName_nickname="orderStatement_nickname.csv";
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 주문 데이터 저장 리스
    private List<string> nicknames = new List<string>();
    public int currentDay = 1; // 현재 날짜를 추적
    public GameObject customer_order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 주문 텍스트 표시용
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public GameObject speechBubble;
    private int NicknameIndex=0;
    bool isOrderCompleted = false;
    public string csvFileName_guest="guest.csv";
    private List<DialogueLine> guestDialogues = new List<DialogueLine>(); // 주문 데이터 저장 리스트
    private Dictionary<string, List<DialogueLine>> guestDialoguesByState = new Dictionary<string, List<DialogueLine>>(); // 상태별 대화 그룹화


    public GameObject MadeMenu;
    public string menuName;
    public SetMenu setmenu;
    private GameObject customer; 

    private int DataMoney;

    [SerializeField] private GameData GD = new GameData();
    
    public struct DialogueLine{
    public string id;
    public string menu;
    public string description;

    public DialogueLine(string id, string menu, string description){
        this.id=id;
        this.menu=menu;
        this.description=description;
    }
    }
    void Start(){
        Debug.Log("[Start] 씬이 시작됨! 데이터 로드 시도...");
        customer_order.SetActive(false);
        speechBubble.SetActive(false);
        SceneManager.LoadScene("Main", LoadSceneMode.Additive); //기본 UI 띄우기 
        LoadDialoguesFromCSV(); // CSV 파일 로드
        LoadNicknameFromCSV();
        LoadGuestFromCSV();

        customers.Add(man);
        customers.Add(girl);
        customers.Add(shortgirl);
        GameData dateGD = DataManager.Instance.LoadGameData();
        StartCoroutine(ProcessCustomers(dateGD.date-1));
        
        
    }
     private void LoadDialoguesFromCSV()
    {
        try
        {
            // Resources 폴더에서 CSV 파일 읽기
            TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(csvFileName));
            if (csvFile == null)
            {
                Debug.LogError($"CSV 파일을 찾을 수 없습니다: {csvFileName}");
                return;
            }

            // 줄 단위로 나누기
            string[] lines = csvFile.text.Split(new[] {'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                // 쉼표로 구분된 데이터 파싱
                string[] fields = ParseCSVLine(line);
                if (fields.Length < 3) continue;

                    string id=fields[0].Trim();
                    string menu=fields[1].Trim();
                    string description=fields[2].Trim();

                    dialogues.Add(new DialogueLine(id, menu, description));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }
    private void LoadNicknameFromCSV(){
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(csvFileName_nickname));
            if (csvFile == null)
            {
                Debug.LogError($"CSV 파일을 찾을 수 없습니다: {csvFileName_nickname}");
                return;
            }

            string[] lines = csvFile.text.Split('\n');
            foreach (string line in lines)
        {
            // 구분자로 데이터 파싱
            string[] fields = line.Split(',');
            if (fields.Length > 0) { 
                nicknames.Add(fields[1].Trim());
        }
        }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"닉네임 CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

private void LoadGuestFromCSV()
{
    try
    {
        // Resources 폴더에서 CSV 파일 읽기
        TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(csvFileName_guest));
        if (csvFile == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {csvFileName_guest}");
            return;
        }

        // 줄 단위로 나누기
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            // 쉼표로 구분된 데이터 파싱
            string[] fields = ParseCSVLine(line);
            if (fields.Length < 3) continue; // 최소 3개 필드가 있어야 함

            string index = fields[0].Trim();   // 대화 인덱스
            string state = fields[1].Trim();   // 상태 값
            string dialogue = fields[2].Trim(); // 대화 내용

            // 대화 라인 생성
            DialogueLine dialogueLine = new DialogueLine(index, state, dialogue);

            // 해당 상태에 맞는 대화 추가
            if (!guestDialoguesByState.ContainsKey(state))
            {
                guestDialoguesByState[state] = new List<DialogueLine>();
            }
            guestDialoguesByState[state].Add(dialogueLine);
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
    }
}
    private string[] ParseCSVLine(string line){
        List<string> result=new List<string>();
        bool inQuotes=false;
        string currentField="";

        foreach (char c in line)
        {
            if (c == '"' && !inQuotes)
            {
                inQuotes = true; // 큰따옴표 시작
            }
            else if (c == '"' && inQuotes)
            {
                inQuotes = false; // 큰따옴표 종료
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField);
                currentField = ""; // 필드 초기화
            }
            else
            {
                currentField += c; // 필드에 문자 추가
            }
        }
        result.Add(currentField); // 마지막 필드 추가
        return result.ToArray();
    }
  public IEnumerator ProcessCustomers(int dayToProcess)
{
    GameData data = DataManager.Instance.LoadGameData();
    SerializableDictionary<int, ValueData> orderDictionary =
        JsonUtility.FromJson<SerializableDictionary<int, ValueData>>(data.serializedDailyOrders);

    Debug.Log($"[{dayToProcess}일] 손님 처리 시작");

    none.gameObject.SetActive(true);
    MadeMenu.SetActive(true);

    if (dailyOrders == null)
    {
        dailyOrders = new Dictionary<int, List<List<int>>>();
    }
    dailyOrders.Clear();

    if (orderDictionary == null || orderDictionary.keyValuePairs.Count == 0)
    {
        Debug.Log($"[{dayToProcess}일] 주문 데이터가 없습니다.");
        MadeMenu.SetActive(false);
        none.gameObject.SetActive(false);
        yield break;
    }

    if (!dailyOrders.ContainsKey(dayToProcess))
    {
        dailyOrders[dayToProcess] = new List<List<int>>();
    }

    foreach (var pair in orderDictionary.keyValuePairs)
    {
        if (pair.Key == dayToProcess) // 특정 키값을 찾기
    {
        foreach (var order in pair.Value.orders)
        {
            if (order.items.Count == 2)
            {
                dailyOrders[dayToProcess].Add(new List<int> { order.items[0], order.items[1] });
            }
        }
        }
    }

    // 디버깅: dailyOrders에 데이터가 잘 추가되었는지 확인
    Debug.Log($"[{dayToProcess}일] dailyOrders 내용: {dailyOrders[dayToProcess].Count} 개의 주문");

    if (!dailyOrders.ContainsKey(dayToProcess) || dailyOrders[dayToProcess].Count == 0)
    {
        Debug.Log($"[{dayToProcess}일] 처리할 주문이 없습니다.");
        MadeMenu.SetActive(false);
        none.gameObject.SetActive(false);
        yield break;
    }

    List<List<int>> menuForDay = new List<List<int>>(dailyOrders[dayToProcess]);

    foreach (List<int> order in menuForDay)
    {
        int orderId = order[0];
        int nicknameIndex = order[1];

        // 디버깅: orderId와 nicknameIndex가 유효한 값인지 확인
        Debug.Log($"주문 정보: orderId = {orderId}, nicknameIndex = {nicknameIndex}");

        if (orderId < 0 || orderId >= dialogues.Count)
        {
            Debug.LogError($"Invalid orderId: {orderId}");
            continue;
        }
        if (nicknameIndex < 0 || nicknameIndex >= nicknames.Count)
        {
            Debug.LogError($"Invalid nicknameIndex: {nicknameIndex}");
            continue;
        }

        menuName = dialogues[orderId].menu;
        dialogueName.text = nicknames[nicknameIndex];

        customer = GetRandomCustomer();
        if (customer == null)
        {
            Debug.LogError("Failed to get a customer.");
            continue;
        }
        customer.SetActive(true);

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.bell);
        Debug.Log($"손님 {customer.name}이(가) 메뉴 {menuName}을(를) 받으러 왔습니다!");
        setmenu.current_cus(menuName, "cus");
        ShowOrder(orderId);

        isOrderCompleted = false;

        none.onClick.RemoveAllListeners();
        none.onClick.AddListener(() => {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_fail);
            Debug.Log($"손님 {customer.name}이(가) 메뉴를 받지 못했습니다.");
            UpdateDialogue(5);
            LoadMoData();
            UiLogicManager.Instance.LoadMoneyData();
            isOrderCompleted = true;
        });

        yield return new WaitUntil(() => isOrderCompleted);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        customer.SetActive(false);
        customer_order.SetActive(false);
        speechBubble.SetActive(false);

        Debug.Log($"손님 {customer.name}이(가) 메뉴를 받아갔습니다!");
        characterManager.ChangeFace(customer, Expression.set);
    }
    orderDictionary.keyValuePairs.RemoveAll(pair => pair.Key == dayToProcess);
    data.serializedDailyOrders = JsonUtility.ToJson(orderDictionary);

    Debug.Log($"[{dayToProcess}일] 모든 손님이 메뉴를 받아갔습니다.");
    MadeMenu.SetActive(false);
    none.gameObject.SetActive(false);
}

    private GameObject GetRandomCustomer(){
        int randomIndex=Random.Range(0, customers.Count);
        return customers[randomIndex];
        
    }
 public string GetRandomDialogue(string state)
{
    // 해당 state에 대한 대화 리스트가
    Debug.Log($"[디버깅] 현재 저장된 state 목록: {string.Join(", ", guestDialoguesByState.Keys)}");

    if (guestDialoguesByState.ContainsKey(state))
    {
        List<DialogueLine> dialoguesForState = guestDialoguesByState[state];

        // 대화 리스트가 비어있지 않으면 랜덤으로 하나 선택
        if (dialoguesForState.Count > 0)
        {
            int randomIndex = Random.Range(0, dialoguesForState.Count);
            return dialoguesForState[randomIndex].description; // 랜덤 대화 반환

        }
    }

    return "대화 없음"; // 해당 state에 대한 대화가 없을 경우
}


    private void ShowOrder(int order){
        customer_order.SetActive(true);
        dialogueOrder.text = dialogues[order].description;
        speechBubble.SetActive(true);
        dialogueText.text=guestDialoguesByState["0"][0].description;

    }
    public void UpdateDialogue(int state){
        isOrderCompleted = true; 
          
        dialogueText.text=GetRandomDialogue(state.ToString());
        ChangeFace(state);

    }
    private void ChangeFace(int state)
{
    Debug.Log($"[GetchangeFace] 상태: {state}");
    Expression expression = Expression.set; // 기본값을 'Normal'로 설정

    // 상태 값에 맞는 표현식을 매핑
    if (state == 1) {
        expression = Expression.Happy;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_succ);
    }
    else if (state == 2){
         expression = Expression.Normal;
         AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_succ);
    }
    else if (state == 3 || state == 4 || state == 5){
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_fail);
        expression = Expression.Bad;
}

    // 표정 변경
    characterManager.ChangeFace(customer, expression);
}


[System.Serializable]
public class OrderItem
{
    public List<int> items;  // 내부 리스트를 클래스로 감싸기
}

[System.Serializable]
public class OrderListWrapper
{
    public List<OrderItem> orders = new List<OrderItem>(); // OrderItem을 사용

}
private void LoadDate()
{
    Debug.Log("[LoadDate] 데이터 로드 시작");

    if (GD == null)
    {
        Debug.LogError("[LoadDate] GD 객체가 null입니다! 데이터를 불러올 수 없습니다.");
        return;
    }

    GD = DataManager.Instance.LoadGameData(); // GameData 로드
    if (GD == null)
    {
        Debug.LogError("[LoadDate] 저장된 GameData가 없습니다. LoadGameData()가 null을 반환했습니다.");
        return;
    }

    // 🔥 씬 이동 후 JSON 데이터가 잘 불러와지는지 확인!
    Debug.Log($"[LoadDate] 불러온 JSON 데이터 (씬 이동 후): {GD.serializedDailyOrders}");

    if (!string.IsNullOrEmpty(GD.serializedDailyOrders))
    {
        SerializableDictionary<int, OrderListWrapper> deserializedData =
            JsonUtility.FromJson<SerializableDictionary<int, OrderListWrapper>>(GD.serializedDailyOrders);

        dailyOrders = new Dictionary<int, List<List<int>>>();

        foreach (var pair in deserializedData.ToDictionary())
        {
            Debug.Log($"[LoadDate] 날짜 {pair.Key} 데이터 복원 중...");
            List<List<int>> orderList = new List<List<int>>();

            foreach (var orderItem in pair.Value.orders)
            {
                orderList.Add(orderItem.items);
                Debug.Log($"[LoadDate] 날짜 {pair.Key} - 복원된 주문 데이터: [{string.Join(", ", orderItem.items)}]");
            }

            dailyOrders[pair.Key] = orderList;
        }
    }

    Debug.Log("[LoadDate] 데이터 로드 완료!");
}

private void SaveDate()
{
    Debug.Log("[SaveDate] 데이터 저장 시작");

    // 새로운 Dictionary<int, OrderListWrapper> 생성
    Dictionary<int, OrderListWrapper> wrappedOrdersToSave = new Dictionary<int, OrderListWrapper>();

    foreach (var dayOrder in dailyOrders)
    {
        OrderListWrapper wrapper = new OrderListWrapper();
        Debug.Log($"[SaveDate] 날짜 {dayOrder.Key} 데이터 변환 중...");

        // 기존 List<List<int>> → List<OrderItem> 변환
        foreach (var order in dayOrder.Value)
        {
            wrapper.orders.Add(new OrderItem { items = order });
            Debug.Log($"[SaveDate] 날짜 {dayOrder.Key} - 저장할 주문: [{string.Join(", ", order)}]");
        }

        wrappedOrdersToSave[dayOrder.Key] = wrapper;
    }

    // SerializableDictionary로 변환
    SerializableDictionary<int, OrderListWrapper> serializableOrders = 
        new SerializableDictionary<int, OrderListWrapper>(wrappedOrdersToSave);

    // 디버깅 로그: serializableOrders 확인
    foreach (var pair in serializableOrders.keyValuePairs)
    {
        Debug.Log($"[SaveDate] 변환된 데이터 - 날짜 {pair.Key} : {string.Join(", ", pair.Value.orders.Select(order => $"[{string.Join(", ", order.items)}]"))}");
    }

    // JSON 직렬화 (한 줄로 저장)
    string json = JsonUtility.ToJson(serializableOrders, false);
    Debug.Log($"[SaveDate] 직렬화된 JSON 데이터: {json}");

    // 저장
    GD.serializedDailyOrders = json;
    DataManager.Instance.gameData.serializedDailyOrders = json;
    DataManager.Instance.SaveGameData();

    Debug.Log("[SaveDate] 데이터 저장 완료!");
}
private void LoadMoData() {
        GD = DataManager.Instance.LoadGameData();
        GD.money -= 500;
        DataManager.Instance.SaveGameData();
}
}
