using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_h : MonoBehaviour
{
    private Rigidbody2D rigidbody_h;
    private Animator animator;
    private SpriteRenderer renderer_h;
    private float speed = 5;
    private float horizontal;

    private bool isStopped = false; // 플레이어 정지 상태

    void Start()
    {
        rigidbody_h = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer_h = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // GameManager_h에서 현재 상태 확인
        if (GameManager_h.Instance.IsGameOverFinalizing())
        {
            StopPlayer(); // 플레이어 정지
            return; // 움직임 처리 종료
        }

        horizontal = Input.GetAxis("Horizontal");
        PlayerMove_h();
        ScreenClick();
    }

    private void PlayerMove_h()
    {
        if (isStopped) return; // 정지 상태에서는 움직이지 않음

        animator.SetFloat("speed", Mathf.Abs(horizontal));

        if (horizontal < 0)
        {
            renderer_h.flipX = true;
        }
        else if (horizontal > 0)
        {
            renderer_h.flipX = false;
        }

        rigidbody_h.velocity = new Vector2(horizontal * speed, rigidbody_h.velocity.y);
    }

    private void ScreenClick()
    {
        if (isStopped) return; // 정지 상태에서는 화면 경계 처리도 생략

        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);

        if (worldpos.x < 0.05f) worldpos.x = 0.05f;
        if (worldpos.x > 0.95f) worldpos.x = 0.95f;

        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }

    private void StopPlayer()
    {
        if (!isStopped)
        {
            isStopped = true;
            animator.SetFloat("speed", 0); // 애니메이션 정지
            rigidbody_h.velocity = Vector2.zero; // 물리적 움직임 정지
        }
    }

    public void ResetPlayerState()
    {
        isStopped = false; // 정지 상태 해제
        animator.SetFloat("speed", 0); // 애니메이션 초기화
        rigidbody_h.velocity = Vector2.zero; // 속도 초기화
    }
}
