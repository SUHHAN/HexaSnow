using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; // EventTrigger 사용을 위해 필요

public class SetMenu : MonoBehaviour
{
    public getMenu getmenu; // 메뉴 검증용 스크립트
    public List<GameObject> menuObjects = new List<GameObject>(); // 부모 아래 이미지들
    public List<string> made_menu = new List<string> { "오리지널 마들렌", "레몬 마들렌", "오리지널 쿠키", "아몬드 쿠키" };

    private GameObject currentActiveButton; // 현재 활성화된 버튼

    void Start()
    {
        if (menuObjects.Count == 0)
        {
            Debug.LogWarning("menuObjects 리스트가 비어 있습니다. 관리할 자식 오브젝트를 추가하세요.");
            return;
        }

        InitializeMenus(); // 초기화
    }

    // 초기 메뉴 설정
    private void InitializeMenus()
    {
        for (int i = 0; i < menuObjects.Count; i++)
        {
            GameObject menuObject = menuObjects[i];

            if (menuObject == null)
            {
                Debug.LogWarning($"menuObjects[{i}]가 null입니다. 올바른 GameObject를 설정하세요.");
                continue;
            }

            // Image 내의 TextMeshProUGUI와 Button을 찾습니다.
            TextMeshProUGUI menuText = menuObject.GetComponentInChildren<TextMeshProUGUI>();
            Button menuButton = menuObject.GetComponentInChildren<Button>();
            EventTrigger eventTrigger = menuObject.GetComponentInChildren<EventTrigger>();

            if (menuText == null || eventTrigger == null)
            {
                Debug.LogError($"GameObject {menuObject.name}에 필요한 컴포넌트(TextMeshProUGUI, EventTrigger)가 없습니다.");
                continue;
            }

            if (menuButton != null)
            {
                menuButton.gameObject.SetActive(false);  // 버튼을 비활성화 상태로 설정
            }
            // 텍스트 설정
            if (i < made_menu.Count)
            {
                menuText.text = made_menu[i];
            }
            else
            {
                menuText.text = "";
            }
            // 텍스트 클릭 이벤트 설정 (EventTrigger 사용)
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            int currentIndex = i; // 람다식에서 사용할 currentIndex 값 캡처
            entry.callback.AddListener((eventData) => ActivateButton(menuButton, currentIndex)); // 텍스트 클릭 시 버튼 활성화
            eventTrigger.triggers.Add(entry);

            // 메뉴 버튼 클릭 이벤트 설정
            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners(); // 기존 이벤트 제거

            // 버튼 클릭 시 호출될 리스너 추가
            menuButton.onClick.AddListener(() =>
            {
                Debug.Log("Button clicked!");
                Debug.Log($"Current index i: {currentIndex}"); // currentIndex로 변경

                // i가 made_menu의 범위를 벗어나지 않는지 확인
                if (currentIndex >= 0 && currentIndex < made_menu.Count)
                {
                    Debug.Log("List index is within bounds.");
                    Debug.Log($"Checking menu: {made_menu[currentIndex]}");

                    // 메뉴 검증
                    CheckMenu(made_menu[currentIndex]);
                    // 메뉴 리스트에서 제거
                    RemoveMenuFromList(made_menu[currentIndex]);
                }
                else
                {
                    Debug.LogWarning($"Index {currentIndex} is out of bounds. List size: {made_menu.Count}");
                }
            });
        }
    }
    }
    // 버튼 활성화 및 이전 버튼 비활성화
    private void ActivateButton(Button targetButton, int currentIndex)
    {
        // 이전 활성 버튼 비활성화
        if (currentActiveButton != null)
        {
            currentActiveButton.SetActive(false);
        }

        // 새 버튼 활성화
        targetButton.gameObject.SetActive(true);
        currentActiveButton = targetButton.gameObject;

        // 버튼 클릭 이벤트를 활성화합니다.
        Debug.Log($"Button {currentIndex} activated");
    }

    // 선택된 메뉴 검증
    private void CheckMenu(string menu)
    {
        if (getmenu == null)
        {
            Debug.LogError("getMenu 스크립트가 설정되지 않았습니다.");
            return;
        }

        if (menu.Equals(getmenu.menuName))
        {
            Debug.Log($"선택된 메뉴가 올바릅니다: {menu}");
            getmenu.UpdateDialogue("True");
        }
        else
        {
            Debug.Log($"선택된 메뉴가 잘못되었습니다: {menu}");
            getmenu.UpdateDialogue("False");
        }
    }

    // 메뉴 리스트에서 제거 후 UI 업데이트
    private void RemoveMenuFromList(string menu)
    {
        if (made_menu.Contains(menu))
        {
            made_menu.Remove(menu);
            Debug.Log($"리스트에서 제거된 메뉴: {menu}");
            InitializeMenus(); // 메뉴 다시 초기화
        }
        else
        {
            Debug.LogWarning($"리스트에서 메뉴를 찾을 수 없습니다: {menu}");
        }
    }
}
