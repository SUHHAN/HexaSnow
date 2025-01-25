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
        orderScript.ResetOrderSystem(day); // Order 시스템 초기화
        Debug.Log($"Day changed to {day}");
    }
}
