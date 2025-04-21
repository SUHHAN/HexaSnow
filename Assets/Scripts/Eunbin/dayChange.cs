using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class DayChange : MonoBehaviour
{
    public GameTime gametime;
    public TextMeshProUGUI timerText;
    public Button dayChangButton; // DayChange 버튼
    public special_customer SpecialScript;
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
        LoadDate();
        //day++;
        SaveDate();
        SpecialScript.currentDay=day;
        Debug.Log("날짜 변경");

        gametime.StopTimer();
        timerText.text = "준비 중";


        if (day==2 || day==5 || day==8){
            SpecialScript.LoadDialoguesFromCSV();

        if (!isSpecialCustomerSubscribed)
        {
            Debug.Log("특별손님 구독");
            //gametime.OnSpecialTimeReached -= SpecialScript.orderSpecialCustomer; // 기존 구독 제거
            //gametime.OnSpecialTimeReached += SpecialScript.orderSpecialCustomer; // 새로운 구독 추가
            isSpecialCustomerSubscribed = true; // 구독 상태 업데이트
        }
        }
        else
    {
        // 조건에 해당하지 않는 날에는 구독 해제
        //gametime.OnSpecialTimeReached -= SpecialScript.orderSpecialCustomer;
        isSpecialCustomerSubscribed = false;
    }
        
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

