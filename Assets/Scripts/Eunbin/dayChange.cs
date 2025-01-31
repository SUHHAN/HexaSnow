using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class DayChange : MonoBehaviour
{
    public GameTime gametime;
    public Order order;
    public TextMeshProUGUI timerText;
    public Button dayChangButton; // DayChange 버튼
    public Order orderScript; // Order 스크립트 참조
    public special_customer SpecialScript;
    public getMenu getMenuScript;
    private int day = 1; // Day 값
    private bool isSpecialCustomerSubscribed = false;

    [SerializeField] private GameData GD = new GameData();

    void Start()
    {
        dayChangButton.onClick.AddListener(() =>
        {
        OnDayChange();
        });
    }

    public void OnDayChange()
    {
        day++;
        order.openMenu(day);
        SpecialScript.currentDay=day;

        gametime.StopTimer();
        timerText.text = "영업 준비 중";

        if (day == 4 || day == 7)
        {
            orderScript.IncreaseAcceptOrder(2); // Order 스크립트에 메서드 호출
        }

        if (day==2 || day==5 || day==8){
            getMenuScript.currentDay = day; // 현재 날짜 업데이트
            SpecialScript.LoadDialoguesFromCSV();

        if (!isSpecialCustomerSubscribed)
        {
            gametime.OnSpecialTimeReached -= SpecialScript.orderSpecialCustomer; // 기존 구독 제거
            gametime.OnSpecialTimeReached += SpecialScript.orderSpecialCustomer; // 새로운 구독 추가
            isSpecialCustomerSubscribed = true; // 구독 상태 업데이트
        }
        }
        else
    {
        // 조건에 해당하지 않는 날에는 구독 해제
        gametime.OnSpecialTimeReached -= SpecialScript.orderSpecialCustomer;
        isSpecialCustomerSubscribed = false;
    }
        orderScript.ResetOrderSystem(day); // Order 시스템 초기화
        getMenuScript.currentDay = day; // 현재 날짜 업데이트
        
    }

    private void LoadDate() {

        GD = DataManager.Instance.LoadGameData();

        // !! 일차 업데이트하기
        day = GD.date;
    }

    private void SaveDate() {
        DataManager.Instance.gameData.date = day;

        DataManager.Instance.SaveGameData();
    }
}

