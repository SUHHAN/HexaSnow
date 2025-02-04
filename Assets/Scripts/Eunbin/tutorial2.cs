using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorial2 : MonoBehaviour
{
    public GameObject speechBubble;
    public GameObject direct1;
    public GameObject direct2;
    public GameObject direct3;
    public GameObject kitchen;
    public GameObject order;
    public TextMeshProUGUI dialogueText;

    private List<DialogueLine> dialogues = new List<DialogueLine>();
    private int currentDialogueIndex = 1;
    public string csvFileName = "tutorial2.csv";

    public struct DialogueLine
    {
        public string id;
        public string dialogue;

        public DialogueLine(string id, string dialogue)
        {
            this.id = id;
            this.dialogue = dialogue;
        }
    }

    private void Start()
    {
        AudioManager.Instance.PlayBgm(AudioManager.Bgm.main_bonus_ingre); 
        LoadDialoguesFromCSV();
        if (dialogues.Count > 0)
        {
            ShowDialogue();
        }
        else
        {
            Debug.LogError("대화 내용이 없습니다. CSV 파일을 확인하세요.");
        }
    }

    private void LoadDialoguesFromCSV()
{
    try
    {
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
            if (fields.Length < 2) continue;

            string id = fields[0].Trim();
            string dialogue = fields[1].Trim();

            dialogues.Add(new DialogueLine(id, dialogue));
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

    public void ShowDialogue()
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            DialogueLine currentLine = dialogues[currentDialogueIndex];
            UpdateDialogueUI(currentLine);

            if(currentLine.id=="3"){
                direct1.SetActive(true);
            }
            if(currentLine.id=="4"){
                direct3.SetActive(true);
            }
            if(currentLine.id=="5"){
                direct3.SetActive(false);
            }

            if(currentLine.id=="7"){
                direct1.SetActive(false);
                direct2.SetActive(true);
            }
            if(currentLine.id=="9"){
                direct2.SetActive(false);
            }
        else
        {
        
        }
    }
    }

    private void UpdateDialogueUI(DialogueLine line)
    {

        dialogueText.text = line.dialogue;
    }

    private void Update()
    {
        if ((speechBubble.activeSelf) && Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= dialogues.Count)
        {

            //speechBubble.SetActive(false);
            //letterBubble.SetActive(false);
            //postman.SetActive(false);
            SceneManager.LoadScene("order1");
        }
        else
        {
            ShowDialogue();
        }
    }
}
