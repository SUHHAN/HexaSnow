using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
public class getMenu : MonoBehaviour
{
    public Order orderScript; // Order 스크립트 참조
    public GameObject oldMan;
    public GameObject man;
    public GameObject child;
    public Button none;
    private Dictionary<int, List<List<int>>> dailyOrders = new Dictionary<int, List<List<int>>>(); // 일별 주문 저장
    private List<GameObject> customers = new List<GameObject>();
    public string csvFileName = "orderStatement_main.csv";
    public string csvFileName_nickname="orderStatement_nickname.csv";
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 주문 데이터 저장 리스트
    private List<string> nicknames = new List<string>();
    public int currentDay = 1; // 현재 날짜를 추적
    public GameObject customer_order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 주문 텍스트 표시용
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public GameObject speechBubble;
    public GameObject nameBubble;
    private int NicknameIndex=0;
    bool isOrderCompleted = false;

    public GameObject MadeMenu;
    public string menuName;
    
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
        oldMan.SetActive(false);
        child.SetActive(false);
        man.SetActive(false);
        customer_order.SetActive(false);
        speechBubble.SetActive(false);
        LoadDialoguesFromCSV(); // CSV 파일 로드
        LoadNicknameFromCSV();
        customers.Add(oldMan);
        customers.Add(man);
        customers.Add(child);
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

    public void ReceiveOrders(int NicknameIndex, int order_id)
{
    if (!dailyOrders.ContainsKey(currentDay))
        {
            dailyOrders[currentDay] = new List<List<int>>();
        }
    Debug.Log($"현재 {currentDay}일의 주문 상태: {dailyOrders[currentDay].Count}개의 주문이 있습니다.");
    dailyOrders[currentDay].Add(new List<int> { order_id, NicknameIndex});

    }

    public IEnumerator ProcessCustomers(int dayToProcess)
{
    Debug.Log($"[{dayToProcess}일] 손님 처리 시작");
    MadeMenu.SetActive(true);

    if (!dailyOrders.ContainsKey(dayToProcess) || dailyOrders[dayToProcess].Count == 0)
    {
        Debug.Log($"[{dayToProcess}일] 처리할 주문이 없습니다.");
        MadeMenu.SetActive(false);
        yield break;
    }
    List<List<int>> menuForDay = new List<List<int>>(dailyOrders[dayToProcess]); // 해당 날짜의 주문 복사

    foreach (List<int> order in menuForDay)
    {
        int orderId = order[0];
        int nicknameIndex = order[1];
        menuName=dialogues[orderId].menu;

        GameObject customer = GetRandomCustomer();
        customer.SetActive(true);
        dialogueName.text = nicknames[nicknameIndex];

        Debug.Log($"손님 {customer.name}이(가) 메뉴 {menuName}을(를) 받으러 왔습니다!");
        ShowOrder(orderId);
        nameBubble.SetActive(true);

        isOrderCompleted = false;

        none.onClick.RemoveAllListeners();
        
        none.onClick.AddListener(() => {
            Debug.Log($"손님 {customer.name}이(가) 메뉴를 받지 못했습니다.");
            UpdateDialogue("none");
            isOrderCompleted = true;
        });

        yield return new WaitUntil(() => isOrderCompleted);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        customer.SetActive(false);
        customer_order.SetActive(false);
        speechBubble.SetActive(false);

        Debug.Log($"손님 {customer.name}이(가) 메뉴를 받아갔습니다!");
    }

    dailyOrders[dayToProcess].Clear(); // 해당 날짜의 주문 처리 완료
    Debug.Log($"[{dayToProcess}일] 모든 손님이 메뉴를 받아갔습니다.");
    MadeMenu.SetActive(false);
}

    private GameObject GetRandomCustomer(){
        int randomIndex=Random.Range(0, customers.Count);
        return customers[randomIndex];
    }

    private void ShowOrder(int order){
        customer_order.SetActive(true);
        dialogueOrder.text = dialogues[order].description;
        speechBubble.SetActive(true);
        dialogueText.text="주문한 거 나왔나요?";

    }
    public void UpdateDialogue(string action){
        if(action.Equals("none")){
            dialogueText.text="안만들었다고요?";
        }
        else if(action.Equals("True")){
            dialogueText.text="감사합니다.";
            isOrderCompleted = true;
        }
        else if(action.Equals("False")){
            dialogueText.text="이거 아니잖아요!";   
            isOrderCompleted = true;         
        }
    }
    
}