using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class special_customer : MonoBehaviour
{
    private static special_customer _instance;
    public static special_customer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<special_customer>();
            }
            return _instance;
        }
    }

    public getMenuOnly getMenuOnly;
    public CharacterManager characterManager;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public GameObject speechBubble;
    public GameObject nameBubble;
    private Dictionary<int, GameObject> specialOrders = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> specialVisit = new Dictionary<int, GameObject>();
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 수정된 구조
    private int currentDialogueIndex = 0; // 현재 대화 인덱스
    public GameObject oldMan;
    public GameObject spcMan;
    public GameObject child;
    public Button none;
    private int check=0;
    private List<GameObject> customers = new List<GameObject>();

    public string csvFileNameGirl = "specialGuest_girl.csv";
    public string csvFileNameOldMan = "specialGuest_oldman.csv";
    public string csvFileNameMan = "specialGuest_man.csv";
    public int Spe_customer = 0;
    public int currentDay;
    private int currentDialogueId = 1; // 현재 대화 ID
    private int currentLineIndex = 0;  // 현재 대화 줄 인덱스
    private bool isOrderCompleted = false;
    private int current_startId;
    private GameObject customer;
    public Button dayChange;
    public GameObject MadeMenu;
    public SetMenu setmenu;
    private float currentTime;
    private int count;
    private bool Spe_visitDone;
    private bool tryvisit=false;

    [SerializeField] private GameData GD = new GameData();

    public struct DialogueLine
    {
        public int id;
        public string name;
        public string dialogue;

        public DialogueLine(int id, string name, string dialogue)
        {
            this.id = id;
            this.name = name;
            this.dialogue = dialogue;
        }
    }
    void Start()
    {
        GameData dateGD = DataManager.Instance.LoadGameData();
        oldMan.SetActive(false);
        child.SetActive(false);
        spcMan.SetActive(false);
        customers.Add(child);
        customers.Add(oldMan);
        customers.Add(spcMan);
        
        speechBubble.SetActive(false);

        specialOrders.Add(2, child);  
        specialOrders.Add(5, oldMan);
        specialOrders.Add(8, spcMan);

        specialVisit.Add(4, child);  
        specialVisit.Add(7, oldMan);
        specialVisit.Add(10, spcMan);

        dayChange.onClick.AddListener(()=>{
            SceneManager.LoadScene("Deadline_Last");
        });

        LoadDone();
        if (dateGD.time <= 355f)
        {
            dayChange.gameObject.SetActive(true);
            if (!Spe_visitDone)
            {
                currentDay = dateGD.date;
                spc_OnSpecialTimeReached();
            }
        }
        if ((dateGD.time <= 1f) & (Spe_visitDone | (!specialOrders.ContainsKey(currentDay) & !specialVisit.ContainsKey(currentDay))))
            {
                LoadDone();
                Spe_visitDone = false;
                SaveDone();
                SceneManager.LoadScene("Deadline");
            }

    }

    private IEnumerator WaitForUiLogicManager()
    {
        yield return new WaitUntil(() => UiLogicManager.Instance != null);

        UiLogicManager.Instance.KitchenButtonGO.GetComponent<Button>().interactable = false;
        UiLogicManager.Instance.order_button.gameObject.SetActive(false);
        UiLogicManager.Instance.RecipeButton.gameObject.SetActive(false);
        UiLogicManager.Instance.InventoryButtonGo.SetActive(false);
        UiLogicManager.Instance.RecipeBook.SetActive(false);
        UiLogicManager.Instance.OrderBook.SetActive(false);
}
private IEnumerator RestoreUI()
{
    yield return new WaitUntil(() => UiLogicManager.Instance != null);

    UiLogicManager.Instance.KitchenButtonGO.GetComponent<Button>().interactable = true;
    UiLogicManager.Instance.order_button.gameObject.SetActive(true);
    UiLogicManager.Instance.RecipeButton.gameObject.SetActive(true);
    UiLogicManager.Instance.InventoryButtonGo.SetActive(true);
}

    public void LoadDialoguesFromCSV()
    {
        try
        {
            Debug.Log($"현재 손님:{customer.name}");
            string csvFileName = "";
            if (customer.name == "child")
                csvFileName = csvFileNameGirl;
            else if (customer.name == "old_man")
                csvFileName = csvFileNameOldMan;
            else if (customer.name == "spcman")
                csvFileName = csvFileNameMan;

            Debug.Log($"로드할 CSV 파일: {csvFileName}");

            dialogues.Clear();

            TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(csvFileName));
            if (csvFile == null)
            {
                Debug.LogError($"CSV 파일을 찾을 수 없습니다: {csvFileName}");
                return;
            }

            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] fields = ParseCSVLine(line);
                if (fields.Length < 3) continue;

                if (int.TryParse(fields[0].Trim(), out int id))
                {
                    string name = fields[1].Trim();
                    string dialogue = fields[2].Trim();

                    dialogues.Add(new DialogueLine(id, name, dialogue));
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        foreach (char c in line)
        {
            if (c == '"' && !inQuotes)
            {
                inQuotes = true;
            }
            else if (c == '"' && inQuotes)
            {
                inQuotes = false;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }
        result.Add(currentField);
        return result.ToArray();
    }

    private void spc_OnSpecialTimeReached()
    {
        Debug.Log($"3시에 손님 등장 이벤트 발생{currentDay}일차");
        if (specialVisit.ContainsKey(currentDay))
        {
            Debug.Log($"특별 손님 베이커리받으러 옴{currentDay}일차");
            StartCoroutine(VisitSpecialCustomer());
        }
        else {
            Debug.Log($"특별 손님 주문받으러 옴{currentDay}일차");
            StartCoroutine(orderSpecialCustomer());// 특별 손님 주문
            }
    }

    public IEnumerator orderSpecialCustomer()
    {
        yield return new WaitUntil(() => getMenuOnly.VisitDone == true);
        Debug.Log("✅ 일반 손님 처리 완료됨! 특별 손님 등장 시작");
        dayChange.gameObject.SetActive(true);

        Debug.Log($"특별 손님 등장");
        if (specialOrders.ContainsKey(currentDay))
        {
            customer = specialOrders[currentDay];
            LoadDialoguesFromCSV();
            customer.SetActive(true);
            RectTransform customerRect = customer.GetComponent<RectTransform>();
            StartCoroutine(MoveCustomerUp(customerRect));

            StartCoroutine(WaitForUiLogicManager());
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.bell);
            current_startId = 1;
            PlayDialogue(current_startId);
        }
        else
        {
            Debug.LogError($"🚨 특별 손님 데이터 없음: {currentDay}일차");
        }
    }

    public IEnumerator VisitSpecialCustomer()
    {
        Debug.Log($"✅ 특별손님 등장 준비!:{currentDay}일차");
        yield return new WaitUntil(() => getMenuOnly.VisitDone == true);
        Debug.Log($"✅ 일반 손님 처리 완료됨! 특별 손님 등장 시작{currentDay}일차");
        dayChange.gameObject.SetActive(true);

        if (!specialVisit.ContainsKey(currentDay))
        {
            Debug.LogError($"특별 손님 방문 데이터가 없습니다!{currentDay}일차");
            yield break;
        }
        else {
        customer = specialVisit[currentDay];
        LoadDialoguesFromCSV();
        customer.SetActive(true);
        RectTransform customerRect = customer.GetComponent<RectTransform>();
        StartCoroutine(MoveCustomerUp(customerRect));

        StartCoroutine(WaitForUiLogicManager());
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.bell);
        current_startId=1001;
        PlayDialogue(current_startId);
        
        setmenu.current_cus("딸기 케이크", customer.name); 
        StartCoroutine(HandleCustomerInteraction(customer, currentDay));

        none.onClick.RemoveAllListeners();
        none.onClick.AddListener(() =>
        {
            Debug.Log($"손님 {customer.name}이(가) 메뉴를 받지 못했습니다.");
            UpdateDialogue("none");
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_fail);
        });
        }
    }
    private void PlayDialogue(int startId)
     {
        // ID에 맞는 대사를 리스트에서 검색
        currentDialogueIndex = dialogues.FindIndex(line => line.id == startId);
        if (currentDialogueIndex != -1)
        {
            StartCoroutine(ShowCurrentDialogue(startId));
        }
    else
    {
        Debug.LogWarning($"ID {startId}에 해당하는 대사가 없습니다.");
        EndDialogue();
    }
    }

    private IEnumerator ShowCurrentDialogue(int startId)
    {
         GameData dateGD = DataManager.Instance.LoadGameData();
        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialogues.Count)
        {
            if(startId==1){
                Debug.Log("스페셜 손님 주문 완료!");
                RectTransform customerRect = customer.GetComponent<RectTransform>();
                yield return StartCoroutine(MoveCustomerDown(customerRect));
                customer.SetActive(false);
                Spe_visitDone=true;
                SaveDone();
                StartCoroutine(RestoreUI());
                EndDialogue();
                if(dateGD.time<1){
                    SceneManager.LoadScene("Deadline");
        }
                yield break;
            }
            else if(startId==1001){
                speechBubble.SetActive(false);
                MadeMenu.SetActive(true);
                none.gameObject.SetActive(true);
                yield break;
            }
            else{
                isOrderCompleted = true;
                Debug.Log("제품 수령");
                yield break;
            }
        }
        DialogueLine currentLine = dialogues[currentDialogueIndex];
        if (string.IsNullOrWhiteSpace(currentLine.name))
        {
            nameBubble.SetActive(false);
        }
        else
        {
            nameBubble.SetActive(true);
            dialogueName.text = currentLine.name;
        }
        dialogueText.text = currentLine.dialogue;
        speechBubble.SetActive(true);

        // 다음 대사로 넘어갈 준비
        int nextId = currentLine.id + 1;
        currentDialogueIndex = dialogues.FindIndex(line => line.id == nextId);
    }

    private IEnumerator HandleCustomerInteraction(GameObject customer, int day)
    {
        yield return new WaitUntil(() => isOrderCompleted);
        RectTransform customerRect = customer.GetComponent<RectTransform>();
        yield return StartCoroutine(MoveCustomerDown(customerRect));
        customer.SetActive(false);
        Spe_visitDone=true;
        SaveDone();
        speechBubble.SetActive(false);
        StartCoroutine(RestoreUI());
        isOrderCompleted = false;
        none.gameObject.SetActive(false);

        Debug.Log("특별 손님이 방문을 완료했습니다.");
        GameData dateGD = DataManager.Instance.LoadGameData();
        if(dateGD.time<1){
            SceneManager.LoadScene("Deadline");
        }
    }

    public void UpdateDialogue(string action)
    {
        GameData dateGD = DataManager.Instance.LoadGameData();

        speechBubble.SetActive(true);
        none.gameObject.SetActive(false);
        MadeMenu.SetActive(false);
        Expression expression = Expression.set;
        if (action.Equals("none"))
        {
            expression = Expression.Bad;
            current_startId=4001;
            PlayDialogue(current_startId);
        }
        else if (action.Equals("True"))
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_succ);
            expression = Expression.Happy;
            current_startId=2001;
            PlayDialogue(current_startId);
            LoadDate();
            count++;
            SaveDate();
            Debug.Log("특별 손님이 제품을 받아갔습니다!");
        }
        else if (action.Equals("False"))
        {
            expression = Expression.Normal;
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_fail);
            current_startId=3001;
            PlayDialogue(current_startId);
            Debug.Log("특별 손님이 제품을 잘못 받아갔습니다!");
        }
        Debug.Log($"특별 손님:{customer}, 표정:{expression}");
        characterManager.ChangeFace(customer, expression);
    }

    private void EndDialogue()
    {
        Debug.Log("대화 종료");
        speechBubble.SetActive(false);
    }

    // 화면 클릭 시 다음 대사로 이동
    private void Update()
    {
        if (speechBubble.activeSelf && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShowCurrentDialogue(current_startId));
        }
        LoadDone();
        GameData dateGD = DataManager.Instance.LoadGameData();
        currentTime = dateGD.time; // 실시간으로 시간 업데이트
        currentDay = dateGD.date;
        if(!tryvisit){
            if (Mathf.Abs(currentTime - 355f) < 0.1f & !Spe_visitDone)
        {
            dayChange.gameObject.SetActive(true);
            tryvisit =true;
            spc_OnSpecialTimeReached();
        }
        }
        if(Mathf.Abs(currentTime - 1f) < 0.1f & (Spe_visitDone | (!specialOrders.ContainsKey(currentDay) & !specialVisit.ContainsKey(currentDay)))){
            LoadDone();
            Spe_visitDone=false;
            SaveDone();
            SceneManager.LoadScene("Deadline");
        }
}
private void LoadDate() {

        GD = DataManager.Instance.LoadGameData();
        // !! 일차 업데이트하기
        count=GD.EndingCount;
    }

    private void SaveDate() {
        DataManager.Instance.gameData.EndingCount = count;

        DataManager.Instance.SaveGameData();
    }

    private IEnumerator MoveCustomerUp(RectTransform customerRect)
{
    check++;
    Debug.Log(check);
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
    Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y-300, startPosition.z);


    float duration = 0.5f;
    float timeElapsed = 0;

    while (timeElapsed < duration)
    {
        customerRect.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
        timeElapsed += Time.deltaTime; // 시간 경과
        yield return null;
    }

    customerRect.position = startPosition; // 목표 위치로 고정
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
private void LoadDone() {

        GD = DataManager.Instance.LoadGameData();
        // !! 일차 업데이트하기
        Spe_visitDone=GD.Spe_visitDone;
    }

    private void SaveDone() {
        DataManager.Instance.gameData.Spe_visitDone = Spe_visitDone;

        DataManager.Instance.SaveGameData();
    }
}
