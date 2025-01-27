using System;
using System.Collections;
using UnityEngine;

public class player_h : MonoBehaviour
{
    private Rigidbody2D rigidbody_h;
    private Animator animator;
    private SpriteRenderer renderer_h;
    private BoxCollider2D boxCollider;
    private float speed = 3f; // 초기 속도를 3으로 설정
    private float horizontal = -1; // 기본적으로 왼쪽으로 이동

    private bool isStopped = false; // 플레이어 정지 상태
    private bool isRightButtonPressed = false; // 오른쪽 버튼 상태
    private bool isPaused = false; // 게임 일시 정지 상태

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
        if (isPaused || ingreGameManager_h.Instance.IsGameStarting()) return;

        if (ingreGameManager_h.Instance.IsGameOverFinalizing())
        {
<<<<<<< HEAD
            GameOver(); // 게임 종료 시 한 번만 실행
=======
            StopPlayer();
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
            return;
        }

        HandleButtonInput();
        PlayerMove_h();
        ScreenClick();
    }

    private void PlayerMove_h()
    {
        if (isStopped) return;

        // 오른쪽 버튼 상태에 따라 방향 결정
        horizontal = isRightButtonPressed ? 1 : -1;

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

        rigidbody_h.velocity = new Vector2(horizontal * speed, rigidbody_h.velocity.y);
    }

    private void HandleButtonInput()
    {
        if (isPaused) return; // 일시 정지 상태에서는 입력 무시

        // 오른쪽 버튼 상태 업데이트
        if (Input.GetMouseButton(0)) // 버튼이 눌려 있는 동안
        {
            isRightButtonPressed = true;
        }
        else
        {
            isRightButtonPressed = false;
        }
    }

    private void ScreenClick()
    {
        if (isStopped || isPaused) return;

        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);

        if (worldpos.x < 0.1f) worldpos.x = 0.1f;
        if (worldpos.x > 0.9f) worldpos.x = 0.9f;

        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }

    private void StopPlayer()
    {
        if (!isStopped)
        {
            isStopped = true;
            animator.SetFloat("speed", 0);
            rigidbody_h.velocity = Vector2.zero;
            SetColliderState(PlayerState.Idle); // Idle 상태로 복원
        }
    }

    public void ResetPlayerState()
    {
        isStopped = false;
        isRightButtonPressed = false;

        animator.SetFloat("speed", 0);
        rigidbody_h.velocity = Vector2.zero;
        SetColliderState(PlayerState.Idle);
    }

    public void SetPauseState(bool pause)
    {
        isPaused = pause;

        if (pause)
        {
            // 일시 정지 시 움직임 멈춤
            rigidbody_h.velocity = Vector2.zero;
            animator.SetFloat("speed", 0);
            SetColliderState(PlayerState.Idle);
        }
<<<<<<< HEAD
        else{
            animator.SetFloat("speed", 1);
        }
=======
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
    }

    private void SetColliderState(PlayerState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case PlayerState.Idle:
                ConfigureBoxColliders(0.65f, 1.4f, 0.5f);
                break;

            case PlayerState.MovingRight:
                ConfigureBoxColliders(1f, 1.4f, 0.9f);
                break;

            case PlayerState.StoppedAfterMovingRight:
                ConfigureBoxColliders(0.65f, 1.4f, 0.5f);
                break;

            case PlayerState.MovingLeft:
                ConfigureBoxColliders(-1f, 1.4f, -0.9f);
                break;

            case PlayerState.StoppedAfterMovingLeft:
                ConfigureBoxColliders(-0.65f, 1.4f, -0.5f);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BadItem")
        {
            StartCoroutine(HandleBadItemCollision());
        }
    }

    private IEnumerator HandleBadItemCollision()
    {
        ingreGameManager_h.Instance.BackVoidScore();
        ingreGameManager_h.Instance.BackHeartScore(); // 생명 감소
        if (animator != null)
        {

            if (horizontal > 0)
            {
                renderer_h.flipX = true;
                if (currentState != PlayerState.StoppedAfterMovingRight)
                {
                    SetColliderState(PlayerState.StoppedAfterMovingRight);
                }
            }
            else if (horizontal < 0)
            {
                renderer_h.flipX = false;
                if (currentState != PlayerState.StoppedAfterMovingLeft)
                {
                    SetColliderState(PlayerState.StoppedAfterMovingLeft);
                }
            }

            rigidbody_h.velocity = new Vector2(horizontal * speed, rigidbody_h.velocity.y);
            
            animator.SetBool("badItem", true); // badItem 애니메이션 실행
            yield return new WaitForSeconds(1f); // 1초 대기
<<<<<<< HEAD

        // if (ingreGameManager_h.Instance.heartScore > 0)
        // {
            animator.SetBool("badItem", false); // 다시 달리는 상태로 복귀
        // }

        }

        
    }

    public void GameOver()
    {
        if (animator.GetBool("isGameOver")) return; // 이미 GameOver 상태면 실행 안 함

        animator.SetBool("isGameOver", true); // 게임 종료 상태 설정

        if (ingreGameManager_h.Instance.heartScore > 0)
        {
            Debug.Log("게임 종료 - 생명 있음 (Idle 상태)");
            animator.SetBool("badDeath", false); // BadDeath 해제
        }
        else
        {
            Debug.Log("게임 종료 - 생명 없음 (BadDeath 실행)");
            animator.SetBool("badDeath", true); // BadDeath 실행

        }

        animator.SetFloat("speed", 0);
        StopPlayer();
        if(speed >= 0f) {
                speed = 0f;
        }
=======
            animator.SetBool("badItem", false); // run 애니메이션으로 복귀
        }
        
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
    }
}
