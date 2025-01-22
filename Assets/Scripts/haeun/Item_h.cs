using System.Collections;
using UnityEngine;

public class Item_h : MonoBehaviour
{
    private bool isInitialized = false;

    void Start()
    {
        StartCoroutine(InitializeDelay());
    }

    private IEnumerator InitializeDelay()
    {
        yield return new WaitForSeconds(0.1f); // 초기화 지연
        isInitialized = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInitialized) return; // 초기화 완료 전 충돌 무시

        if (collision.gameObject.tag == "Ground")
        {
            Destroy(this.gameObject); // 땅에 닿으면 삭제
        }
        else if (collision.gameObject.tag == "player")
        {
            if (gameObject.tag == "GoodItem") // goodItem 태그 확인
            {
                ingreGameManager_h.Instance.GetVoidScore(); // 점수 증가
            }
            else if (gameObject.tag == "BadItem") // badItem 태그 확인
            {
                ingreGameManager_h.Instance.BackVoidScore();
                ingreGameManager_h.Instance.BackHeartScore(); // 생명 감소
            }
            Destroy(this.gameObject);
        }
    }
}
