using UnityEngine;
using UnityEngine.UI;

public class DayChange : MonoBehaviour
{
    public Button dayChangButton; // DayChange 버튼
    public Order orderScript; // Order 스크립트 참조

    private int day = 1; // Day 값

    void Start()
    {
        dayChangButton.onClick.AddListener(OnDayChange);
    }

    private void OnDayChange()
    {
        day++;
        if (day == 4 || day == 7)
        {
            orderScript.IncreaseAcceptOrder(2); // Order 스크립트에 메서드 호출
        }
<<<<<<< HEAD

        if (day==2 || day==5 || day==8){
            getMenuScript.currentDay = day; // 현재 날짜 업데이트

        if (!isSpecialCustomerSubscribed)
        {
            gametime.OnSpecialTimeReached -= SpecialScript.orderSpecialCustomer; // 기존 구독 제거
            gametime.OnSpecialTimeReached += SpecialScript.orderSpecialCustomer; // 새로운 구독 추가
            isSpecialCustomerSubscribed = true; // 구독 상태 업데이트
        }
            Debug.Log("3시에 손님 등장 이벤트 발생");
        }
        else
    {
        // 조건에 해당하지 않는 날에는 구독 해제
        gametime.OnSpecialTimeReached -= SpecialScript.orderSpecialCustomer;
        isSpecialCustomerSubscribed = false;
    }
=======
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
        orderScript.ResetOrderSystem(day); // Order 시스템 초기화
        Debug.Log($"Day changed to {day}");
    }
}

