using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    public GameObject order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 주문 텍스트 표시용
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 주문 데이터 저장 리스트
    private int currentDialogueIndex; // 현재 주문 인덱스
    public string csvFileName = "postmanDialogues.csv"; // CSV 파일 이름
    public Button acceptButton; // 수락 버튼
    public Button cancelButton; // 취소 버튼

    private int order_count=0;
    private int accept_order=2;
    private int day=1;
  public struct DialogueLine{
    public string id;
    public string menu;
    //public string nickname;
    public string description;

    public DialogueLine(string id, string menu, string description){
        this.id=id;
        this.menu=menu;
        this.description=description;

    }
  }
    void Start()
    {
        order.SetActive(true); // 주문 UI 활성화
        //order.SetActive(false); // 주문 UI 비활성화
        acceptButton.onClick.AddListener(NextDialogue); // 수락 버튼 클릭 시 대화 진행
        cancelButton.onClick.AddListener(CloseDialogue); // 취소 버튼 클릭 시 대화 종료
        LoadDialoguesFromCSV(); // CSV 파일 로드

        if (dialogues.Count>0){
             SetRandomDialogueIndex(); // 랜덤 인덱스 설정
            ShowDialogue(); // 첫 대화 표시
    }
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
    private void ShowDialogue()
    {
        order_count++;

        if (dialogues.Count > currentDialogueIndex)
        {
             DialogueLine currentLine = dialogues[currentDialogueIndex];
            dialogueOrder.text = currentLine.description;

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
        day++;
    }
 private void SetRandomDialogueIndex()
    {
        currentDialogueIndex = Random.Range(1, dialogues.Count); // 랜덤으로 인덱스 선택
        Debug.Log($"랜덤으로 선택된 인덱스: {currentDialogueIndex}");
    }
}
