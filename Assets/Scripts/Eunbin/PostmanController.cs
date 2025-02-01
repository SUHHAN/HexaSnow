using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PostmanController : MonoBehaviour
{

    public GameObject postman;
    public GameObject speechBubble;
    public GameObject nameBubble;
    public GameObject letterBubble;
    public GameObject letter;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI letterText;
    public GameObject recipe;


    private List<DialogueLine> dialogues = new List<DialogueLine>();
    private int currentDialogueIndex = 1;
    public string csvFileName = "postmanDialogues.csv";

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
        speechBubble.SetActive(false);
        nameBubble.SetActive(false);
        letterBubble.SetActive(false);
        postman.SetActive(false);
        LoadDialoguesFromCSV();

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

    public void ShowDialogue()
    {
        postman.SetActive(true);
        if (currentDialogueIndex < dialogues.Count)
        {
            DialogueLine currentLine = dialogues[currentDialogueIndex];
            if(currentLine.id=="1"){
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.bell); // 효과음 재생
            }

                if(currentLine.id=="20")
                    letter.SetActive(true);
                    
                if(currentLine.id=="26")
                    recipe.SetActive(true);
            if (currentLine.id == "27")
            {
                // 편지 UI 활성화
                letter.SetActive(false);
                letterBubble.SetActive(true);
                letterText.text = currentLine.dialogue.Replace("\\n", "\n");
                speechBubble.SetActive(false);
                nameBubble.SetActive(false);
            }
            else
            {
                // 일반 대화
                speechBubble.SetActive(true);
                UpdateDialogueUI(currentLine);
                letterBubble.SetActive(false);
            }
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
        if ((speechBubble.activeSelf || letterBubble.activeSelf) && Input.GetMouseButtonDown(0))
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
            SceneManager.LoadScene("tutorial2");
        }
        else
        {
            ShowDialogue();
        }
    }
}
