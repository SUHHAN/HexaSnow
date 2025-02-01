using System.Collections;
using UnityEngine;
using System;

public class FadeManager : MonoBehaviour
{
    public CanvasGroup lobbyPanel;   // 기존 패널 (페이드 아웃 대상)
    public CanvasGroup blackPanel;   // 검은 화면 (페이드 인 & 유지)
    public CanvasGroup newPanel;     // 새로운 패널 (페이드 인 대상)
    
    public Action onFadeComplete;    // 페이드 완료 시 실행할 콜백

    private float fadeDuration = 2f; // 페이드 시간

    void Start()
    {
        AudioManager.Instance.PlayBgm(AudioManager.Bgm.inside_kitchen_baking);
        StartCoroutine(HandlePanelTransition());
    }

    IEnumerator HandlePanelTransition()
    {
        // 1️⃣ 로비 화면 3.5초 유지
        yield return new WaitForSeconds(3.5f);

        // 2️⃣ 로비 화면 페이드 아웃 + 검은 화면 페이드 인 (동시에)
        yield return StartCoroutine(FadeOutAndBlackIn(lobbyPanel, blackPanel));

        // 3️⃣ 검은 화면 유지 (2초)
        yield return new WaitForSeconds(2f);

        // 4️⃣ 새로운 패널이 검은 화면 위에서 페이드 인
        yield return StartCoroutine(FadeIn(newPanel));

        // 5️⃣ 검은 화면 페이드 아웃 (완전히 사라짐)
        yield return StartCoroutine(FadeOut(blackPanel));

        // 🎯 페이드 완료 후, 상단 & 하단 패널 이동 시작
        onFadeComplete?.Invoke();
    }

    IEnumerator FadeOutAndBlackIn(CanvasGroup fadeOutPanel, CanvasGroup fadeInPanel)
    {
        fadeInPanel.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = elapsedTime / fadeDuration;
            fadeOutPanel.alpha = Mathf.Lerp(1, 0, alpha);
            fadeInPanel.alpha = Mathf.Lerp(0, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeOutPanel.alpha = 0;
        fadeOutPanel.gameObject.SetActive(false);
        fadeInPanel.alpha = 1;
    }

    IEnumerator FadeIn(CanvasGroup panel)
    {
        panel.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            panel.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panel.alpha = 1;
    }

    IEnumerator FadeOut(CanvasGroup panel)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            panel.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panel.alpha = 0;
        panel.gameObject.SetActive(false);
    }
}
