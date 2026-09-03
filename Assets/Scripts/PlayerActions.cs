using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class PlayerActions : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    [Header("Speed")]
    [SerializeField] private int speed = 10;
    private float currentSpeed;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private int maxJumps = 2;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashCooldown = 2.0f;
    [SerializeField] private LayerMask dashObstacleLayer; // Capa de los obstáculos que bloquean el Dash
    [SerializeField] private float margenPared = 1f; // Distancia de seguridad para no quedar pegado dentro de la pared
    private bool canDash = true;
    private float dashTimer = 0f;
    private bool isDashing = false;

    private int jumpsRemaining;

    private KeyCode jumpKey = KeyCode.W;
    private KeyCode dashKey = KeyCode.D;

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

        if (Input.GetKeyDown(dashKey) && !isCrouching && canDash)
        {
            StartCoroutine(DashRoutine());
        }
        if (!canDash)
        {
            dashTimer -= Time.deltaTime;
        }
        if (dashTimer < 0 && !canDash && isGrounded)
        {
            canDash = true;
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        float direccionX = Mathf.Sign(currentSpeed);
        Vector2 direccionDash = new Vector2(direccionX, 0f);

        float distanciaEfectiva = dashDistance;

        Vector2 origenCast = (Vector2)transform.position + boxCollider.offset;
        Vector2 tamanoCaja = new Vector2(0.05f, boxCollider.size.y * 0.9f);

        RaycastHit2D hit = Physics2D.BoxCast(origenCast, tamanoCaja, 0f, direccionDash, dashDistance, dashObstacleLayer);

        if (hit.collider != null)
        {
            distanciaEfectiva = hit.distance - margenPared;
            if (distanciaEfectiva < 0) distanciaEfectiva = 0;
        }

        Vector2 posicionDestino = (Vector2)transform.position + new Vector2(direccionX * distanciaEfectiva, 0f);

        float gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        while (Vector2.SqrMagnitude((Vector2)transform.position - posicionDestino) > 0.02f)
        {
            transform.position = Vector2.MoveTowards(transform.position, posicionDestino, dashSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = posicionDestino;
        rb.gravityScale = gravedadOriginal;
        isDashing = false;
        canDash = false;
        dashTimer = dashCooldown;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            jumpsRemaining = maxJumps;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            currentSpeed *= -1;
        }
    }

    public KeyCode GetJumpKey()
    {
        return jumpKey;
    }
}