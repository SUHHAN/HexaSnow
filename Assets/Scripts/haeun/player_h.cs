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
    private float speed = 3f; // 초기 속도를 3으로 설정
    private float maxSpeed = 5f; // 최대 속도
    private float dragDuration = 1f; // 속도가 최대가 되는 시간
    private float horizontal;

    private bool isDragging = false; // 드래그 상태 확인
    private float dragStartTime; // 드래그 시작 시간

    private bool isStopped = false; // 플레이어 정지 상태

    private enum PlayerState
    {
        Idle,
        MovingRight,
        StoppedAfterMovingRight,
        MovingLeft,
        StoppedAfterMovingLeft
    }

    private BoxCollider2D boxCollider1;
    private BoxCollider2D boxCollider2;

    private PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        rigidbody_h = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer_h = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        if (colliders.Length >= 2)
        {
            boxCollider1 = colliders[0];
            boxCollider2 = colliders[1];
        }

        SetColliderState(PlayerState.Idle);
    }

    void Update()
    {
        if (ingreGameManager_h.Instance.IsGameStarting()) return;

        horizontal = Input.GetAxis("Horizontal");

        HandleScreenTouch();

        if (ingreGameManager_h.Instance.IsGameOverFinalizing())
        {
            StopPlayer();
            return;
        }

        PlayerMove_h();
        ScreenClick();
    }

    private void PlayerMove_h()
    {
        if (isStopped) return;

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

    private void HandleScreenTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartTime = Time.time; // 드래그 시작 시간 기록
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            float elapsedTime = Time.time - dragStartTime; // 드래그 시간 계산
            speed = Mathf.Lerp(0, maxSpeed, elapsedTime / dragDuration); // 선형 보간으로 속도 증가
            speed = Mathf.Min(speed, maxSpeed); // 최대 속도를 초과하지 않도록 제한

            Vector3 touchPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (touchPosition.x < 0.5f)
            {
                horizontal = -1;
            }
            else if (touchPosition.x >= 0.5f)
            {
                horizontal = 1;
            }
        }
        else if (Mathf.Approximately(Input.GetAxis("Horizontal"), 0f))
        {
            horizontal = 0;
        }
    }

    private void ScreenClick()
    {
        if (isStopped) return;

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
            animator.SetFloat("speed", 0);
            rigidbody_h.velocity = Vector2.zero;
        }
    }

    public void ResetPlayerState()
    {
        isStopped = false;
        isDragging = false; // 드래그 상태 확인

        animator.SetFloat("speed", 0);
        rigidbody_h.velocity = Vector2.zero;
        SetColliderState(PlayerState.Idle);
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
            boxCollider1.offset = new Vector2(offset1X, boxCollider1.offset.y);
            boxCollider1.size = new Vector2(size1X, boxCollider1.size.y);
        }

        if (boxCollider2 != null)
        {
            boxCollider2.offset = new Vector2(offset2X, boxCollider2.offset.y);
        }
    }
}
