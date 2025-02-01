using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Order : MonoBehaviour
{
    public GameTime gametime;
    public getMenu getMenuScript;
    public special_customer SpecialScript;
    public DayChange dayChange;
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

    public GameObject popup;
    public TextMeshProUGUI popupText;
    private int openmenuIndex=1;
    private bool isPopupCoroutineRunning=false;
    public GameObject Mademenu;



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
        AudioManager.Instance.PlayBgm(AudioManager.Bgm.inside_kitchen_baking);
        SceneManager.LoadScene("Main", LoadSceneMode.Additive); //기본 UI 띄우기 
        postman.SetActive(true);
        orderCheck.gameObject.SetActive(true);
        order.SetActive(true); // 주문 UI 비활성화
        Mademenu.SetActive(false);

        InitializeButtons(); // 버튼 초기화
        LoadDialoguesFromCSV(); // CSV 파일 로드
        LoadNicknameFromCSV();
        LoadMenuForPopup(); // 메뉴 해금 팝업용 CSV 로드
        Postmanment();

        openMenu(1);
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
        order.SetActive(true);
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
            
            CloseDialogue(); // 대화 종료
        }
        else
        {
            ShowDialogue(); // 다음 내용 표시
        }
    }

    private void CloseDialogue()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
        gametime.StartGameTimer();
        order.SetActive(false); // UI 비활성화
        currentDialogueIndex = 0; // 대화 인덱스 초기화
        postman.SetActive(false);
        speechBubble.SetActive(false);
        nameBubble.SetActive(false);

     int previousDay = getMenuScript.currentDay - 1;
    
        if (getMenuScript.gameObject.activeSelf)
        {
            getMenuScript.StartCoroutine(getMenuScript.ProcessCustomers(previousDay));
        }
    else
    {
        Debug.LogError("getMenuScript가 비활성화 상태입니다. 코루틴 실행 불가!");
    }
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
public void IncreaseAcceptOrder(int increment){
    accept_order+=increment;
    }
private void InitializeButtons(){
    // 기존 리스너 제거 후 새로 추가
    acceptButton.onClick.RemoveAllListeners();
    acceptButton.onClick.AddListener(() => {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.recipe_order);
        order_menu_id = currentDialogueIndex;
        order_deadLine.Add(deadline);
        
        getMenuScript.ReceiveOrders(currentNicknameIndex, order_menu_id);
        NextDialogue();
    });

    cancelButton.onClick.RemoveAllListeners();
    cancelButton.onClick.AddListener(CloseDialogue);

    orderCheck.onClick.RemoveAllListeners();
    orderCheck.onClick.AddListener(OpenOrderUI);
}

public void openMenu(int day){
     int maxId=day*2000+1000;

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

public void ResetOrderSystem(int day)
    {
        postman.SetActive(true);
        orderCheck.gameObject.SetActive(true);
        order.SetActive(true);
        order_nickname.Clear();
        order_deadLine.Clear();
        speechBubble.SetActive(true);
        nameBubble.SetActive(true);
        order_count = 0;
        Postmanment();
        
    }
    private void showPopup(){
        Debug.Log($"openmenuIndex, unlocked_menu.Count {openmenuIndex, nlocked_menu.Count}");
    if (openmenuIndex < unlocked_menu.Count)
    {
        string unlockedMenu1 = unlocked_menu[openmenuIndex];
        string unlockedMenu2 = (openmenuIndex + 1 < unlocked_menu.Count) ? unlocked_menu[openmenuIndex + 1] : null;

        if (!string.IsNullOrEmpty(unlockedMenu1) && !string.IsNullOrEmpty(unlockedMenu2))
        {
            popup.SetActive(true);
            popupText.text = $"{unlockedMenu1}, {unlockedMenu2}\n레시피가 해금되었습니다!";
        }
        
        openmenuIndex += 2;
}
    }
    void Update()
    {
    // 팝업이 활성화된 상태에서 클릭 감지
    if (popup.activeSelf && Input.GetMouseButtonDown(0))
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        popup.SetActive(false); // 팝업 비활성화
    }
}
}