
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;

public class BreadScrollbarManager : MonoBehaviour
{
    private static BreadScrollbarManager _instance;
    public static BreadScrollbarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BreadScrollbarManager>();
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class Recipe
    {
        public int index;
        public string name;

        public Recipe(int index, string name)
        {
            this.index = index;
            this.name = name;
        }
    }

    // [System.Serializable]
    // public class MyRecipeList
    // {
    //     public int index;
    //     public int menuID;
    //     public string name;
    //     public int score;
    //     public bool bonus;

    //     public MyRecipeList(int index, int menuID, string name, int score, bool bonus)
    //     {
    //         this.index = index;
    //         this.menuID = menuID;
    //         this.name = name;
    //         this.score = score;
    //         this.bonus = bonus;
    //     }
    // }


    [SerializeField] private GameData ingredientGD = new GameData();

    private string csvFileName = "recipe.csv"; // CSV 파일명
    // public List<Recipe> MenuList = new List<Recipe>(); // 재료 리스트
    public List<MyRecipeList> MyList = new List<MyRecipeList>(); // 재료 리스트
    // public List<int> Menu_Num = new List<int>();
    [SerializeField] private Sprite[] MenuSprites;

    public GameObject itemPrefab; // 아이템 Prefab
    public Transform content; // Content 오브젝트
    [SerializeField] private GameObject BonusPanel;
    [SerializeField] private GameObject BlackBackground;
    [SerializeField] private TextMeshProUGUI BonusPanelText;

    [SerializeField] private GameData GD = new GameData();

    private int FinalSc = 0; // 초기 점수
    private int SumSc;

    

    void Start()
    {
        // 데이터 업로드
        LoadRecipeDate();

        // 오디오 관리
        AudioManager.Instance.PlayBgm(AudioManager.Bgm.inside_kitchen_baking);

        BonusPanel.SetActive(false);
        BlackBackground.SetActive(false);

        AddItems(); // 10개의 아이템 추가

        // PrintIngredients(); // 확인용
    }

    public void AddItems()
    {
        foreach (var me in MyList)
        {
            
            GameObject item = Instantiate(itemPrefab, content);

            // 카드 스프라이트의 인덱스 지정 변수
            Bread_h menu = item.GetComponent<Bread_h>();
            menu.SetMenuID(me.menuID);
            menu.SetIndex(me.index);
            menu.SetScore(me.score);
            menu.SetBonus(me.bonus);
            menu.SetName(me.name);

            Transform menuImage = item.transform.Find("menuImage");
            menuImage.GetComponent<UnityEngine.UI.Image>().sprite = MenuSprites[me.menuID];

            //item의 색상을 각 등급에 맞는 색으로 지정하는 함수 작성하기
            menu.SetMenuColor();
            menu.SetButtonActive();
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

    private void LoadRecipeDate() {
        GD = DataManager.Instance.LoadGameData();

        foreach (MyRecipeList recipe in DataManager.Instance.gameData.myBake) {
            MyList.Add(recipe);
        }
    }
}

