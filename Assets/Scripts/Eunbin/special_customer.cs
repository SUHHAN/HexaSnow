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
    public GameObject speechBubble;
    public GameTime gametime;
    public GameObject order; // 주문 UI
    public TextMeshProUGUI dialogueOrder; // 주문 텍스트 표시용
    private Dictionary<int, GameObject> specialOrders = new Dictionary<int, GameObject>();
    public TextMeshProUGUI orderCustomer;
    public GameObject oldMan;
    public GameObject man;
    public GameObject child;
    public Button acceptButton;
    public Button none;
    private List<GameObject> customers = new List<GameObject>();
    public int Spe_customer=0;
    public int currentDay;
    private bool isOrderCompleted=false;

    public GameObject MadeMenu;
    public SetMenu setmenu;

    void Start()
    {
        oldMan.SetActive(false);
        child.SetActive(false);
        man.SetActive(false);
        customers.Add(child);
        customers.Add(oldMan);
        customers.Add(man);

        order.SetActive(false);
        gametime.OnSpecialTimeReached += OnSpecialTimeReached;
        }    
    private void OnSpecialTimeReached()
    {   
        Debug.Log("3시에 손님 등장 이벤트 발생");
        if(specialOrders.ContainsKey(currentDay)){
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
        speechBubble.SetActive(true);
        MadeMenu.SetActive(true);
        setmenu.current_cus("오리지널 마들렌", "special"); 
        StartCoroutine(HandleCustomerInteraction(customer, day));
        none.onClick.RemoveAllListeners();
        
        none.onClick.AddListener(() => {
            Debug.Log($"손님 {customer.name}이(가) 메뉴를 받지 못했습니다.");
            UpdateDialogue("none");
        });
    }
    private IEnumerator HandleCustomerInteraction(GameObject customer, int day){
        yield return new WaitUntil(() => isOrderCompleted);

        Debug.Log("사용자 입력 감지됨");

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        speechBubble.SetActive(false);
        customer.SetActive(false);
        order.SetActive(false);
        MadeMenu.SetActive(false);
        specialOrders.Remove(day);

        Debug.Log("특별 손님이 방문을 완료했습니다.");
}
    public void UpdateDialogue(string action){
        Debug.Log($"UpdateDialogue 호출됨: {action}");
        if(action.Equals("none")){
            dialogueText.text="안만들었다고요?";
        }
        else if(action.Equals("True")){
            dialogueText.text="감사합니다.";
            Debug.Log("특별 손님이 제품을 받아갔습니다!");
        }
        else if(action.Equals("False")){
            dialogueText.text="이거 아니잖아요!";   
            Debug.Log("특별 손님이 제품을 잘못 받아갔습니다!");     
        }
        isOrderCompleted=true;
    }
}
