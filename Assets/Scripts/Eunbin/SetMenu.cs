using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class SetMenu : MonoBehaviour
{
    private static SetMenu _instance;

    public static SetMenu Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SetMenu>();
            }
            return _instance;
        }
    }
    public getMenu getmenu; // 메뉴 검증용 스크립트
    public special_customer SpecialScript;
    public string currentcus;
    public string currentmenu;

    [SerializeField] private GameData ingredientGD = new GameData();
    // public List<Recipe> MenuList = new List<Recipe>(); // 재료 리스트
    public List<MyRecipeList> MyList = new List<MyRecipeList>(); // 재료 리스트
    // public List<int> Menu_Num = new List<int>();
    [SerializeField] private Sprite[] MenuSprites; //제품 이미지
    public GameObject actionButtonPrefab;
    public GameObject itemPrefab; // 아이템 Prefab
    public Transform content; // Content 오브젝트
    private Bk_h currentSlot;

    [SerializeField] private GameData GD = new GameData();


    void Start()
    {
        LoadRecipeDate();
        AddItems();
    }

    public void current_cus(string menu, string cus){
        currentmenu=menu;
        currentcus=cus;
        Debug.Log($"current_cus 호출됨: 메뉴 - {menu}, 손님 유형 - {cus}");
}

public void AddItems()
    {
        Debug.Log("컴포넌트 추가 중");
        foreach (var me in MyList)
        {
            GameObject item = Instantiate(itemPrefab, content);

            // 카드 스프라이트의 인덱스 지정 변수
            Bk_h menu = item.GetComponent<Bk_h>();
            menu.SetMenuID(me.menuID);
            menu.SetIndex(me.index);
            menu.SetScore(me.score);
            menu.SetName(me.name);

            Transform menuImage = item.transform.Find("menuImage");
            menuImage.GetComponent<Image>().sprite = MenuSprites[me.menuID];

            //item의 색상을 각 등급에 맞는 색으로 지정하는 함수 작성하기
            menu.SetMenuColor();
        // 슬롯 클릭 이벤트에 이름과 인덱스 전달
        Button slotButton = item.GetComponent<Button>();
        if (slotButton != null)
        {
            string capturedName = me.name;
            int capturedIndex = me.index; // 클로저 문제 방지
            slotButton.onClick.AddListener(() => SlotClick(me.name, capturedIndex));
        }
    }
}

    public void SlotClick(string name, int index)
{
    Debug.Log($"슬롯 {name}, 인덱스 {index} 클릭됨");

    // 이전에 선택된 슬롯 버튼 제거
    if (currentSlot != null)
    {
        Transform previousButton = currentSlot.transform.Find("ConfirmButton");
        if (previousButton != null)
        {
            Destroy(previousButton.gameObject);
        }
    }

    // 현재 슬롯 탐색
    bool slotFound = false; // 슬롯 발견 여부 확인
    foreach (Transform slot in content)
    {
        Bk_h child = slot.GetComponent<Bk_h>();
        if (child != null)
        {
            Debug.Log($"슬롯 확인 중: 이름 = {child.GetMenuName()}, 인덱스 = {child.GetIndex()}");
            if (child.GetMenuName().Trim().Equals(name.Trim()) && child.GetIndex() == index) // 이름과 인덱스 둘 다 일치
            {
                Debug.Log($"일치하는 슬롯 발견: {child.GetMenuName()}, 인덱스 = {child.GetIndex()}");
                currentSlot = child;
                slotFound = true;

                // "확인" 버튼 생성
                GameObject confirmButton = Instantiate(actionButtonPrefab, currentSlot.transform);
                confirmButton.name = "ConfirmButton";
                confirmButton.transform.localPosition = Vector3.zero;
                confirmButton.transform.localScale = Vector3.one;

                // 버튼 이벤트 연결
                Button actionButton = confirmButton.GetComponent<Button>();
                if (actionButton != null)
                {
                    actionButton.onClick.RemoveAllListeners();
                    actionButton.onClick.AddListener(() =>
                    {
                        OnActionButtonClick(currentSlot.GetMenuName());
                    });
                }
                break;
            }
        }
    }

    if (!slotFound)
    {
        Debug.LogError($"슬롯 '{name}', 인덱스 '{index}'를 찾을 수 없습니다.");
    }
}

    private void OnActionButtonClick(string name)
    {
        Debug.Log($"'{name}' 메뉴가 확인되었습니다!");
        
    }

    public void NoButtonClick()
    {

        PlayerPrefs.DeleteKey("SelectedMenuIndex");
        PlayerPrefs.DeleteKey("SelectedMenuScore");
        PlayerPrefs.Save(); // 변경 사항 저장

        Debug.LogWarning("✔ PlayerPrefs 초기화 완료");

        // 슬롯 버튼 활성화
        SetAllSlotsInteractable(true);
    }

    // ✅ 모든 슬롯의 버튼을 활성화/비활성화하는 함수
    private void SetAllSlotsInteractable(bool interactable)
    {
        foreach (Transform ch in content)
        {
            Bk_h child = ch.GetComponent<Bk_h>();
            if (child != null)
            {
                // 슬롯 활성화/비활성화 설정
                Button slotButton = child.GetComponent<Button>();
                if (slotButton != null)
                {
                    slotButton.interactable = interactable;
                }
            }
        }
    }
    private void LoadRecipeDate() {
        GD = DataManager.Instance.LoadGameData();

        foreach (MyRecipeList recipe in DataManager.Instance.gameData.myBake) {
            MyList.Add(recipe);
            Debug.Log("리스트 추가");
        }

    }
void Awake()
{
    if (_instance != null && _instance != this)
    {
        Destroy(this.gameObject); // 중복된 인스턴스 제거
        return;
    }
    _instance = this;
}

    }
 