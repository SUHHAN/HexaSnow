using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Cook_h : MonoBehaviour
{
    [SerializeField] private int Menu_ID; // 현재 요리의 메뉴의 번호 -> 메뉴 고유 번호
    [SerializeField] private int Menu_Index; // 현재 요리의 번호 순서를 의미 -> 만들어진 순서대로
    [SerializeField] private int Menu_Score; // 현재 요리의 점수
    [SerializeField] private char Menu_Level; // 현재 요리의 등급
    [SerializeField] private bool Menu_Bonus; // 보너스 게임을 했는지 안했는지 여부
    [SerializeField] private string Menu_Name;

    private Button SlotPanelButton; // 버튼 컴포넌트 추가

    // 등급별 배경 색상 지정 (글자 색상과 조화되도록 조정)
    private Color S_BackgroundColor = new Color32(180, 200, 255, 255);  // 연한 파랑 (파스텔 블루)
    private Color A_BackgroundColor = new Color32(190, 230, 190, 255);  // 연한 초록 (파스텔 그린)
    private Color B_BackgroundColor = new Color32(220, 220, 220, 255);  // 밝은 회색 (균형 유지)
    private Color C_BackgroundColor = new Color32(210, 210, 210, 255);  // 연한 회색 (중간 톤)
    private Color D_BackgroundColor = new Color32(200, 200, 200, 255);  // 약간 어두운 회색
    private Color F_BackgroundColor = new Color32(240, 190, 200, 255);  // 연한 버건디 (파스텔 핑크빛)



    // 등급별 글자 색상 지정 (F, S, A는 조금 더 밝게, 나머지는 검정 고정)
    private Color S_TextColor = new Color32(50, 100, 200, 255);  // 밝은 딥 블루
    private Color A_TextColor = new Color32(70, 140, 70, 255);   // 밝은 딥 그린
    private Color B_TextColor = new Color32(34, 34, 34, 255);    // 검정 (고정)
    private Color C_TextColor = new Color32(34, 34, 34, 255);    // 검정 (고정)
    private Color D_TextColor = new Color32(34, 34, 34, 255);    // 검정 (고정)
    private Color F_TextColor = new Color32(140, 50, 70, 255);   // 밝은 버건디

    // 검정색 (기본 고정)
    private Color DefaultBlack = new Color32(34, 34, 34, 255);   // #222222 (짙은 회색)
    private Color Custom_BackgroundColor = new Color32(255, 247, 231, 255);  // 크리미한 아이보리 톤 (밝은 느낌)





    void Start()
    {

        // 현재 게임 오브젝트에서 Button 컴포넌트를 가져와서 클릭 이벤트 추가
        SlotPanelButton = this.GetComponent<Button>();
        if (SlotPanelButton != null)
        {
            SlotPanelButton.onClick.AddListener(OnButtonClick);
        }

    }

    // 버튼 클릭 시 실행될 함수
    private void OnButtonClick()
    {
        InventoryManager_h.Instance.SlotClick(Menu_Name);

        // Menu_Index 값을 PlayerPrefs에 저장
        PlayerPrefs.SetInt("SelectedMenuIndex", Menu_Index);
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
    }

    public void SetScore(int Score) {
        this.Menu_Score = Score;
    }

    public void SetMenuID(int id)
    {
        this.Menu_ID = id;
    }

    public void SetName(string name)
    {
        this.Menu_Name = name;
    }

    public void SetIndex(int index)
    {
        this.Menu_Index = index;
    }

    public void SetBonus(bool bonus) {
        this.Menu_Bonus = bonus;
    }

    public bool ReturnBonus() {
        return this.Menu_Bonus;
    }

    // 나의 요리의 등급에 따라서 색상을 표시해줌
    public void SetMenuColor() {

        SetLevel_Char();

        Image SlotPanel = this.GetComponent<Image>();
        Transform SlotReal = this.transform.Find("RealImage");
        Image SlotPanelImage = SlotReal.GetComponent<Image>();
        Transform SlotLevelPanel = this.transform.Find("Panel");
        TextMeshProUGUI Leveltext = SlotLevelPanel.GetComponentInChildren<TextMeshProUGUI>();


        if (Menu_Level == 'S') {
            SlotPanel.color = S_BackgroundColor;
            Leveltext.color = S_TextColor;
            Leveltext.text = "S";

        }else if(Menu_Level == 'A') {
            SlotPanel.color = A_BackgroundColor;
            Leveltext.color = A_TextColor;
            Leveltext.text = "A";


        }else if(Menu_Level == 'B') {
            SlotPanel.color = Custom_BackgroundColor; 
            Leveltext.color = B_TextColor;
            Leveltext.text = "B";


        }else if(Menu_Level == 'C') {
            SlotPanel.color = Custom_BackgroundColor;    
            Leveltext.color = C_TextColor;
            Leveltext.text = "C";


        }else if(Menu_Level == 'D') {
            SlotPanel.color = Custom_BackgroundColor;   
            Leveltext.color = D_TextColor;
            Leveltext.text = "D";


        }else if(Menu_Level == 'F') {
            SlotPanel.color = F_BackgroundColor;
            Leveltext.color = F_TextColor;
            Leveltext.text = "F";


        }
    }

    void SetLevel_Char() {
        if (Menu_Score == 60) {
            Menu_Level = 'S';
        }else if(Menu_Score > 40) {
            Menu_Level = 'A';
        }else if(Menu_Score > 30) {
            Menu_Level = 'B';            
        }else if(Menu_Score > 20) {
            Menu_Level = 'C';            
        }else if(Menu_Score > 10) {
            Menu_Level = 'D';            
        }else if(Menu_Score <= 10) {
            Menu_Level = 'F';            
        }
    }

    // 만약 이미 보너스 게임을 진행한 빵이라면, 버튼 활성화 및 비활성화
    public void SetButtonActive()
    {
        Button SlotPanelButton = this.GetComponent<Button>();
        Transform Image = this.transform.Find("menuImage");
        Image SlotImage = Image.GetComponent<Image>();      
        CanvasGroup canvasGroup = this.GetComponent<CanvasGroup>(); // 테두리 판넬

        Transform SlotReal = this.transform.Find("RealImage");  
        Image SlotPanelImage = SlotReal.GetComponent<Image>();  // 테두리 안 판넬

        Transform SlotLevel = this.transform.Find("Panel");
        Image SlotLevelPanel = SlotLevel.GetComponent<Image>(); // 등급 판넬
        TextMeshProUGUI Leveltext = SlotLevel.GetComponentInChildren<TextMeshProUGUI>(); // 등급 글자

        if (Menu_Bonus == false)
        {
            SlotPanelButton.interactable = true; // 클릭 가능
            if (canvasGroup != null) canvasGroup.blocksRaycasts = true; // 터치 가능
            if (SlotImage != null) SlotImage.color = new Color(SlotImage.color.r, SlotImage.color.g, SlotImage.color.b, 1f);
        }
        else
        {
            SlotPanelButton.interactable = false; // 클릭 불가능
            // SlotImage의 투명도를 50%로 낮춰줘.
            if (canvasGroup != null) canvasGroup.blocksRaycasts = false; // UI 터치 막기
            if (SlotImage != null) SlotImage.color = new Color(SlotImage.color.r, SlotImage.color.g, SlotImage.color.b, 0.7f);
            if (SlotPanelImage != null) SlotPanelImage.color = new Color(SlotPanelImage.color.r, SlotPanelImage.color.g, SlotPanelImage.color.b, 0.7f);
            if (SlotLevelPanel != null) SlotLevelPanel.color = new Color(SlotLevelPanel.color.r, SlotLevelPanel.color.g, SlotLevelPanel.color.b, 0.7f);
        }
    }

}
