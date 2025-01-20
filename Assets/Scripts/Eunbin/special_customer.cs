using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class special_customer : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public GameTime gametime;
    public GameObject order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 주문 텍스트 표시용
    private Dictionary<int, GameObject> specialOrders = new Dictionary<int, GameObject>();
    public TextMeshProUGUI orderCustomer;
    public GameObject oldMan;
    public GameObject man;
    public GameObject child;
    public Button acceptButton;
    private List<GameObject> customers = new List<GameObject>();
    public int Spe_customer=0;
    public int currentDay;
    private bool isOrderCompleted=false;

    void Start()
    {
        oldMan.SetActive(false);
        child.SetActive(false);
        man.SetActive(false);
        customers.Add(child);
        customers.Add(oldMan);
        customers.Add(man);

        order.SetActive(true);
        gametime.OnSpecialTimeReached += OnSpecialTimeReached;
        }    
    private void OnSpecialTimeReached()
    {   
        Debug.Log("특정 시간(3분)에 손님 등장 이벤트 발생");
        if(specialOrders.ContainsKey(currentDay)){
            Debug.Log("여기1");
            VisitSpecialCustomer(currentDay);
        }
    }
    public void orderSpecialCustomer(){
    if (Spe_customer >= customers.Count)
    {
        Debug.LogError("스페셜 손님 인덱스가 범위를 초과했습니다!");
        return;
    }   
        Debug.Log("특별 손님 등장");
        GameObject customer=customers[Spe_customer];
        customer.SetActive(true);
        order.SetActive(true);

        dialogueOrder.text="특별한 디저트 주세요";
        orderCustomer.text="특별 손님";

        acceptButton.gameObject.SetActive(true);
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => {
            Debug.Log("스페셜 손님 주문 완료!");
            specialOrders[currentDay + 2] = customer;
            customer.SetActive(false);
            order.SetActive(false);
           acceptButton.gameObject.SetActive(false);
           Spe_customer++;
    });
    }

    public void CheckSpecialCustomerVisit()
    {
        if (specialOrders.ContainsKey(currentDay))
        {
            Debug.Log($"{currentDay}오늘은 특별 손님 제품 수령일입니다!");
        }
        else
        {
            Debug.Log("오늘은 특별 손님이 방문하지 않습니다.");
        }
    }
    public void VisitSpecialCustomer(int day)
    {
        if (!specialOrders.ContainsKey(day))
        {
            Debug.LogError("특별 손님 방문 데이터가 없습니다!");
            return;
        }

        GameObject customer = specialOrders[day];
        customer.SetActive(true);
        order.SetActive(true);

        dialogueOrder.text = "특별한 디저트를 받으러 왔습니다!";
        orderCustomer.text = "특별 손님";

        acceptButton.gameObject.SetActive(true);
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() =>
        {
            Debug.Log("특별 손님이 제품을 받아갔습니다!");
            customer.SetActive(false);
            order.SetActive(false);
            acceptButton.gameObject.SetActive(false);

            // 방문 완료 후 데이터 삭제
            specialOrders.Remove(day);
        });
    }
}
