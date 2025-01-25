using UnityEngine;
using System.Collections;

public class LoginPanelManager : MonoBehaviour
{
    public GameObject LoginPanel; // 로그인 패널 (게스트/계정 로그인 선택)
    public ButtonManager buttonManager; // 버튼 관리자 참조
    public GuestLoginManager guestLoginManager; // 게스트 로그인 관리자 참조

    private void Start()
    {
        // 처음에는 로그인 패널 숨기기 (애니메이션 이후 4초 후 표시)
        LoginPanel.SetActive(false);

        // 4초 후 로그인 상태 확인
        StartCoroutine(ShowLoginPanelAfterDelay(4f));
    }

    // 지연 후 로그인 상태 확인 및 UI 업데이트
    private IEnumerator ShowLoginPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        bool isLoggedIn = guestLoginManager.IsGuestLoggedIn();
        Debug.Log($"게스트 로그인 상태: {isLoggedIn}");

        if (isLoggedIn)
        {
            // Show button if already logged in as guest
            Debug.Log("게스트 로그인 상태입니다. 버튼을 표시합니다.");
            buttonManager.ShowButtons();
        }
        else
        {
            // Show login panel if not logged in
            Debug.Log("로그인 상태가 아닙니다. 로그인 패널을 표시합니다.");
            LoginPanel.SetActive(true);
        }
    }

    // 게스트 로그인 버튼 클릭 시 호출
    public void OnGuestLogin()
    {
        guestLoginManager.GuestLogin(); // 게스트 로그인 처리
        Debug.Log("게스트 로그인 성공!");
        buttonManager.OnLoginSuccess(); // 로그인 성공 후 버튼 표시
        LoginPanel.SetActive(false); // 로그인 패널 숨기기
    }

    // 계정 로그인 버튼 클릭 시 호출 (추후 구현 가능)
    public void OnAccountLogin()
    {
        Debug.Log("계정 로그인 버튼 클릭됨 - 추후 구현 필요");
        // 계정 로그인 로직을 여기에 추가 예정
    }

    // 로그인 패널 닫기 버튼 클릭 시 호출
    public void OnCloseLoginPanel()
    {
        Debug.Log("로그인 패널 닫기");
        LoginPanel.SetActive(false); // 로그인 패널 숨기기
        buttonManager.ShowButtons(); // 버튼 다시 표시
    }
}




