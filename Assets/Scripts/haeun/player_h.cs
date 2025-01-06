using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_h : MonoBehaviour
{
    private Rigidbody2D rigidbody_h;
    private Animator animator;
    private SpriteRenderer renderer_h;
    private BoxCollider2D boxCollider;
    private float speed = 5;
    private float horizontal;

    private bool isStopped = false; // 플레이어 정지 상태

    // 상태를 나타내는 enum
    private enum PlayerState
    {
        Idle,                      // 1. 완전 처음 상태
        MovingRight,               // 2. 오른쪽으로 이동 중
        StoppedAfterMovingRight,   // 3. 오른쪽 이동 후 정지
        MovingLeft,                // 4. 왼쪽으로 이동 중
        StoppedAfterMovingLeft     // 5. 왼쪽 이동 후 정지
    }
    private BoxCollider2D boxCollider1; // 첫 번째 박스 콜라이더
    private BoxCollider2D boxCollider2; // 두 번째 박스 콜라이더

    private PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        rigidbody_h = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer_h = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        // 두 개의 BoxCollider2D를 가져옴
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        if (colliders.Length >= 2)
        {
            boxCollider1 = colliders[0];
            boxCollider2 = colliders[1];
        }

        SetColliderState(PlayerState.Idle); // 초기 상태 설정
    }

    void Update()
    {
        // 게임 시작 전에는 움직이지 않음
        if (ingreGameManager_h.Instance.IsGameStarting()) return;

        horizontal = Input.GetAxis("Horizontal");

        // GameManager_h에서 현재 상태 확인
        if (ingreGameManager_h.Instance.IsGameOverFinalizing())
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

        if (horizontal > 0)
        {
            renderer_h.flipX = true;
            if (currentState != PlayerState.MovingRight)
            {
                SetColliderState(PlayerState.MovingRight);
            }
        }
        else if (horizontal < 0)
        {
            renderer_h.flipX = false;
            if (currentState != PlayerState.MovingLeft)
            {
                SetColliderState(PlayerState.MovingLeft);
            }
        }
        else
        {
            if (currentState == PlayerState.MovingRight)
            {
                SetColliderState(PlayerState.StoppedAfterMovingRight);
            }
            else if (currentState == PlayerState.MovingLeft)
            {
                SetColliderState(PlayerState.StoppedAfterMovingLeft);
            }
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
        SetColliderState(PlayerState.Idle); // 상태 초기화
    }

    private void SetColliderState(PlayerState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case PlayerState.Idle:
                ConfigureBoxColliders(0.5f, 1.8f, 0.5f);
                break;

            case PlayerState.MovingRight:
                ConfigureBoxColliders(0.9f, 2.0f, 0.9f);
                break;

            case PlayerState.StoppedAfterMovingRight:
                ConfigureBoxColliders(0.5f, 1.8f, 0.5f);
                break;

            case PlayerState.MovingLeft:
                ConfigureBoxColliders(-0.9f, 2.0f, -0.9f);
                break;

            case PlayerState.StoppedAfterMovingLeft:
                ConfigureBoxColliders(-0.5f, 1.8f, -0.5f);
                break;
        }
    }

    private void ConfigureBoxColliders(float offset1X, float size1X, float offset2X)
    {
        if (boxCollider1 != null)
        {
            // 첫 번째 박스 콜라이더
            boxCollider1.offset = new Vector2(offset1X, boxCollider1.offset.y);
            boxCollider1.size = new Vector2(size1X, boxCollider1.size.y);
        }

        if (boxCollider2 != null)
        {
            // 두 번째 박스 콜라이더
            boxCollider2.offset = new Vector2(offset2X, boxCollider2.offset.y);
            // size는 고정되므로 수정하지 않음
        }
    }
    
}
