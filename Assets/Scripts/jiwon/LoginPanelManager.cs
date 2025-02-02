using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanelManager : MonoBehaviour
{
    public GameObject LoginPanel; // 로그인 패널 (게스트/계정 로그인 선택)
    public ButtonManager buttonManager; // 버튼 관리자 참조
    public GuestLoginManager guestLoginManager; // 게스트 로그인 관리자 참조
    public GameObject SettingButton; // 설정 버튼 UI
    public Button button;

    private void Start()
    {
        // 처음에는 로그인 패널 숨기기 (애니메이션 이후 4초 후 표시)
        LoginPanel.SetActive(false);

        // 4초 후 로그인 상태 확인
        StartCoroutine(ShowLoginPanelAfterDelay(4f));
        button.onClick.AddListener(Deadline);
    }

    // 지연 후 로그인 상태 확인 및 UI 업데이트
    private IEnumerator ShowLoginPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SettingButton.SetActive(true);
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

    public void OnGuestLogin()
    {
        guestLoginManager.GuestLogin();
        buttonManager.OnLoginSuccess();
        LoginPanel.SetActive(false);
    }

    // 계정 로그인 버튼 클릭 시 호출 (추후 구현 가능)
    public void OnAccountLogin()
    {
        Debug.Log("계정 로그인 버튼 클릭됨 - 추후 구현 필요");
        // 계정 로그인 로직을 여기에 추가 예정
    }

    public void Deadline()
    {
        SceneManager.LoadScene("Deadline");
    }
}




