using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    Animator anim;
    public event Action OnEncountered;
    public float speed;
    Vector2 movement;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x != 0)
        {
            transform.localScale = new Vector3(movement.x, 1, 1);
        }
        SwitchAnim();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement *speed*Time.fixedDeltaTime);
    }

    void SwitchAnim() 
    {
        anim.SetFloat("speed", movement.magnitude);    
    }

    void OnCollisionEnter2D(Collision2D other)  //other是指碰撞到的東西 
    {
        if (other.gameObject.tag == "Monster")
        {
            OnEncountered();
        }


    }
}
