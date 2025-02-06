
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
// using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;

public class InventoryManager_h : MonoBehaviour
{
    public Button orderScene;
    private static InventoryManager _instance;
    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManager>();
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class MyCook
    {
        public int index;
        public int menuID;
        public string name;
        public int score;
        public bool bonus;

        public MyCook(int index, int menuID, string name, int score, bool bonus)
        {
            this.index = index;
            this.menuID = menuID;
            this.name = name;
            this.score = score;
            this.bonus = bonus;
        }
    }

    [System.Serializable]
    public class MyIngre
    {
        public int index;
        public string name;
        public int type;
        public int num;

        public MyIngre(int index, string name, int type, int num)
        {
            this.index = index;
            this.name = name;
            this.type = type;
            this.num = num;
        }
    }



    public List<int> ingredientNum = new List<int>(); // 재료 개수 저장 리스트
    [SerializeField] private GameData ingredientGD = new GameData();

    private string recipe_csvFileName = "recipe.csv"; // CSV 파일명
    private string ingredient_csvFileName = "ingredient.csv"; // CSV 파일명

    public List<MyRecipeList> MyCookList = new List<MyRecipeList>(); // 재료 리스트
    public List<MyIngre> MyIngreList = new List<MyIngre>(); // 재료 리스트

    // public List<int> Menu_Num = new List<int>();


    [Header("이미지 관리")]
    [SerializeField] private Sprite[] CookSprites;
    [SerializeField] private Sprite[] IngredientSprites;


    [Header("칸 관리 Prefab")]
    public GameObject IngrePrefab; // 아이템 Prefab
    public GameObject CookPrefab; // 아이템 Prefab
    public Transform content; // Content 오브젝트

    [Header("탭 관리")]
    public GameObject AllPanel;
    public GameObject IngreButton; // 재료 탭
    public GameObject CookButton; // 요리 탭
    public GameObject xButton; // x 탭
    public GameObject TabSet; // Tab 세트
    public GameObject SelectInventoryButton;


    [SerializeField] private GameObject BonusPanel;
    [SerializeField] private GameObject BlackBackground;
    [SerializeField] private TextMeshProUGUI BonusPanelText;

    [SerializeField] private GameData GD = new GameData();

    

    void Start()
    {
        // 재료 데이터 업로드
        LoadIngredientsFromCSV();
        LoadCookData();

        BonusPanel.SetActive(false);
        BlackBackground.SetActive(false);
        AllPanel.SetActive(false);
        TabSet.SetActive(false);

        // Ingre_AddItems();
        orderScene.onClick.AddListener(()=>{
            SceneManager.LoadScene("Deadline_Last");
        });
        
    }

    public void OnClickSelectButton() {
        AllPanel.SetActive(true);
        BlackBackground.SetActive(true);
        TabSet.SetActive(true);

        // 완성 음식 데이터 업로드
        OnClickIngreTab();
    }

    public void OnClickxTab() {
        BlackBackground.SetActive(false);
        TabSet.SetActive(false);
        AllPanel.SetActive(false);
    }

    public void OnClickIngreTab()
    {
        // 기존 아이템 삭제 후 재료 아이템 추가
        ClearItems();
        Ingre_AddItems();

        // CookButton을 UI 계층에서 뒤로 이동 (즉, 재료 탭이 위로 올라옴)
        UpdateTabColors(IngreButton);
    }

    public void OnClickCookTab()
    {
        // 기존 아이템 삭제 후 요리 아이템 추가
        ClearItems();
        Cook_AddItems();

        // IngreButton을 UI 계층에서 뒤로 이동 (즉, 요리 탭이 위로 올라옴)
        UpdateTabColors(CookButton);
    }

    private void UpdateTabColors(GameObject activeTab)
    {
        Color ingreSelectedColor = new Color32(102, 124, 152, 255); // #667C98 (재료 탭 선택)
        Color cookSelectedColor = new Color32(120, 146, 118, 255); // #789276 (요리 탭 선택)
        Color inactiveColor = new Color32(162, 162, 162, 255); // #A2A2A2 (비활성화된 탭)

        // 활성화된 탭에 맞춰 색상 변경
        if (activeTab == IngreButton)
        {
            IngreButton.GetComponent<Image>().color = ingreSelectedColor;
            CookButton.GetComponent<Image>().color = inactiveColor;
        }
        else if (activeTab == CookButton)
        {
            IngreButton.GetComponent<Image>().color = inactiveColor;
            CookButton.GetComponent<Image>().color = cookSelectedColor;
        }
    }

    public void ClearItems()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void Cook_AddItems()
    {
        foreach (var me in MyCookList)
        {
            GameObject item = Instantiate(CookPrefab, content);

            Debug.Log($"{me.index}, {me.menuID}, {me.score}, {me.bonus}, {me.name}");

            // 카드 스프라이트의 인덱스 지정 변수
            Cook_h menu = item.GetComponent<Cook_h>();
            menu.SetMenuID(me.menuID);
            menu.SetIndex(me.index);
            menu.SetScore(me.score);
            menu.SetBonus(me.bonus);
            menu.SetName(me.name);

            Transform menuImage = item.transform.Find("menuImage");
            menuImage.GetComponent<Image>().sprite = CookSprites[me.menuID];

            //item의 색상을 각 등급에 맞는 색으로 지정하는 함수 작성하기
            // menu.SetMenuColor();
            // menu.SetButtonActive();
        }
    }

