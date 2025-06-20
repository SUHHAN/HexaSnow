using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OrderOnly : MonoBehaviour
{
    private static OrderOnly _instance;
    public static OrderOnly Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OrderOnly>();
            }
            return _instance;
        }
    }

    public GameTime gametime;
    public GameObject postman;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public GameObject order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 주문 텍스트 표시용
    public TextMeshProUGUI orderCustomer;
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 주문 데이터 저장 리스트
    private List<DialogueLine> filteredDialogues = new List<DialogueLine>();
    private List<string> nicknames = new List<string>();
    public int order_menu_id;
    public List<string> order_nickname=new List<string>();
    public List<string> unlocked_menu=new List<string>();
    public List<int> order_deadLine=new List<int>();
    private int currentDialogueIndex; // 현재 주문 인덱스
    private int currentNicknameIndex; //현재 손님 인덱스
    public string csvFileName = "orderStatement_main.csv"; // CSV 파일 이름
    public string csvFileName_nickname="orderStatement_nickname.csv";
    public string csvFileName_menu="menu.csv";
    public Button acceptButton; // 수락 버튼
    public Button cancelButton; // 취소 버튼
    public Button orderCheck;
    public GameObject speechBubble;
    public GameObject nameBubble;
    private bool isAcceptButtonClicked = false;
    private int order_count=0;
    private int accept_order=2;
    private int deadline=1;
    private string currentScene;
    public GameObject popup;
    public TextMeshProUGUI popupText;
    private bool isPopupCoroutineRunning=false;
    private Dictionary<int, List<List<int>>> dailyOrders = new Dictionary<int, List<List<int>>>(); // 일별 주문 저장
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
    void Start()
    {
        GameData dateGD = DataManager.Instance.LoadGameData();
        if (dateGD.date == 10)
        {
            postman.SetActive(false);
            SceneManager.LoadScene("customer");
        }
        AudioManager.Instance.PlayBgm(AudioManager.Bgm.inside_kitchen_baking);
        SceneManager.LoadScene("Main", LoadSceneMode.Additive); //기본 UI 띄우기 
        StartCoroutine(WaitForUiLogicManager());
        
    
        //order.SetActive(false);
        RectTransform customerRect = postman.GetComponent<RectTransform>();
        StartCoroutine(MoveCustomerUp(customerRect));
        orderCheck.gameObject.SetActive(true);

        InitializeButtons(); // 버튼 초기화
        LoadDialoguesFromCSV(); // CSV 파일 로드
        LoadNicknameFromCSV();
        LoadMenuForPopup(); // 메뉴 해금 팝업용 CSV 로드
        Postmanment();

        openMenu(dateGD.date);

        if (dateGD.date >= 4)
        {
            accept_order = 4;
        }
        if (dateGD.date >= 7)
        {
            accept_order = 6;
        }

        //UiLogicManager.Instance.LoadMoneyData();
        Loadscene();
        currentScene = "order1";
        Savescene();
    }

    private IEnumerator WaitForUiLogicManager()
    {
        yield return new WaitUntil(() => UiLogicManager.Instance != null);

        UiLogicManager.Instance.KitchenButtonGO.GetComponent<Button>().interactable = false;
    
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

    private void LoadMenuForPopup()
    {
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(csvFileName_menu));
            if (csvFile == null)
            {
                Debug.LogError($"CSV 파일을 찾을 수 없습니다: {csvFileName_menu}");
                return;
            }

            string[] lines = csvFile.text.Split('\n');
            foreach (string line in lines)
            {
                string[] fields = line.Split(',');
                if (fields.Length > 0)
                {
                    string menu = fields[1].Trim(); // 메뉴 이름
                    unlocked_menu.Add(menu);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"메뉴 해금 CSV 파일 읽기 중 오류 발생: {ex.Message}");
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

    private void OpenOrderUI(){
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        orderCheck.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        SetRandomDialogueIndex();
        ShowDialogue();

    }
    private void Postmanment(){
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.bell);
        dialogueText.text=dialogues[1].description;
        dialogueName.text=dialogues[1].menu;

    }
    private void ShowDialogue()
    {
        order_count++;

        if (dialogues.Count > currentDialogueIndex)
        {   
            DialogueLine currentLine = dialogues[currentDialogueIndex];
            dialogueOrder.text = currentLine.description;
            orderCustomer.text=nicknames[currentNicknameIndex];
        }
        else
        {
            Debug.LogWarning("대화 내용이 없습니다");
        }
    }

    private void NextDialogue()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
        SetRandomDialogueIndex(); // 다음 대화도 랜덤으로 선택

        if (order_count==accept_order)
        {
            
            StartCoroutine(CloseDialogue()); // 대화 종료
        }
        else
        {
            ShowDialogue(); // 다음 내용 표시
        }
    }

    private IEnumerator CloseDialogue()
    {
        GameData dateGD = DataManager.Instance.LoadGameData();
        dateGD.time=360f;
        gametime.StartGameTimer();
        order.SetActive(false); // UI 비활성화
        currentDialogueIndex = 0; // 대화 인덱스 초기화'
        RectTransform customerRect = postman.GetComponent<RectTransform>();
        StartCoroutine(MoveCustomerDown(customerRect));
        yield return new WaitForSeconds(1f);
        postman.SetActive(false);
        speechBubble.SetActive(false);
        nameBubble.SetActive(false);
        UiLogicManager.Instance.KitchenButtonGO.GetComponent<Button>().interactable = true;
        SceneManager.LoadScene("customer");
}
 private void SetRandomDialogueIndex()
    {
        if (filteredDialogues.Count == 0)
    {
        Debug.LogWarning("필터링된 대화가 없습니다. 기본 대화 리스트를 사용합니다.");
        currentDialogueIndex = Random.Range(2, dialogues.Count); // 기본 전체 리스트에서 랜덤 선택
    }
    else
    {
        currentDialogueIndex = Random.Range(2, filteredDialogues.Count); // 필터링된 리스트에서 랜덤 선택
    }
        currentNicknameIndex=Random.Range(1, nicknames.Count);
        
    }
