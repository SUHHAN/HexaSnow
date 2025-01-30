using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class special_customer : MonoBehaviour
{
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
    GameObject customer;

    public GameObject MadeMenu;
    public SetMenu setmenu;

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
        oldMan.SetActive(false);
        child.SetActive(false);
        man.SetActive(false);
        customers.Add(child);
        customers.Add(oldMan);
        customers.Add(man);

        speechBubble.SetActive(false);
        gametime.OnSpecialTimeReached += OnSpecialTimeReached;
    }

    public void LoadDialoguesFromCSV()
    {
        try
        {
            Debug.Log($"현재 손님 인덱스:{Spe_customer}");
            string csvFileName = "";
            if (Spe_customer == 0)
                csvFileName = csvFileNameGirl;
            else if (Spe_customer == 1)
                csvFileName = csvFileNameOldMan;
            else if (Spe_customer == 2)
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

    private void OnSpecialTimeReached()
    {
        Debug.Log("3시에 손님 등장 이벤트 발생");
        if (specialOrders.ContainsKey(currentDay))
        {
            VisitSpecialCustomer(currentDay);
        }
    }

    public void orderSpecialCustomer()
    {
        if (Spe_customer >= customers.Count)
        {
            Debug.LogError("스페셜 손님 인덱스가 범위를 초과했습니다!");
            return;
        }
        foreach (GameObject customerObj in customers)
        {
            customerObj.SetActive(false); // 모든 손님 비활성화
        }
        Debug.Log($"특별 손님 등장{Spe_customer}");
        customer = customers[Spe_customer];
        customer.SetActive(true);

        current_startId=1;
        PlayDialogue(current_startId);
    }

    public void VisitSpecialCustomer(int day)
    {
        if (!specialOrders.ContainsKey(day))
        {
            Debug.LogError("특별 손님 방문 데이터가 없습니다!");
            return;
        }

        GameObject customer = specialOrders[day];
        customer.SetActive(true);
        current_startId=1001;
        PlayDialogue(current_startId);
        setmenu.current_cus("오리지널 마들렌", "special"); 
        StartCoroutine(HandleCustomerInteraction(customer, day));

        none.onClick.RemoveAllListeners();
        none.onClick.AddListener(() =>
        {
            Debug.Log($"손님 {customer.name}이(가) 메뉴를 받지 못했습니다.");
            UpdateDialogue("none");
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
                specialOrders[currentDay + 2] = customer;
                customer.SetActive(false);
                EndDialogue();
                return;
            }
            else if(startId==1001){
                speechBubble.SetActive(false);
                MadeMenu.SetActive(true);
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
        specialOrders.Remove(day);
        isOrderCompleted = false;
        Spe_customer++;

        Debug.Log("특별 손님이 방문을 완료했습니다.");
        
    }

    public void UpdateDialogue(string action)
    {
        speechBubble.SetActive(true);
        MadeMenu.SetActive(false);
        if (action.Equals("none"))
        {
            current_startId=4001;
            PlayDialogue(current_startId);
        }
        else if (action.Equals("True"))
        {
            current_startId=2001;
            PlayDialogue(current_startId);
            Debug.Log("특별 손님이 제품을 받아갔습니다!");
        }
        else if (action.Equals("False"))
        {
            current_startId=3001;
            PlayDialogue(current_startId);
            Debug.Log("특별 손님이 제품을 잘못 받아갔습니다!");
        }
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
    }
}