    public void Ingre_AddItems()
    {
        try {
            foreach (var ingre in MyIngreList)
            {
                if(ingre.index != 23) {
                    GameObject item = Instantiate(IngrePrefab, content);

                    // 카드 스프라이트의 인덱스 지정 변수
                    Ingre_h ingredient = item.GetComponent<Ingre_h>();

                    ingredient.SetType(ingre.type);
                    ingredient.SetIndex(ingre.index);
                    ingredient.SetName(ingre.name);
                    ingredient.SetNum(ingre.num);

                    Transform IngreImage = item.transform.Find("menuImage");
                    IngreImage.GetComponent<Image>().sprite = IngredientSprites[ingre.index];

                    Transform SlotLevel = item.transform.Find("Panel");
                    TextMeshProUGUI Leveltext = SlotLevel.GetComponentInChildren<TextMeshProUGUI>(); // 재료 개수

                    Leveltext.text = $"{ingre.num}";
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"뭔가 이상이상: {ex.Message}");
        }
    }

    public void SlotClick(string name)
    {
        
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        BlackBackground.SetActive(true);
        BonusPanel.SetActive(true);

        BonusPanelText.text = $"'{name}' (으)로\n추가 베이킹하시겠습니까?";

        // ✅ 모든 슬롯의 버튼을 비활성화 (다른 슬롯 클릭 방지)
        SetAllSlotsInteractable(false);
    }

    public void NoButtonClick()
    {
        BlackBackground.SetActive(false);
        BonusPanel.SetActive(false);

        PlayerPrefs.DeleteKey("SelectedMenuIndex");
        PlayerPrefs.DeleteKey("SelectedMenuScore");
        PlayerPrefs.Save(); // 변경 사항 저장

        // ✅ 키가 존재하는지 확인
        if (PlayerPrefs.HasKey("SelectedMenuIndex"))
        {
            int savedIndex = PlayerPrefs.GetInt("SelectedMenuIndex");
            Debug.Log($"✔ 'SelectedMenuIndex' 키가 존재합니다. 저장된 값: {savedIndex}");
        }
        else
        {
            Debug.LogWarning("⚠ 'SelectedMenuIndex' 키가 존재하지 않습니다.");
        }

        // ✅ 모든 슬롯의 버튼을 다시 활성화
        SetAllSlotsInteractable(true);
    }

    // ✅ 모든 슬롯의 버튼을 활성화/비활성화하는 함수
    private void SetAllSlotsInteractable(bool interactable)
    {
        foreach (Transform ch in content)
        {
            Bread_h child = ch.GetComponent<Bread_h>();
            
            // 보너스 게임을 이미 진행한 요리의 경우, 변화를 주지 않도록 설정.
            bool Select_bonus = child.ReturnBonus();

            if(!Select_bonus) {
                Button slotButton = child.GetComponent<Button>();

                // Transform SelectImage = child.transform.Find("SelectImage");
                // GameObject SelectImage_ = SelectImage.gameObject;

                if (slotButton != null)
                {
                    slotButton.interactable = interactable;
                }
            }
        }
    }

    public void YesButtonClick()
    {   
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        SceneManager.LoadScene("Match");
    }

    private void LoadIngredientsFromCSV()
    {
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(ingredient_csvFileName));
            if (csvFile == null)
            {
                Debug.LogError($"CSV 파일을 찾을 수 없습니다: {ingredient_csvFileName}");
                return;
            }

            Debug.Log($"일단 여기까지 돌아감");

            string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // 첫 번째 줄 (헤더) 무시하고 읽기
            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(',');
                if (fields.Length < 4) continue;

                int index = int.Parse(fields[0].Trim());
                string name = fields[1].Trim();
                int type = int.Parse(fields[2].Trim());
                int num = 0;

                MyIngreList.Add(new MyIngre(index, name, type, num));
            }

            Debug.Log($"총 {MyIngreList.Count}개의 재료를 불러왔습니다.");

            // ingre_Num 리스트를 재료 리스트 크기만큼 0으로 초기화
            MyIngreList.Add(new MyIngre(23,"null",2,0));
            LoadRecipeData();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {ex.Message}");
        }
    }

    private void LoadRecipeData() {
        GD = DataManager.Instance.LoadGameData();
        ingredientNum = DataManager.Instance.gameData.ingredientNum;

        int i = 0;
        foreach (int num in ingredientNum) {
            MyIngreList[i].num = num;
            i++;
        }

        
    }

    private void LoadCookData() {
        GD = DataManager.Instance.LoadGameData();

        foreach (MyRecipeList bake in DataManager.Instance.gameData.myBake) {
            MyCookList.Add(bake);
        }
    }
}