private void InitializeButtons(){
    // 기존 리스너 제거 후 새로 추가
    acceptButton.onClick.RemoveAllListeners();
    acceptButton.onClick.AddListener(() => {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
        order_menu_id = currentDialogueIndex;
        order_deadLine.Add(deadline);
        
        ReceiveOrders(currentNicknameIndex, order_menu_id);
        NextDialogue();
    });

    cancelButton.onClick.RemoveAllListeners();
    cancelButton.onClick.AddListener(() => StartCoroutine(CloseDialogue()));

    orderCheck.onClick.RemoveAllListeners();
    orderCheck.onClick.AddListener(OpenOrderUI);
}
    private void ReceiveOrders(int currentNicknameIndex, int order_menu_id)
    {
        GameData dateGD = DataManager.Instance.LoadGameData();
        LoadDate(); // GD가 null이면 로드하여 처리
        if (!dailyOrders.ContainsKey(dateGD.date))
        {
            dailyOrders[dateGD.date] = new List<List<int>>();
        }
        Debug.Log($"현재 {dateGD.date}일의 주문 상태: {dailyOrders[dateGD.date].Count}개의 주문이 있습니다.");
        dailyOrders[dateGD.date].Add(new List<int> { order_menu_id, currentNicknameIndex });
        SaveDate();
        Loadscene();
        currentScene = "customer";
        Savescene();
}
public void openMenu(int day){
    int maxId;
    if (day <= 2)
            maxId = day * 2000 + 1000;
        else maxId = day * 1000 + 3000;
     filteredDialogues=dialogues.FindAll(dialogue=>{
        if (int.TryParse(dialogue.id, out int dialogueId)){
            return dialogueId<=maxId;
        }
        return false;
     });

     if (filteredDialogues.Count == 0)
    {
        Debug.LogWarning($"현재 날짜({day})에 허용된 주문이 없습니다!");
    }
    else
    {
        Debug.Log($"현재 날짜({day})에 허용된 주문 개수: {filteredDialogues.Count}");
    }
    showPopup();
}
    private void showPopup(){
         int openmenuIndex;
        GameData dateGD = DataManager.Instance.LoadGameData();
        if(dateGD.date<=2)
            openmenuIndex=dateGD.date*2-1;
        else openmenuIndex=dateGD.date+2;
        Debug.Log($"openmenuIndex, unlocked_menu.Count {openmenuIndex}, {unlocked_menu.Count}");
    if (openmenuIndex < unlocked_menu.Count)
    {
        string unlockedMenu1 = unlocked_menu[openmenuIndex];
        string unlockedMenu2 = (openmenuIndex+1 < unlocked_menu.Count) ? unlocked_menu[openmenuIndex+1] : null;

        if (!string.IsNullOrEmpty(unlockedMenu1))
        {
            popup.SetActive(true);
            if(dateGD.date<=2)
                popupText.text = $"{unlockedMenu1}, {unlockedMenu2}\n레시피가 해금되었습니다!";
            else popupText.text = $"{unlockedMenu1}\n레시피가 해금되었습니다!";
        }
}
    }
    void Update()
    {
    // 팝업이 활성화된 상태에서 클릭 감지
    if (popup.activeSelf && Input.GetMouseButtonDown(0))
    {
      AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        popup.SetActive(false); // 팝업 비활성화
        order.SetActive(true); // UI 비활성화
        postman.SetActive(true);
        speechBubble.SetActive(true);
    }
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

    GD = DataManager.Instance.LoadGameData(); // GameData 로드
    if (GD == null)
    {
        Debug.LogError("[LoadDate] 저장된 GameData가 없습니다. LoadGameData()가 null을 반환했습니다.");
        return;
    }
    if (!string.IsNullOrEmpty(GD.serializedDailyOrders))
    {
        SerializableDictionary<int, OrderListWrapper> deserializedData =
            JsonUtility.FromJson<SerializableDictionary<int, OrderListWrapper>>(GD.serializedDailyOrders);

        dailyOrders = new Dictionary<int, List<List<int>>>();

        foreach (var pair in deserializedData.ToDictionary())
        {
            List<List<int>> orderList = new List<List<int>>();

            foreach (var orderItem in pair.Value.orders)
            {
                orderList.Add(orderItem.items);
            }

            dailyOrders[pair.Key] = orderList;
        }
    }
}

