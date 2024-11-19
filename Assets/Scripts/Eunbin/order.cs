using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    public GameObject order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 대화 텍스트 표시용
    private List<string> dialogues = new List<string>(); // 대화 데이터 저장 리스트
    private int currentDialogueIndex = 0; // 현재 대화 인덱스
    public string csvFileName = "postmanDialogues.csv"; // CSV 파일 이름
    public Button acceptButton; // 수락 버튼
    public Button cancelButton; // 취소 버튼

    void Start()
    {
        order.SetActive(true); // 주문 UI 활성화
        acceptButton.onClick.AddListener(NextDialogue); // 수락 버튼 클릭 시 대화 진행
        cancelButton.onClick.AddListener(CloseDialogue); // 취소 버튼 클릭 시 대화 종료
        LoadDialoguesFromCSV(); // CSV 파일 로드
        ShowDialogue(); // 첫 대화 표시
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
            string[] lines = csvFile.text.Split('\n');
            foreach (string line in lines)
            {
                // 쉼표로 구분된 데이터 파싱
                string[] fields = line.Split(',');
                if (fields.Length > 1)
                {
                    dialogues.Add(fields[1].Trim()); // Dialogue 컬럼 추가
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

    private void ShowDialogue()
    {
        if (dialogues.Count > 0) // 대화 내용이 있을 때만 표시
        {
            order.SetActive(true);
            dialogueOrder.text = dialogues[currentDialogueIndex]; // 대화 텍스트 업데이트
        }
        else
        {
            Debug.LogWarning("대화 내용이 없습니다");
        }
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= dialogues.Count) // 대화가 끝났다면
        {
            CloseDialogue(); // 대화 종료
        }
        else
        {
            dialogueOrder.text = dialogues[currentDialogueIndex]; // 다음 내용 표시
        }
    }

    private void CloseDialogue()
    {
        order.SetActive(false); // UI 비활성화
        currentDialogueIndex = 0; // 대화 인덱스 초기화
    }
}
