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
    public CharacterManager characterManager;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public GameObject speechBubble;
    public GameObject nameBubble;
    public GameTime gametime;
    private Dictionary<int, GameObject> specialOrders = new Dictionary<int, GameObject>();
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 수정된 구조
    private int currentDialogueIndex = 0; // 현재 대화 인덱스
    public GameObject oldMan;
    public GameObject man;
    public GameObject child;
    public Button none;
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
        man.SetActive(false);
        customers.Add(child);
        customers.Add(oldMan);
        customers.Add(man);
        
        speechBubble.SetActive(false);

        specialOrders.Add(2, child);  
        specialOrders.Add(5, oldMan);
        specialOrders.Add(8, man);

        dayChange.onClick.AddListener(()=>{
            SceneManager.LoadScene("Deadline_Last");
        });

        if(dateGD.time <= 240f){
            currentDay = dateGD.date;
            orderSpecialCustomer(); // 특별 손님 주문
            spc_OnSpecialTimeReached();
        }

    }
    private IEnumerator WaitForUiLogicManager()
{
    yield return new WaitUntil(() => UiLogicManager.Instance != null);

    UiLogicManager.Instance.KitchenButtonGO.GetComponent<Button>().interactable = false;
    UiLogicManager.Instance.order_button.gameObject.SetActive(false);
    UiLogicManager.Instance.RecipeButton.gameObject.SetActive(false);
    UiLogicManager.Instance.InventoryButtonGo.SetActive(false);
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
            else if (customer.name == "man")
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
        Debug.Log("3시에 손님 등장 이벤트 발생");
        if (specialOrders.ContainsKey(currentDay-2))
        {
            VisitSpecialCustomer(currentDay-2);
        }
        else Debug.Log("특별 손님 주문받으러 옴");
    }

    public void orderSpecialCustomer()
    {
        dayChange.gameObject.SetActive(true);
        foreach (GameObject customerObj in customers)
        {
            customerObj.SetActive(false); // 모든 손님 비활성화
        }
        Debug.Log($"특별 손님 등장");
        if (specialOrders.ContainsKey(currentDay))
        {
            customer = specialOrders[currentDay];
            LoadDialoguesFromCSV();
            customer.SetActive(true);
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

    public void VisitSpecialCustomer(int day)
    {
        if (!specialOrders.ContainsKey(day))
        {
            Debug.LogError("특별 손님 방문 데이터가 없습니다!");
            return;
        }

        customer = specialOrders[day];
        LoadDialoguesFromCSV();
        customer.SetActive(true);
        StartCoroutine(WaitForUiLogicManager());
        current_startId=1001;
        PlayDialogue(current_startId);
        setmenu.current_cus("딸기 케이크", "special"); 
        StartCoroutine(HandleCustomerInteraction(customer, day));

        none.onClick.RemoveAllListeners();
        none.onClick.AddListener(() =>
        {
            Debug.Log($"손님 {customer.name}이(가) 메뉴를 받지 못했습니다.");
            UpdateDialogue("none");
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.ingre_fail);
        });
    }
    private void PlayDialogue(int startId)
     {
        // ID에 맞는 대사를 리스트에서 검색
        currentDialogueIndex = dialogues.FindIndex(line => line.id == startId);
        if (currentDialogueIndex != -1)
        {
            ShowCurrentDialogue(startId);
        }
    else
    {
        Debug.LogWarning($"ID {startId}에 해당하는 대사가 없습니다.");
        EndDialogue();
    }
    }

    private void ShowCurrentDialogue(int startId)
    {
        if (currentDialogueIndex < 0 || currentDialogueIndex >= dialogues.Count)
        {
            if(startId==1){
                Debug.Log("스페셜 손님 주문 완료!");
                customer.SetActive(false);
                StartCoroutine(RestoreUI());
                EndDialogue();
                return;
            }
            else if(startId==1001){
                speechBubble.SetActive(false);
                MadeMenu.SetActive(true);
                none.gameObject.SetActive(true);
                return;
            }
            else{
                isOrderCompleted = true;
                Debug.Log("제품 수령");
                return;
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
        speechBubble.SetActive(false);
        customer.SetActive(false);
        StartCoroutine(RestoreUI());
        isOrderCompleted = false;
        none.gameObject.SetActive(false);

        Debug.Log("특별 손님이 방문을 완료했습니다.");
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
            dateGD.EndingCount++;
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
            ShowCurrentDialogue(current_startId);
        }

        GameData dateGD = DataManager.Instance.LoadGameData();
        currentTime = dateGD.time; // 실시간으로 시간 업데이트

        if (Mathf.Abs(currentTime - 240f) < 0.1f)
        {
            orderSpecialCustomer();
            spc_OnSpecialTimeReached();
            currentDay = dateGD.date;
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
}
