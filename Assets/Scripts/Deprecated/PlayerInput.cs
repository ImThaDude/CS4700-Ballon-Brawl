using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public Rigidbody2D rb;
    public Animator ac;
    public float Modifier = 10f;
    public float distToGround = 0.5f;
    public float MovementGroundModifier = 5f;
    public LayerMask groundMask;
    Vector2 dir;

    // Start is called before the first frame update
    void Start()
    {
        ac.SetBool("IsGrounded", false);
    }

    // Update is called once per frame
    void Update()
    {
        dir = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            dir = Vector2.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dir = Vector2.right;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            dir += Vector2.up;
            rb.AddForce(dir.normalized * Modifier);

            ac.SetTrigger("Flap");
        }

        if (IsGrounded) {
            rb.velocity = Vector2.zero;

            rb.velocity = dir * MovementGroundModifier;
        }

        ac.SetFloat("Movement", Mathf.Abs(rb.velocity.x));
        ac.SetBool("IsGrounded", IsGrounded);
        Debug.Log(IsGrounded);
        ac.SetFloat("Dir", dir.x);

    }

    bool IsGrounded {
        get {
            return Physics2D.Raycast(rb.transform.position, -Vector2.up, distToGround + 0.1f, groundMask);
        }
    }

}
