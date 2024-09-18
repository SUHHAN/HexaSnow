using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    // 드래그 시작 위치를 저장하는 변수
    private Vector3 defaultPos;
    // 마우스 클릭 시 오브젝트와 마우스 사이의 거리
    private Vector3 offset;
    // 드래그 상태를 나타내는 변수
    private bool isDragging = false;
    // 볼이 있는 영역
    private GameObject makeArea;
    // 쓰레기통 오브젝트
    private GameObject trashcan;

    //private BakingController bakingController;
    //private TrashController trashController;

    public GameObject Madeleince;

    // Start is called before the first frame update
    void Start()
    {
        // 오브젝트의 초기 위치를 저장
        defaultPos = this.transform.position;
        // 볼 영역과 쓰레기통 오브젝트를 찾음
        makeArea = GameObject.Find("MakeArea");
        trashcan = GameObject.Find("TrashCan");
        // 음료 만들기와 쓰레기통 컨트롤러를 찾음
        //BakingController = FindObjectOfType<BakingController>();
        //trashController = FindObjectOfType<TrashController>();
    }

    // 마우스를 클릭했을 때 호출되는 함수
    void OnMouseDown()
    {
        // 클릭 시 오브젝트와 마우스 간의 거리를 계산
        //offset = transform.position - GetMouseWorldPosition();
        // 드래그 중임을 표시
        //isDragging = true;
    }

    // 마우스를 놓았을 때 호출되는 함수
    void OnMouseUp()
    {
        // 드래그 종료
        isDragging = false;
        // 음료 제조 영역과 쓰레기통을 체크
        //CheckForMakeArea();
        //CheckForTrashCan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
