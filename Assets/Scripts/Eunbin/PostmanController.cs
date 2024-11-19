using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PostmanController : MonoBehaviour
{
    public GameObject speechBubble; // 말풍선 UI 오브젝트
    public GameObject order; 
    public TextMeshProUGUI dialogueText; // 대화 텍스트 표시용
    public TextMeshProUGUI dialogueName; // 이름 표시용
    private List<DialogueLine> dialogues = new List<DialogueLine>(); // 대화 데이터를 저장할 리스트
    private int currentDialogueIndex = 0; // 현재 대화 인덱스
    public string csvFileName = "postmanDialogues.csv"; // CSV 파일 이름

    public struct DialogueLine
    {
        public string name; // 화자 이름
        public string dialogue; // 대화 내용

        public DialogueLine(string name, string dialogue)
        {
            this.name = name;
            this.dialogue = dialogue;
        }
    }

    private void Start()
    {
        speechBubble.SetActive(false); // 게임 시작 시 말풍선 숨기기
        //order.SetActive(false); 
        LoadDialoguesFromCSV(); // CSV 파일에서 대화 로드
    }

    // CSV 파일에서 대화 내용 로드
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
            string[] lines = csvFile.text.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // 빈 줄 건너뛰기

                // 쉼표로 구분된 데이터 파싱
                string[] fields = line.Split(',');
                if (fields.Length < 3) continue; // 필드가 부족한 줄 건너뛰기

                string name = fields[1].Trim();
                string dialogue = fields[2].Trim();
                dialogues.Add(new DialogueLine(name, dialogue)); // 대화 데이터 추가
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

    // 포스트맨 클릭 시 말풍선과 대화 시작
    private void OnMouseDown()
    {
        if (!speechBubble.activeSelf)
        {
            ShowDialogue(); // 말풍선과 대화 시작
        }
    }

    // 말풍선에 대화 텍스트 표시
    private void ShowDialogue()
    {
        if (dialogues.Count > 0) // 대화 내용이 있을 때만 표시
        {
            speechBubble.SetActive(true); // 말풍선 나타내기
            dialogueName.text = dialogues[currentDialogueIndex].name; // 이름 표시
            dialogueText.text = dialogues[currentDialogueIndex].dialogue; // 첫 번째 대화 표시
        }
        else
        {
            Debug.LogWarning("대화 내용이 없습니다");
        }
    }

    // 아무 곳이나 클릭하면 대화가 넘어가게 하기
    private void Update()
    {
        if (speechBubble.activeSelf && Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            NextDialogue();
        }
    }

    // 대화 내용 진행
    private void NextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= dialogues.Count) // 대화가 끝났다면
        {
            speechBubble.SetActive(false); // 말풍선 숨기기
            currentDialogueIndex = 0; // 대화 인덱스 초기화
            //order.SetActive(true);
        }
        else
        {
            dialogueName.text = dialogues[currentDialogueIndex].name; // 이름 업데이트
            dialogueText.text = dialogues[currentDialogueIndex].dialogue; // 다음 대화 내용 표시
        }
    }
}
