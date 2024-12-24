using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_h : MonoBehaviour
{
    private Rigidbody2D rigidbody_h;
    private Animator animator;
    private SpriteRenderer renderer_h;
    private float speed = 4;
    private float horizontal;

    void Start() {
        rigidbody_h = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer_h = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        
        PlayerMove_h();
        ScreenClick();
    }

    private void PlayerMove_h() {
        animator.SetFloat("speed", Mathf.Abs(horizontal));

        if(horizontal < 0) {
            renderer_h.flipX = true;
        }
        else if(horizontal > 0) {
            renderer_h.flipX = false;
        }
        rigidbody_h.velocity = new Vector2(horizontal * speed, rigidbody_h.velocity.y);
    }

    private void ScreenClick() {
        Vector3 worldpos = Camera.main.WorldToViewportPoint(this.transform.position);

        if(worldpos.x < 0.05f) worldpos.x = 0.05f;
        if(worldpos.x > 0.95f) worldpos.x = 0.95f;

        this.transform.position = Camera.main.ViewportToWorldPoint(worldpos);
    }   
}
