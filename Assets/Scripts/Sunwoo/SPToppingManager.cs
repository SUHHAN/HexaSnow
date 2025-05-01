using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class SpecialToppingManager : MonoBehaviour
{
    public GameObject startToppingPanel, addToppingPanel, addCreamPanel, addFlowerPanel, finishBakingPanel;
    public TMP_Text talkingText;
    public Button startToppingButton, finishToppingButton, finishCreamButton, finishFlowerButton, finishButton;
    public Image bakingImage;
    public Image oriImage; // 오리지널 이미지 보여주는 용도
    public List<Sprite> dessertSprites;
    public List<Sprite> oriSprites; // 디저트별 오리지널 이미지

    public List<Button> toppingButtons;
    public List<Button> creamButtons;
    public List<Button> flowerButtons;

    private int currentDay;
    private int toppingStage = 0;
    private int selectedDessertIndex = 0;
    private string selectedDessertName = "";

    private List<int> selectedToppingIndices = new List<int>();
    private List<int> selectedCreamIndices = new List<int>();
    private List<int> selectedFlowerIndices = new List<int>();

    public OvenGameManager ovenGameManager;
    public Dictionary<int, string> menuDictionary;

    void Start()
    {
        currentDay = DataManager.Instance.LoadGameData().date;
        SetupPanels();
        SetupToppingButtons();
        SetupCreamButtons();
        SetupFlowerButtons();
    }

    public void SetSelectedDessert(string name, int index)
    {
        selectedDessertName = name;
        selectedDessertIndex = index;

        // 선택된 디저트의 오리지널 이미지 띄우기 (index 1부터 시작이라고 가정)
        if (index - 1 >= 0 && index - 1 < oriSprites.Count)
        {
            oriImage.sprite = oriSprites[index - 1];
        }
        else
        {
            Debug.LogWarning("해당 디저트의 오리지널 이미지가 없습니다.");
        }
    }

    void SetupPanels()
    {
        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        addCreamPanel.SetActive(false);
        addFlowerPanel.SetActive(false);
        finishBakingPanel.SetActive(false);

        talkingText.text = GetTalkingText(currentDay);
        startToppingButton.gameObject.SetActive(false);
        StartCoroutine(EnableStartButtonAfterDelay(2f));

        startToppingButton.onClick.AddListener(HandleStartTopping);
        finishToppingButton.onClick.AddListener(HandleFinishTopping);
        finishCreamButton.onClick.AddListener(HandleFinishCream);
        finishFlowerButton.onClick.AddListener(HandleFinishFlower);
        finishButton.onClick.AddListener(FinishBaking);

        finishToppingButton.gameObject.SetActive(true);
        finishCreamButton.gameObject.SetActive(true);
        finishFlowerButton.gameObject.SetActive(true);
    }

    string GetTalkingText(int day)
    {
        if (day >= 2 && day <= 4)
            return "무작정 화려한 것보단, 어린아이들이 좋아할 만하게 만드는 게 좋겠어.";
        else if (day >= 5 && day <= 7)
        {
            if (!PlayerPrefs.HasKey("SeenDialogue_5to7"))
            {
                PlayerPrefs.SetInt("SeenDialogue_5to7", 1);
                return "아무래도 말씀하셨던 세 가지 재료를 한 번에 쓰긴 쉽지 않을 것 같아… 하나로 만드는 방법 말고 다른 걸 생각해볼까?";
            }
            return "";
        }
        else if (day >= 8 && day <= 10)
            return "아무리 그래도, 온통 파란색인 케이크는 별로일 것 같은데… 메인 크림과 데코 크림을 각각 다른 색으로 해볼까?";

        return "";
    }

    IEnumerator EnableStartButtonAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        startToppingButton.gameObject.SetActive(true);
    }

    void HandleStartTopping()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
    }

    public void ToggleTopping(int index)
    {
        if (selectedToppingIndices.Contains(index))
            selectedToppingIndices.Remove(index);
        else
            selectedToppingIndices.Add(index);

        UpdateButtonVisuals(toppingButtons, selectedToppingIndices);
    }

    public void ToggleCream(int index)
    {
        if (selectedCreamIndices.Contains(index))
            selectedCreamIndices.Remove(index);
        else
            selectedCreamIndices.Add(index);

        UpdateButtonVisuals(creamButtons, selectedCreamIndices);
    }

    public void ToggleFlower(int index)
    {
        if (selectedFlowerIndices.Contains(index))
            selectedFlowerIndices.Remove(index);
        else
            selectedFlowerIndices.Add(index);

        UpdateButtonVisuals(flowerButtons, selectedFlowerIndices);
    }

    void UpdateButtonVisuals(List<Button> buttons, List<int> selectedIndices)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var img = buttons[i].transform.Find("Imageaft")?.GetComponent<Image>();
            if (img == null) continue;

            Color c = img.color;
            c.a = selectedIndices.Contains(i) ? 0.3f : 1f;
            img.color = c;
        }
    }

    void HandleFinishTopping()
    {
        addToppingPanel.SetActive(false);

        if (currentDay >= 2 && currentDay <= 4)
        {
            addCreamPanel.SetActive(true);
        }
        else if (currentDay >= 5 && currentDay <= 7)
        {
            finishBakingPanel.SetActive(true);
        }
        else if (currentDay >= 8 && currentDay <= 10)
        {
            addCreamPanel.SetActive(true);
            toppingStage = 1;
        }
    }

    void HandleFinishCream()
    {
        addCreamPanel.SetActive(false);

        if (currentDay >= 2 && currentDay <= 4)
        {
            finishBakingPanel.SetActive(true);
        }
        else if (currentDay >= 8 && currentDay <= 10)
        {
            addToppingPanel.SetActive(true);
            toppingStage = 2;
        }
    }

    void HandleFinishFlower()
    {
        addFlowerPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
    }

    public void HandleToppingStageAdvance()
    {
        if (toppingStage == 2)
        {
            addToppingPanel.SetActive(false);
            addFlowerPanel.SetActive(true);
        }
    }

    void FinishBaking()
    {
        int resultIndex = 1; // 기본 실패 이미지 인덱스

        if (currentDay >= 2 && currentDay <= 4)
        {
            if (selectedDessertName == "Muffin")
            {
                if (OnlySelected(selectedToppingIndices, 0) && OnlySelected(selectedCreamIndices, 0)) resultIndex = 2;
                else if (OnlySelected(selectedToppingIndices, 1) && OnlySelected(selectedCreamIndices, 1)) resultIndex = 3;
                else resultIndex = 4;
            }
            else if (selectedDessertName == "PoundCake")
            {
                if (OnlySelected(selectedToppingIndices, 0) && OnlySelected(selectedCreamIndices, 0)) resultIndex = 5;
                else if (OnlySelected(selectedToppingIndices, 1) && OnlySelected(selectedCreamIndices, 1)) resultIndex = 6;
                else resultIndex = 7;
            }
        }
        else if (currentDay >= 5 && currentDay <= 7)
        {
            if (selectedDessertName == "PoundCake")
            {
                if (OnlySelected(selectedToppingIndices, 2)) resultIndex = 8;
                else if (OnlySelected(selectedToppingIndices, 3)) resultIndex = 9;
                else if (OnlySelected(selectedToppingIndices, 1)) resultIndex = 10;
                else resultIndex = 11;
            }
            else if (selectedDessertName == "Tart")
            {
                if (OnlySelected(selectedToppingIndices, 3)) resultIndex = 12;
                else resultIndex = 13;
            }
        }
        else if (currentDay >= 8 && currentDay <= 10)
        {
            if (selectedDessertName == "SliceCake")
            {
                if (OnlySelected(selectedCreamIndices, 2, 3) && OnlySelected(selectedToppingIndices, 7) && OnlySelected(selectedFlowerIndices, 0))
                    resultIndex = 14;
                else
                    resultIndex = 15;
            }
        }

        if (resultIndex < dessertSprites.Count)
            bakingImage.sprite = dessertSprites[resultIndex];

        Debug.Log($"선택된 디저트: {selectedDessertName} ({selectedDessertIndex})");
        Debug.Log("토핑: " + string.Join(", ", selectedToppingIndices));
        Debug.Log("크림: " + string.Join(", ", selectedCreamIndices));
        Debug.Log("플라워: " + string.Join(", ", selectedFlowerIndices));

        SaveBakingResult(resultIndex);
    }

    private bool OnlySelected(List<int> list, params int[] targets)
    {
        if (list.Count != targets.Length) return false;
        foreach (int t in targets)
            if (!list.Contains(t)) return false;
        return true;
    }

    private void SaveBakingResult(int imageIndex)
    {
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        int totalScore = ovenGameManager.GetTotalScore();

        if (imageIndex == 21 || imageIndex == 30)
        {
            totalScore = 0;
        }

        string finalDessertName = menuDictionary.ContainsKey(imageIndex) ? menuDictionary[imageIndex] : "알 수 없음";

        Debug.Log($"최종 총점: {totalScore}");
        Debug.Log($"최종 저장할 디저트: {finalDessertName}");

        MyRecipeList newRecipe = new MyRecipeList(
            DataManager.Instance.gameData.myBake.Count + 1,
            imageIndex,
            finalDessertName,
            totalScore,
            false
        );

        DataManager.Instance.gameData.myBake.Add(newRecipe);
        DataManager.Instance.SaveGameData();

        Debug.Log($"저장 완료: {finalDessertName} | 이미지 인덱스: {imageIndex} | 점수: {totalScore}");
    }

    private void SetupToppingButtons()
    {
        for (int i = 0; i < toppingButtons.Count; i++)
        {
            int index = i;
            toppingButtons[i].onClick.RemoveAllListeners();
            toppingButtons[i].onClick.AddListener(() => ToggleTopping(index));
        }
    }

    private void SetupCreamButtons()
    {
        for (int i = 0; i < creamButtons.Count; i++)
        {
            int index = i;
            creamButtons[i].onClick.RemoveAllListeners();
            creamButtons[i].onClick.AddListener(() => ToggleCream(index));
        }
    }

    private void SetupFlowerButtons()
    {
        for (int i = 0; i < flowerButtons.Count; i++)
        {
            int index = i;
            flowerButtons[i].onClick.RemoveAllListeners();
            flowerButtons[i].onClick.AddListener(() => ToggleFlower(index));
        }
    }
}
