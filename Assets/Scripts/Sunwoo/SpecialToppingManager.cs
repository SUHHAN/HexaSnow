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
    public Image oriImage, oriImage2, oriImage3, oriImage4;

    public List<Sprite> dessertSprites;
    public List<Sprite> oriImageSprites;

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
    public SPBakingStartManager bakingStartManager;
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

        Dictionary<int, int> dessertIndexToOriIndex = new Dictionary<int, int>()
    {
        { 1, 1 }, { 4, 2 }, { 7, 3 }, { 10, 4 },
        { 14, 5 }, { 15, 6 }, { 18, 7 }, { 21, 8 },
        { 25, 9 }, { 28, 10 }, { 31, 11 }
    };

        if (dessertIndexToOriIndex.ContainsKey(selectedDessertIndex))
        {
            int oriIndex = dessertIndexToOriIndex[selectedDessertIndex];
            if (oriIndex - 1 < oriImageSprites.Count)
            {
                oriImage.sprite = oriImageSprites[oriIndex - 1];
                oriImage2.sprite = oriImageSprites[oriIndex - 1];
                oriImage3.sprite = oriImageSprites[oriIndex - 1];
                oriImage4.sprite = oriImageSprites[oriIndex - 1];
                Debug.Log($"[OriImage] {selectedDessertName} → oriIndex {oriIndex}");
            }
            else
            {
                Debug.LogError($"[OriImage] oriIndex {oriIndex}는 oriImageSprites 범위를 벗어났습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[OriImage] {selectedDessertIndex}는 매핑된 디저트 인덱스가 아닙니다.");
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
        talkingText.gameObject.SetActive(true);
        startToppingButton.gameObject.SetActive(false);
        StartCoroutine(HideTalkingTextAfterDelay(5f));

        startToppingButton.onClick.AddListener(HandleStartTopping);
        finishToppingButton.onClick.AddListener(HandleFinishTopping);
        finishCreamButton.onClick.AddListener(HandleFinishCream);
        finishFlowerButton.onClick.AddListener(HandleFinishFlower);
        finishButton.onClick.AddListener(FinishBaking);

        finishToppingButton.gameObject.SetActive(true);
        finishCreamButton.gameObject.SetActive(true);
        finishFlowerButton.gameObject.SetActive(true);

        EnableToppingsByDay();
    }

    void EnableToppingsByDay()
    {
        foreach (var btn in toppingButtons) btn.gameObject.SetActive(false);
        foreach (var btn in creamButtons) btn.gameObject.SetActive(false);
        foreach (var btn in flowerButtons) btn.gameObject.SetActive(false);

        if (currentDay >= 2 && currentDay <= 4)
        {
            foreach (int i in new int[] { 0, 1, 2, 3 }) toppingButtons[i].gameObject.SetActive(true);
            foreach (int i in new int[] { 0, 1, 2, 3 }) creamButtons[i].gameObject.SetActive(true);
        }
        else if (currentDay >= 5 && currentDay <= 7)
        {
            foreach (int i in new int[] { 4, 5, 1, 0 }) toppingButtons[i].gameObject.SetActive(true);
        }
        else if (currentDay >= 8 && currentDay <= 10)
        {
            foreach (int i in new int[] { 0, 1, 4, 5 }) creamButtons[i].gameObject.SetActive(true);
            foreach (var btn in flowerButtons) btn.gameObject.SetActive(true);
        }
    }

    IEnumerator HideTalkingTextAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        talkingText.gameObject.SetActive(false);
        startToppingButton.gameObject.SetActive(true);
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

    public void ToggleTopping(int index)
    {
        if (selectedToppingIndices.Contains(index)) selectedToppingIndices.Remove(index);
        else selectedToppingIndices.Add(index);
        UpdateButtonVisuals(toppingButtons, selectedToppingIndices);
    }

    public void ToggleCream(int index)
    {
        if (selectedCreamIndices.Contains(index)) selectedCreamIndices.Remove(index);
        else selectedCreamIndices.Add(index);
        UpdateButtonVisuals(creamButtons, selectedCreamIndices);
    }

    public void ToggleFlower(int index)
    {
        if (selectedFlowerIndices.Contains(index)) selectedFlowerIndices.Remove(index);
        else selectedFlowerIndices.Add(index);
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

    void HandleStartTopping()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
    }

    void HandleFinishTopping()
    {
        addToppingPanel.SetActive(false);
        if (currentDay >= 2 && currentDay <= 4) addCreamPanel.SetActive(true);
        else if (currentDay >= 5 && currentDay <= 7) { finishBakingPanel.SetActive(true); UpdateBakingImage(); }
        else if (currentDay >= 8 && currentDay <= 10) { addCreamPanel.SetActive(true); toppingStage = 1; }
    }

    void HandleFinishCream()
    {
        addCreamPanel.SetActive(false);
        if (currentDay >= 2 && currentDay <= 4) { finishBakingPanel.SetActive(true); UpdateBakingImage(); }
        else if (currentDay >= 8 && currentDay <= 10) { addToppingPanel.SetActive(true); toppingStage = 2; }
    }

    void HandleFinishFlower()
    {
        addFlowerPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
        UpdateBakingImage();
    }

    public void HandleToppingStageAdvance()
    {
        if (toppingStage == 2)
        {
            addToppingPanel.SetActive(false);
            addFlowerPanel.SetActive(true);
        }
    }

    void UpdateBakingImage()
    {
        int resultIndex = 1;

        if (currentDay >= 2 && currentDay <= 4)
        {
            if (selectedDessertIndex == 7) // 머핀
            {
                if (OnlySelected(selectedToppingIndices, 0) && OnlySelected(selectedCreamIndices, 0)) resultIndex = 3;
                else if (OnlySelected(selectedToppingIndices, 1) && OnlySelected(selectedCreamIndices, 1)) resultIndex = 4;
                else resultIndex = 5;
            }
            else if (selectedDessertIndex == 10) // 파운드케이크
            {
                if (OnlySelected(selectedToppingIndices, 0) && OnlySelected(selectedCreamIndices, 0)) resultIndex = 6;
                else if (OnlySelected(selectedToppingIndices, 1) && OnlySelected(selectedCreamIndices, 1)) resultIndex = 7;
                else resultIndex = 8;
            }
        }
        else if (currentDay >= 5 && currentDay <= 7)
        {
            if (selectedDessertIndex == 10) // 파운드케이크
            {
                if (OnlySelected(selectedToppingIndices, 4)) resultIndex = 9;
                else if (OnlySelected(selectedToppingIndices, 5)) resultIndex = 10;
                else if (OnlySelected(selectedToppingIndices, 1)) resultIndex = 11;
                else resultIndex = 12;
            }
            else if (selectedDessertIndex == 21) // 타르트
            {
                if (OnlySelected(selectedToppingIndices, 5)) resultIndex = 13;
                else resultIndex = 14;
            }
        }
        else if (currentDay >= 8 && currentDay <= 10)
        {
            if (selectedDessertIndex == 28) // 조각케이크
            {
                if (OnlySelected(selectedCreamIndices, 4, 5) &&
                    OnlySelected(selectedToppingIndices, 0) &&
                    OnlySelected(selectedFlowerIndices, 0))
                {
                    resultIndex = 15;
                }
                else
                {
                    resultIndex = 16;
                }
            }
        }

        if (resultIndex <= dessertSprites.Count)
        {
            bakingImage.sprite = dessertSprites[resultIndex - 1];
            Debug.Log($"[BakingImage] 결과 이미지 인덱스: {resultIndex}");
        }
        else
        {
            Debug.LogError($"[BakingImage] resultIndex {resultIndex}는 dessertSprites 범위를 벗어남");
        }

        SaveBakingResult(resultIndex);
    }

    void FinishBaking()
    {
        SceneManager.LoadScene("BakingStart");
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