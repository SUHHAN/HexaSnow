using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class tutorial : MonoBehaviour
{
    public PostmanController postman;
    public GameObject Tutorial;
    public GameObject front;
    public GameObject room;
    public GameObject speechBubble;
    public GameObject nameBubble;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;

    private List<DialogueLine> dialogues = new List<DialogueLine>();
    private int currentDialogueIndex = 1;
    public string csvFileName = "tutorial.csv";

    public struct DialogueLine
    {
        public string id;
        public string name;
        public string dialogue;

        public DialogueLine(string id, string name, string dialogue)
        {
            this.id = id;
            this.name = name;
            this.dialogue = dialogue;
        }
    }
    private void Start()
    {
        LoadDialoguesFromCSV();
        AudioManager.Instance.StopBgm();

        
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
                if (fields.Length < 3) continue;

                string id = fields[0].Trim();
                string name = fields[1].Trim();
                string dialogue = fields[2].Trim();

                dialogues.Add(new DialogueLine(id, name, dialogue));
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

    private void ShowDialogue()
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            DialogueLine currentLine = dialogues[currentDialogueIndex];
                speechBubble.SetActive(true);
                UpdateDialogueUI(currentLine);
                
                if(currentLine.id=="6"){
                    AudioManager.Instance.PlayBgm(AudioManager.Bgm.tutorial);
                }
                if(currentLine.id=="8"){
                    room.SetActive(false);
                }
                if(currentLine.id=="14"){
                    Tutorial.SetActive(false);
                }
                if(currentLine.id=="16"){
                    front.SetActive(false);
                    AudioManager.Instance.PlaySfx(AudioManager.Sfx.bell);
                }
                if(currentLine.id=="19")
                    AudioManager.Instance.StopBgm();
                    
        }
        else
        {
            Debug.LogWarning("대화가 끝났습니다.");


        }
    }

    private void UpdateDialogueUI(DialogueLine line)
    {
        if (string.IsNullOrWhiteSpace(line.name))
        {
            nameBubble.SetActive(false);
        }
        else
        {
            nameBubble.SetActive(true);
            dialogueName.text = line.name;
        }

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
            speechBubble.SetActive(false);
            postman.ShowDialogue();
        }
        else
        {
            ShowDialogue();
        }
    }
}
