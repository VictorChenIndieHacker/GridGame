using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Components
    Rigidbody2D rb;

    //Player
    float walkSpeed = 4f;
    float inputHorizontal;
    float inputVertical;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        if (inputHorizontal!=0||inputVertical!=0)
        {
            animator.SetFloat("X", inputHorizontal);
            animator.SetFloat("Y", inputVertical);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(inputHorizontal,inputVertical).normalized*walkSpeed;
    }
}
