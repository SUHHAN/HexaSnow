using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    // �巡�� ���� ��ġ�� �����ϴ� ����
    private Vector3 defaultPos;
    // ���콺 Ŭ�� �� ������Ʈ�� ���콺 ������ �Ÿ�
    private Vector3 offset;
    // �巡�� ���¸� ��Ÿ���� ����
    private bool isDragging = false;
    // ���� �ִ� ����
    private GameObject makeArea;
    // �������� ������Ʈ
    private GameObject trashcan;

    //private BakingController bakingController;
    //private TrashController trashController;

    public GameObject Madeleince;

    // Start is called before the first frame update
    void Start()
    {
        // ������Ʈ�� �ʱ� ��ġ�� ����
        defaultPos = this.transform.position;
        // �� ������ �������� ������Ʈ�� ã��
        makeArea = GameObject.Find("MakeArea");
        trashcan = GameObject.Find("TrashCan");
        // ���� ������ �������� ��Ʈ�ѷ��� ã��
        //BakingController = FindObjectOfType<BakingController>();
        //trashController = FindObjectOfType<TrashController>();
    }

    // ���콺�� Ŭ������ �� ȣ��Ǵ� �Լ�
    void OnMouseDown()
    {
        // Ŭ�� �� ������Ʈ�� ���콺 ���� �Ÿ��� ���
        //offset = transform.position - GetMouseWorldPosition();
        // �巡�� ������ ǥ��
        //isDragging = true;
    }

    // ���콺�� ������ �� ȣ��Ǵ� �Լ�
    void OnMouseUp()
    {
        // �巡�� ����
        isDragging = false;
        // ���� ���� ������ ���������� üũ
        //CheckForMakeArea();
        //CheckForTrashCan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
