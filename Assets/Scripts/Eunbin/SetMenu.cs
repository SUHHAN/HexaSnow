using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using System.IO;
using System.Collections;

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

    [System.Serializable]
    public class RecipeC
    {
        public int index;                 // 레시피 인덱스
        public string menu;                // 메뉴 이름
        public int coin;

    }

    public getMenuOnly getmenu; // 메뉴 검증용 스크립트
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
    private Bk_h bakerySlotData;

    [SerializeField] private GameData GD = new GameData();
    private List<RecipeC> recipes = new List<RecipeC>();

    private int coin = 0;
    private bool isBakeryDelivering = false; // 베이커리 전달 중인지 여부
        private Dictionary<string, List<string>> specialCustomerMenu = new Dictionary<string, List<string>>();


    void Start()
    {
        InitializeSpecialCustomerMenus();
        LoadRecipeDate();
        LoadRecipesFromCSV("Assets/Resources/recipe.csv");
        AddItems();

    }
    private void InitializeSpecialCustomerMenus()
        {
            specialCustomerMenu.Clear();

            specialCustomerMenu.Add("child", new List<string> {"핑크 딸기 머핀", "리얼 초코 머핀", "핑크 딸기 파운드 케이크", "리얼 초코 파운드 케이크" });
            specialCustomerMenu.Add("old_man", new List<string> { "꿀고구마 파운드 케이크", "달콤 귤 파운트 케이크", "리얼 초코 파운드 케이크", "달콤 귤 타르트"});
            specialCustomerMenu.Add("man", new List<string> { "블루 레몬 케이크" });
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

             // 특별 손님 인덱스 별로 사진 수정하기
            if(me.menuID <= 34) {
                menuImage.GetComponent<Image>().sprite = MenuSprites[me.menuID];
            }
            else if(101 <= me.menuID && me.menuID <= 104) {
                if(me.score <= 10) {
                    menuImage.GetComponent<Image>().sprite = MenuSprites[44 + me.menuID % 100];
                }
                else{
                    menuImage.GetComponent<Image>().sprite = MenuSprites[34 + me.menuID % 100];
                }
            }
            else if(201 <= me.menuID && me.menuID <= 204) {
                if(me.score <= 10) {
                    menuImage.GetComponent<Image>().sprite = MenuSprites[44 + 4 + me.menuID % 200];
                }
                else{
                    menuImage.GetComponent<Image>().sprite = MenuSprites[34 + 4 + me.menuID % 200];
                }
            }
            else if(301 == me.menuID) {
                if(me.score <= 10) {
                    menuImage.GetComponent<Image>().sprite = MenuSprites[53];
                }
                else{
                    menuImage.GetComponent<Image>().sprite = MenuSprites[43];
                }
            }
            else if(401 == me.menuID) {
                menuImage.GetComponent<Image>().sprite = MenuSprites[44];
            }


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

    if (isBakeryDelivering)
    {
        Debug.LogWarning("슬롯 클릭 무시됨: 베이커리 전달 중입니다.");
        return;
    }

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

                bakerySlotData = child;
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
                        OnActionButtonClick(bakerySlotData);
                    });
                }
                break;
            }
        }
    }

   // if (!slotFound)
    //{
     //   Debug.LogError($"슬롯 '{name}', 인덱스 '{index}'를 찾을 수 없습니다.");
   // }
}
private void OnActionButtonClick(Bk_h bakerySlot)
{
    if (isBakeryDelivering)
    {
        Debug.LogWarning("이미 베이커리 전달 중입니다! 터치를 기다리는 중.");
        return;
    }

    isBakeryDelivering = true; // 전달 중 플래그 설정

    StartCoroutine(HandleBakeryDelivery(bakerySlot));
}

    private IEnumerator HandleBakeryDelivery(Bk_h bakerySlot)
{
    if (bakerySlot == null)
    {
        Debug.LogError("저장된 베이커리 슬롯 데이터를 찾을 수 없음!");
        yield break;
    }

    Debug.Log($"메뉴: {bakerySlot.GetMenuName()}, 인덱스: {bakerySlot.GetIndex()}");

    CheckMenu(bakerySlot.GetMenuName(), bakerySlot.GetScore());

    MyList.RemoveAll(item => item.name == bakerySlot.GetMenuName() && item.index == bakerySlot.GetIndex());
    DataManager.Instance.gameData.myBake.RemoveAll(item => item.name == bakerySlot.GetMenuName() && item.index == bakerySlot.GetIndex());
    DataManager.Instance.gameData.money += coin;

    DataManager.Instance.SaveGameData();
    UiLogicManager.Instance.LoadMoneyData();
    Destroy(bakerySlot.gameObject);

    // 💥 사용자 터치를 기다림
    Debug.Log("터치 대기 중... 베이커리를 더 줄 수 없음!");
    yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

    // 🟢 다시 전달 가능 상태로 초기화
    isBakeryDelivering = false;
    Debug.Log("화면 터치 감지됨! 다시 베이커리 전달 가능.");
}

