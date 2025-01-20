using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    public GameObject postman;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public GameObject order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 주문 텍스트 표시용
    public TextMeshProUGUI orderCustomer;
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 주문 데이터 저장 리스트
    private List<DialogueLine> filteredDialogues = new List<DialogueLine>();
    private List<string> nicknames = new List<string>();
    public List<int> order_menu_id=new List<int>();
    public List<string> order_nickname=new List<string>();
    public List<int> order_deadLine=new List<int>();
    private int currentDialogueIndex; // 현재 주문 인덱스
    private int currentNicknameIndex; //현재 손님 인덱스
    public string csvFileName = "orderStatement_main.csv"; // CSV 파일 이름
    public string csvFileName_nickname="orderStatement_nickname.csv";
    public Button acceptButton; // 수락 버튼
    public Button cancelButton; // 취소 버튼
    public Button orderCheck;
     public GameObject speechBubble;
    public GameObject nameBubble;
    private bool isAcceptButtonClicked = false;
    private int order_count=0;
    private int accept_order=2;
    private int deadline=1;

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
        postman.SetActive(true);
        orderCheck.gameObject.SetActive(true);
        order.SetActive(true); // 주문 UI 비활성화

        acceptButton.onClick.AddListener(() => {
        order_menu_id.Add(currentDialogueIndex); // 현재 대화 인덱스를 ID로 추가
        order_nickname.Add(nicknames[currentNicknameIndex]);
        order_deadLine.Add(deadline);
        
        NextDialogue(); // 다음 대화 진행
    });
        cancelButton.onClick.AddListener(CloseDialogue); // 취소 버튼 클릭 시 대화 종료
        orderCheck.onClick.AddListener(OpenOrderUI);
        LoadDialoguesFromCSV(); // CSV 파일 로드
        LoadNicknameFromCSV();
        Postmanment();

        openMenu();
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
        orderCheck.gameObject.SetActive(false);
        order.SetActive(true);
        SetRandomDialogueIndex();
        ShowDialogue();

    }
    private void Postmanment(){
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
        order.SetActive(false); // UI 비활성화
        currentDialogueIndex = 0; // 대화 인덱스 초기화
        postman.SetActive(false);
        speechBubble.SetActive(false);
        nameBubble.SetActive(false);
        Debug.Log("수락한 메뉴의 ID:");
  /*      foreach (int id in order_menu_id)
     {
            Debug.Log(id);
     }*/
     foreach (string nickname in order_nickname)
     {
        Debug.Log(nickname);
     }
     foreach (int deadline in order_deadLine)
     {
        Debug.Log(order_deadLine.Count);

     }
    
    }
 private void SetRandomDialogueIndex()
    {
        currentDialogueIndex = Random.Range(3, dialogues.Count); // 랜덤으로 인덱스 선택
        Debug.Log($"랜덤으로 선택된 인덱스: {currentDialogueIndex}");
        currentNicknameIndex=Random.Range(1, nicknames.Count);
        
    }
public void IncreaseAcceptOrder(int increment){
    accept_order+=increment;
    }

public void openMenu(){
     int maxId=dayChange.day*2000+1000;

     filteredDialogues=dialogues.FindAll(dialogue=>{
        if (int.TryParse(dialogue.id, out int dialogueId)){
            return dialogueId<=maxId;
        }
        return false;
     });

     if (filteredDialogues.Count == 0)
    {
        Debug.LogWarning($"현재 날짜({dayChange.day})에 허용된 대화가 없습니다!");
    }
    else
    {
        Debug.Log($"현재 날짜({dayChange.day})에 허용된 대화 개수: {filteredDialogues.Count}");
    }
}

public void ResetOrderSystem(int day)
    {
        Debug.Log($"Resetting Order System for Day {day}");
        InitializeOrderSystem();
    }
    private void InitializeOrderSystem()
    {
        // 초기화 로직
        postman.SetActive(true);
        orderCheck.gameObject.SetActive(true);
        order.SetActive(true);
        order_menu_id.Clear();
        order_nickname.Clear();
        order_deadLine.Clear();
        order_count = 0;
        isAcceptButtonClicked = false;
        Postmanment();
    }
}