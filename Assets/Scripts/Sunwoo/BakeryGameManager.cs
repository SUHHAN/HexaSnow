using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeryGameManager : MonoBehaviour
{
    // ���� ������Ʈ ����
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

    // �÷��̾ ��Ḧ �����ߴ��� ����
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
        // ���콺 ���� ��ư�� Ŭ������ ��
        if (Input.GetMouseButtonDown(0))
        {
            // Ŭ���� ��ġ�� ȭ�� ��ǥ���� ���� ��ǥ�� ��ȯ
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject clickedObject = null;

            // Ŭ���� ��ġ���� Raycast�� �߻��� ������Ʈ�� Ž��
            RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);
            if (hit.collider != null)
            {
                clickedObject = hit.collider.gameObject;
            }

            // ����� Ŭ���ϸ� ����� �� ��� ui Ȱ��ȭ
            if (clickedObject != null && clickedObject.name == "Refrigerator")
            {
                RefriUI.SetActive(true);
            }

            // ���ڷ����� Ŭ���ϸ� ���� �ð� �� Ȱ��ȭ
            if (clickedObject != null && clickedObject.name == "Microwave")
            {
                StartCoroutine(ActivateObjectAfterDelay(2f, MeltButter));
            }

            // �����Ǻ� Ŭ���ϸ� Ȱ��ȭ
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
