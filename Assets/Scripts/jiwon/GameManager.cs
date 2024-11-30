using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("게임 종료 시도");

        // 애플리케이션 종료
        Application.Quit();

        // Unity 에디터에서 실행 중일 때 종료 처리
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
