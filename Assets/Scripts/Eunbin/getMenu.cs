using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class getMenu : MonoBehaviour
{
    public Order orderScript; // Order 스크립트 참조
    public GameObject oldMan;
    public GameObject man;
    public GameObject child;
    public Button right;
     private Dictionary<int, List<int>> dailyOrders = new Dictionary<int, List<int>>(); // 일별 주문 저장
    private List<GameObject> customers = new List<GameObject>();
    public int currentDay = 1; // 현재 날짜를 추적



    void Start(){
        oldMan.SetActive(false);
        child.SetActive(false);
        man.SetActive(false);

        customers.Add(oldMan);
        customers.Add(man);
        customers.Add(child);
    }

    public void ReceiveOrders(int order_id)
{
    if (!dailyOrders.ContainsKey(currentDay))
        {
            dailyOrders[currentDay] = new List<int>();
        }
    Debug.Log($"현재 {currentDay}일의 주문 상태: {dailyOrders[currentDay].Count}개의 주문이 있습니다.");
    dailyOrders[currentDay].Add(order_id);
    Debug.Log($"{currentDay}일 주문에 메뉴 ID {order_id}가 추가되었습니다.");
    }

    public IEnumerator ProcessCustomers(int dayToProcess)
    {
        Debug.Log($"{dayToProcess}일의 손님들이 메뉴를 받으러 왔습니다.");
        if (!dailyOrders.ContainsKey(dayToProcess) || dailyOrders[dayToProcess].Count == 0)
        {
            Debug.Log($"처리할 주문이 없습니다: {dayToProcess}일");
            yield break;
        }

        List<int> menuForDay = new List<int>(dailyOrders[dayToProcess]); // 해당 날짜의 주문 복사

        foreach(int order in menuForDay)
        {
            GameObject customer=GetRandomCustomer();
            customer.SetActive(true);
            Debug.Log($"손님 {customer.name}이(가) 메뉴 {order}을(를) 받으러 왔습니다!");
            
            bool isOrderCompleted = false;
            right.onClick.RemoveAllListeners(); 
            right.onClick.AddListener(() => {
                Debug.Log($"{customer.name}이(가) 메뉴를 받았습니다.");
                customer.SetActive(false);
                isOrderCompleted = true; 
            });
            yield return new WaitUntil(() => isOrderCompleted);
            Debug.Log("손님이 메뉴를 받아갔습니다!");
        }
        dailyOrders[dayToProcess].Clear(); // 해당 날짜의 주문 처리 완료
        Debug.Log($"{dayToProcess}일의 모든 주문이 처리되었습니다.");
        
    }
    private GameObject GetRandomCustomer(){
        int randomIndex=Random.Range(0, customers.Count);
        return customers[randomIndex];
    }

}