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
    [SerializeField] private float wallSize = 1f; // Distancia de seguridad para no quedar pegado dentro de la pared
    private bool canDash = true;
    private float dashTimer = 0f;
    private bool isDashing = false;

    [Header("Agachado (Crouch)")]
    [SerializeField] private float multiplicadorAlturaCollider = 0.5f;
    [SerializeField] private LayerMask roofLayer;
    [SerializeField] private float nextSize = 1f;
    [SerializeField] private float widthSize = 1f;

    private int jumpsRemaining;

    private bool isCrouching = false;
    private bool wantsToStandUp = false;

    private Vector2 originalColliderSize;
    private Vector2 offsetOriginalCollider;

    private KeyCode jumpKey = KeyCode.W;
    private KeyCode dashKey = KeyCode.D;
    private KeyCode crouchKey = KeyCode.Z;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        currentSpeed = speed;
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        transform.Translate(new Vector3(1 * currentSpeed * Time.deltaTime, 0, 0));

        if (isDashing) return;

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

        // --- LÓGICA DE AGACHARSE ---
        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            Agacharse();
        }
        else if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            wantsToStandUp = true;
        }

        if (wantsToStandUp && isCrouching)
        {
            TryToStandUp();
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
            distanciaEfectiva = hit.distance - wallSize;
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
    private void Agacharse()
    {
        isCrouching = true;
        wantsToStandUp = false;

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * multiplicadorAlturaCollider);
            float reduccion = originalColliderSize.y * (1f - multiplicadorAlturaCollider);
            boxCollider.offset = new Vector2(offsetOriginalCollider.x, offsetOriginalCollider.y - (reduccion / 2f));
        }
    }

    private void TryToStandUp()
    {
        if (!RoofAbove())
        {
            isCrouching = false;
            wantsToStandUp = false;

            if (boxCollider != null)
            {
                boxCollider.size = originalColliderSize;
                boxCollider.offset = offsetOriginalCollider;
            }
        }
    }

    private bool RoofAbove()
    {
        if (boxCollider == null) return false;

        Vector2 feetCenter = (Vector2)transform.position + offsetOriginalCollider;
        float direccion = Mathf.Sign(currentSpeed);

        feetCenter.x -= (direccion * nextSize / 2f);
        Vector2 fettSize = new Vector2(originalColliderSize.x + nextSize + widthSize, originalColliderSize.y);

        Collider2D solape = Physics2D.OverlapBox(feetCenter, fettSize, 0f, roofLayer);

        return solape != null;
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