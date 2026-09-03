using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerActions : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Speed")]
    [SerializeField] private int speed = 10;
    private float currentSpeed;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private int maxJumps = 2;

    private int jumpsRemaining;

    private KeyCode jumpKey = KeyCode.W;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        currentSpeed = speed;
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        transform.Translate(new Vector3(1 * currentSpeed * Time.deltaTime, 0, 0));

        if (Input.GetKeyDown(jumpKey) && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpsRemaining--;
            isGrounded = false;
        }
    }

    public KeyCode GetJumpKey()
    {
        return jumpKey;
    }
}