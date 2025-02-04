using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PanelMover : MonoBehaviour
{
    public RectTransform panelTransform; // 이동할 UI 패널 (상단)
    public TextMeshProUGUI newDateText; // 새로운 일차를 알리는 알림 텍스트
    public float targetY = 560f;  // 최종적으로 내려올 위치
    private float moveDuration = 0.5f; // 이동에 걸리는 시간

    private float startY = 700f; // 시작 위치 (화면 밖)
    private float endY = 700f;   // 사라질 위치 (다시 화면 밖)

    [SerializeField] private GameData GD = new GameData();
    [SerializeField] private int mydate = 0;

    

    void Start()
    {
        // 시작 시 패널을 화면 밖으로 이동
        panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, startY);

        // 🎯 `FadeManager`의 onFadeComplete 이벤트가 발생하면 MovePanelSequence 실행
        FindObjectOfType<FadeManager>().onFadeComplete += StartMovingPanel;
    }

    void StartMovingPanel()
    {
        LoadDate();

        newDateText.text = $"{mydate}일차 가게 오픈";

        StartCoroutine(MovePanelSequence());
    }

    IEnumerator MovePanelSequence()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.bell);

        // 1️⃣ 위에서 아래로 천천히 이동
        yield return StartCoroutine(MovePanel(startY, targetY));

        // 2️⃣ 2초 유지
        yield return new WaitForSeconds(2f);

        // 3️⃣ 다시 위로 천천히 이동 (사라짐)
        yield return StartCoroutine(MovePanel(targetY, endY));

        yield return new WaitForSeconds(1.5f);

        GoGame();
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

        panelTransform.anchoredPosition = new Vector2(panelTransform.anchoredPosition.x, toY);
    }

    void GoGame() {
        SceneManager.LoadScene("order1");
    }

    private void LoadDate() {

        GD = DataManager.Instance.LoadGameData();

        // !! 일차 업데이트하기
        mydate = GD.date;
    }
}
