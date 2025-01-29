using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Bread_h : MonoBehaviour
{
    public int Menu_ID; // 현재 요리의 메뉴의 번호 -> 메뉴 고유 번호
    public int Menu_Index; // 현재 요리의 번호 순서를 의미 -> 만들어진 순서대로
    private int Menu_Score; // 현재 요리의 점수
    private char Menu_Level; // 현재 요리의 등급
    private bool Menu_Bonus; // 보너스 게임을 했는지 안했는지 여부

    private Button SlotPanelButton; // 버튼 컴포넌트 추가

    // 등급별 색상 (연한 색상 위주)
    private Color S_Color = new Color32(255, 223, 133, 255);  // 연한 골드 (부드러운 황금빛)
    private Color A_Color = new Color32(192, 255, 170, 255);  // 연한 연두색 (부드러운 신선함)
    private Color B_Color = new Color32(173, 216, 230, 255);  // 연한 파랑 (편안한 하늘색)
    private Color C_Color = new Color32(216, 191, 216, 255);  // 연한 보라 (부드러운 라벤더)
    private Color D_Color = new Color32(255, 200, 150, 255);  // 연한 주황 (부드러운 오렌지)
    private Color F_Color = new Color32(255, 160, 160, 255);  // 연한 빨강 (부드러운 코랄)



    void Start()
    {
        InitNum();
        

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
        BreadScrollbarManager.Instance.SlotClick();

        // Menu_Index 값을 PlayerPrefs에 저장
        PlayerPrefs.SetInt("SelectedMenuIndex", Menu_Index);
        PlayerPrefs.Save(); // 변경 사항 저장

        BreadScrollbarManager.Instance.SlotClick();

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

    public void SetIndex(int index)
    {
        this.Menu_Index = index;
    }


    public void SetBonus(bool bonus) {
        this.Menu_Bonus = bonus;
    }

    // 나의 요리의 등급에 따라서 색상을 표시해줌
    public void SetMenuColor() {

        SetLevel_Char();

        Image SlotPanel = this.GetComponent<Image>();

        if (Menu_Level == 'S') {
            SlotPanel.color = S_Color;
        }else if(Menu_Level == 'A') {
            SlotPanel.color = A_Color;
        }else if(Menu_Level == 'B') {
            SlotPanel.color = B_Color;            
        }else if(Menu_Level == 'C') {
            SlotPanel.color = C_Color;           
        }else if(Menu_Level == 'D') {
            SlotPanel.color = D_Color;           
        }else if(Menu_Level == 'F') {
            SlotPanel.color = F_Color;
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
        CanvasGroup canvasGroup = this.GetComponent<CanvasGroup>();

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
            if (SlotImage != null) SlotImage.color = new Color(SlotImage.color.r, SlotImage.color.g, SlotImage.color.b, 0.5f);
        }
    }

    public void InitNum()
    {
        Transform panel = this.transform.Find("numPanel");
        Transform panel2 = this.transform.Find("pricePanel");
    }

    // 이미 보너스 미니게임 진행한 슬롯은 비활성화 하기 함수

}
