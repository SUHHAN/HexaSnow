using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeryGameManager : MonoBehaviour
{
    // 게임 오브젝트 선언
    public GameObject Bowl;
    public GameObject BakeryMap;
    public GameObject Shelf;
    public GameObject Dessert;
    public GameObject RecipeBook;

    public GameObject Butter;
    public GameObject Egg;
    public GameObject Flour;
    public GameObject Sugar;
    public GameObject Milk;

    public GameObject MeltButter;
    public GameObject RefriUI;

    // 플레이어가 재료를 구매했는지 여부
    private bool buyButter = false;
    private bool buyEgg = false;
    private bool buyFlour = false;
    private bool buySugar = false;
    private bool buyMilk = false;

    // Start is called before the first frame update
    void Start()
    {
        if (buyButter)
        {
            Milk.SetActive(true);
        }
        else if(buyEgg)
        {
            Milk.SetActive(true);
        }
        else if (buyFlour)
        {
            Milk.SetActive(true);
        }
        else if (buySugar)
        {
            Milk.SetActive(true);
        }
        else if (buyMilk)
        {
            Milk.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 클릭한 위치를 화면 좌표에서 월드 좌표로 변환
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject clickedObject = null;

            // 클릭한 위치에서 Raycast를 발사해 오브젝트를 탐지
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);
            if (hit.collider != null)
            {
                clickedObject = hit.collider.gameObject;
            }

            // 냉장고 클릭하면 냉장고 속 재료 ui 활성화
            if (clickedObject != null && clickedObject.name == "Refrigerator")
            {
                RefriUI.SetActive(true);
            }

            // 전자레인지 클릭하면 일정 시간 후 활성화
            if (clickedObject != null && clickedObject.name == "Microwave")
            {
                StartCoroutine(ActivateObjectAfterDelay(2f, MeltButter));
            }

            // 레시피북 클릭하면 활성화
            if (clickedObject != null && clickedObject.name == "RecipeBook")
            {
                RecipeBook.SetActive(true);
                BakeryMap.SetActive(false);
                Shelf.SetActive(false);
            }
        }
    }

    IEnumerator ActivateObjectAfterDelay(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }
}
