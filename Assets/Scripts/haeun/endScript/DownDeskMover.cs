using System.Collections;
using UnityEngine;

public class DownDeskMover : MonoBehaviour
{
    public RectTransform panelTransform; // 이동할 UI 패널 (하단)
    public float targetY = 0f;  // 최종적으로 올라올 위치
    public float moveDuration = 1.5f; // 이동에 걸리는 시간

    private float startY = -470f; // 시작 위치 (화면 아래)

    void Start()
    {
        // 시작 시 패널을 아래쪽에 배치
        panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, startY);

        // 🎯 `FadeManager`의 onFadeComplete 이벤트가 발생하면 MovePanel 실행
        FindObjectOfType<FadeManager>().onFadeComplete += StartMovingPanel;
    }

    void StartMovingPanel()
    {
        StartCoroutine(MovePanel(startY, targetY));
    }

    IEnumerator MovePanel(float fromY, float toY)
    {
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t); // 부드러운 Ease In-Out 적용
            float newY = Mathf.Lerp(fromY, toY, smoothT);
            panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, newY);
            yield return null;
        }

        // 도착 후 유지
        panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, toY);
    }
}