private void CheckMenu(string menu, int score){

    if(currentcus.Equals("cus")){
        Debug.Log($"[디버깅] 입력값: '{menu}' / 기대값: '{currentmenu}'");
        foreach(RecipeC re in recipes) 
            {
                if(menu == re.menu) {
                    coin = re.coin;
                }
            }
        if(menu.Equals(currentmenu)){
        Debug.Log($"선택된 메뉴가 올바릅니다: {menu}");
             if(score >= 60) {
                getmenu.UpdateDialogue(1); // s
                coin += 2000;
            }
            else if(score > 40) {
                getmenu.UpdateDialogue(1); //a
                coin += 1000;
            }
            else if(score > 30){
                getmenu.UpdateDialogue(2); //b
                coin += 500;
            }
            else if(score > 20){
                getmenu.UpdateDialogue(2); //c
                coin += 100;
            }
            else if(score > 10){
                getmenu.UpdateDialogue(3); //d
                coin += 0;
            }
            else if(score <= 10){
                getmenu.UpdateDialogue(3); //f
                coin -= 500;
            }
            else{
                getmenu.UpdateDialogue(3); //f
                coin -= 500;
            }
        }
        else{
        if(currentcus.Equals("cus")){
        coin = 0;

        Debug.Log($"선택된 메뉴가 올바르지 않습니다: {menu}");
        getmenu.UpdateDialogue(4);
        coin -= 500;
        }
    }
    }
    else {
    // 🎯 특별손님인 경우 (cus가 아닌 경우)
    Debug.Log($"[특별손님 처리 시작] 손님 유형: {currentcus}, 선택한 메뉴: {menu}");

    if (specialCustomerMenu.ContainsKey(currentcus)) {
        List<string> allowedMenus = specialCustomerMenu[currentcus];

        Debug.Log($"[특별손님 허용 메뉴 목록] {string.Join(", ", allowedMenus)}");

        if (allowedMenus.Contains(menu)) {
            Debug.Log($"✅ [특별손님 정답] 손님 '{currentcus}'에게 '{menu}'는 허용된 메뉴입니다.");
            if(score >= 60) { // s
                SpecialScript.UpdateDialogue("True");
                coin += 2000;
            }
            else if(score > 40) { //a
                SpecialScript.UpdateDialogue("True");
                coin += 1000;
            }
            else if(score > 30){ //b
                SpecialScript.UpdateDialogue("True");
                coin += 500;
            }
            else if(score > 20){ //c
                SpecialScript.UpdateDialogue("True");
                coin += 100;
            }
            else if(score > 10){//d
                SpecialScript.UpdateDialogue("False");
                coin += 0;
            }
            else if(score <= 10){//f
                SpecialScript.UpdateDialogue("False");
                coin -= 500;
            }
            else{ //f
                SpecialScript.UpdateDialogue("False");
                coin -= 500;
            }

        } else {
            Debug.Log($"❌ [특별손님 오답] 손님 '{currentcus}'에게 '{menu}'는 허용된 메뉴가 아닙니다.");
            SpecialScript.UpdateDialogue("False");
            

        }
    } else {
        Debug.LogWarning($"⚠️ [특별손님 처리 실패] 손님 유형 '{currentcus}'에 대한 메뉴 데이터가 specialCustomerMenu에 없습니다.");
        coin -= 500;
        SpecialScript.UpdateDialogue("False"); // 기본값 처리
    }

    Debug.Log($"[특별손님 처리 종료] 손님: {currentcus}, 메뉴: {menu}");
}


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

    // CSV 파일에서 레시피 데이터 로드
    void LoadRecipesFromCSV(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            // 커스텀 파서로 CSV 행을 파싱
            var columns = ParseCSVLine(line);


            // 데이터 개수 확인
            if (columns.Length < 6)
            {
                Debug.LogWarning($"잘못된 CSV 데이터: {line}");
                continue;
            }

            int index, coin;

            // 안전한 정수 변환 (TryParse 사용)
            if (!int.TryParse(columns[0], out index)) index = 0;
            if (!int.TryParse(columns[5], out coin)) coin = 0;

            string menu = columns[1];

            // RecipeB 객체 생성
            RecipeC recipe = new RecipeC()
            {
                index = index,
                menu = menu,
                coin = coin
            };

            // RecipeB 객체 출력 (콘솔에)
            Debug.Log(recipe.ToString());

            recipes.Add(recipe);
            
        }

        foreach (RecipeC re in recipes) {
            Debug.LogWarning($"[레시피 가격 저장] {re.index},{re.menu},{re.coin}");
        }

    }

    // 커스텀 CSV 라인 파서
    string[] ParseCSVLine(string line)
    {
        var columns = new List<string>();
        bool insideQuotes = false;
        string currentColumn = "";

        foreach (char c in line)
        {
            if (c == '"' && !insideQuotes)
            {
                insideQuotes = true;  // 따옴표 안으로 들어감
            }
            else if (c == '"' && insideQuotes)
            {
                insideQuotes = false;  // 따옴표 밖으로 나감
            }
            else if (c == ',' && !insideQuotes)
            {
                columns.Add(currentColumn.Trim());
                currentColumn = "";  // 새로운 컬럼 시작
            }
            else
            {
                currentColumn += c;  // 현재 컬럼에 문자 추가
            }
        }

        // 마지막 컬럼 추가
        if (!string.IsNullOrEmpty(currentColumn))
        {
            columns.Add(currentColumn.Trim());
        }

        return columns.ToArray();
    }

}
