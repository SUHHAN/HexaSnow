using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextFadeEffect : MonoBehaviour
{
    public TextMeshProUGUI textComponent;  // UI 텍스트
    public float fadeDuration = 1.0f;  // 페이드 지속 시간
    public Button SavePointButton;

    private void Start()
    {
        // 처음에는 투명하게 설정
        Color color = textComponent.color;
        textComponent.color = new Color(color.r, color.g, color.b, 0);

        
    }

    // 텍스트를 천천히 나타나게 하기
    public void ShowText()
    {
        StopAllCoroutines(); // 기존 코루틴 중지
        StartCoroutine(FadeText(1)); // Alpha 1로 변경 (완전히 보이도록)
    }

    // 텍스트를 천천히 사라지게 하기
    public void HideText()
    {
        StopAllCoroutines(); // 기존 코루틴 중지
        StartCoroutine(FadeText(0)); // Alpha 0으로 변경 (완전히 사라지도록)
    }

    // 페이드 효과 적용하는 함수
    IEnumerator FadeText(float targetAlpha)
    {
        Color color = textComponent.color;
        float startAlpha = color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            textComponent.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        textComponent.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

    public void ShowTextAndHide()
    {
        ShowText();
        Invoke("HideText", 2.0f); // 2초 후에 HideText() 실행
    }
}