private void SaveDate()
{
    // 새로운 Dictionary<int, OrderListWrapper> 생성
    Dictionary<int, OrderListWrapper> wrappedOrdersToSave = new Dictionary<int, OrderListWrapper>();

    foreach (var dayOrder in dailyOrders)
    {
        OrderListWrapper wrapper = new OrderListWrapper();

        // 기존 List<List<int>> → List<OrderItem> 변환
        foreach (var order in dayOrder.Value)
        {
            wrapper.orders.Add(new OrderItem { items = order });
       }

        wrappedOrdersToSave[dayOrder.Key] = wrapper;
    }

    // SerializableDictionary로 변환
    SerializableDictionary<int, OrderListWrapper> serializableOrders = 
        new SerializableDictionary<int, OrderListWrapper>(wrappedOrdersToSave);

    // JSON 직렬화 (한 줄로 저장)
    string json = JsonUtility.ToJson(serializableOrders, false);
    GD.serializedDailyOrders = json;
    DataManager.Instance.gameData.serializedDailyOrders = json;
    DataManager.Instance.SaveGameData();

    Debug.Log("[SaveDate] 데이터 저장 완료!");
}

  private void Loadscene() {

        GD = DataManager.Instance.LoadGameData();
        currentScene= GD.currentScene;
    }

    private void Savescene() {
        DataManager.Instance.gameData.currentScene = currentScene;

        DataManager.Instance.SaveGameData();
    }

    private void LoadMoData() {
        GD = DataManager.Instance.LoadGameData();
        GD.money -= 500;
        DataManager.Instance.SaveGameData();
}
private IEnumerator MoveCustomerUp(RectTransform customerRect)
{
    Vector3 targetPosition = customerRect.position; // 현재 위치가 목표 위치
    Vector3 startPosition = new Vector3(targetPosition.x, targetPosition.y - 200, targetPosition.z); // 아래에서 시작

    float duration = 0.5f;
    float timeElapsed = 0;

    while (timeElapsed < duration)
    {
        float t = timeElapsed / duration;
        t = EaseOutBounce(t); // 반동 효과
        customerRect.position = Vector3.Lerp(startPosition, targetPosition, t);
        timeElapsed += Time.deltaTime;
        yield return null;
    }

    customerRect.position = targetPosition; // 목표 위치로 고정
}

private IEnumerator MoveCustomerDown(RectTransform customerRect)
{
    Vector3 startPosition = customerRect.position;
    Vector3 bouncePosition = new Vector3(startPosition.x, startPosition.y + 20, startPosition.z); // 반동 위치
    Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y - 1200, startPosition.z); // 최종 위치

    float bounceDuration = 0.3f;
    float downDuration = 0.8f;
    float timeElapsed = 0;

    // 🎮 1단계: 반동 애니메이션
    while (timeElapsed < bounceDuration)
    {
        float t = timeElapsed / bounceDuration;
        t = EaseOutBack(t);
        customerRect.position = Vector3.Lerp(startPosition, bouncePosition, t);
        timeElapsed += Time.deltaTime;
        yield return null;
    }

    // 🎮 원위치로 복구
    //customerRect.position = startPosition;
    timeElapsed = 0;

    // 🎮 2단계: 내려가는 애니메이션
    timeElapsed = 0f;
    while (timeElapsed < downDuration)
    {
        float t = timeElapsed / downDuration;
        customerRect.position = Vector3.Lerp(bouncePosition, targetPosition, t);
        timeElapsed += Time.deltaTime;
        yield return null;
    }

    customerRect.position = targetPosition; // 최종 위치로 고정
}

// 🎮 반동 효과 함수
private float EaseOutBounce(float t)
{
    if (t < 1 / 2.75f)
    {
        return 7.5625f * t * t;
    }
    else if (t < 2 / 2.75f)
    {
        t -= 1.5f / 2.75f;
        return 7.5625f * t * t + 0.75f;
    }
    else if (t < 2.5 / 2.75f)
    {
        t -= 2.25f / 2.75f;
        return 7.5625f * t * t + 0.9375f;
    }
    else
    {
        t -= 2.625f / 2.75f;
        return 7.5625f * t * t + 0.984375f;
    }
}
private float EaseOutBack(float t)
{
    float c1 = 1.70158f;
    float c3 = c1 + 1;
    return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
}
}