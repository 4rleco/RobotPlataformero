using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerActions : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded;

    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Start")]
    [SerializeField] private GameObject startPoint;

    [Header("Finish")]
    [SerializeField] private GameObject endPoint;

    [Header("Speed")]
    [SerializeField] private int speed = 10;
    private float currentSpeed;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashCooldown = 2.0f;
    [SerializeField] private LayerMask dashObstacleLayer; // Capa de los obstáculos que bloquean el Dash
    [SerializeField] private float wallOffset = 1f; // Distancia de seguridad para no quedar pegado dentro de la pared
    private bool canDash = true;
    private float dashTimer = 0f;
    private bool isDashing = false;


    [Header("Slide")]
    [SerializeField] private float heightColliderMultiplier = 0.5f;
    [SerializeField] private LayerMask roofLayer;
    [SerializeField] private float nextOffset = 1f;
    [SerializeField] private float widthOffset = 1f;

    [Header("Key change timer")]
    [SerializeField] private float keyTimer = 3.0f;

    // --- NUEVOS ARRAYS FILAS QWERTY ---
    private readonly KeyCode[] upperLine =
    {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
        KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P
    };

    private readonly KeyCode[] middleLine =
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G,
        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L
    };

    private readonly KeyCode[] bottomLine =
    {
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V,
        KeyCode.B, KeyCode.N, KeyCode.M
    };

    private bool isCrouching = false;
    private bool wantsToStandUp = false;
    private bool playerDied = false;
    private BoxCollider2D boxCollider;
    private Vector2 colliderOriginalSize;
    private Vector2 colliderOriginalOffset;

    // Teclas iniciales asignadas según su respectiva fila
    private KeyCode jumpKey = KeyCode.W;
    private KeyCode dashKey = KeyCode.D;
    private KeyCode crouchKey = KeyCode.Z;

    private float currentKeyTimer = 0.0f;

    Vector3 screenRelativePosition;

    void Start()
    {
        transform.position = startPoint.transform.position;

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            colliderOriginalSize = boxCollider.size;
            colliderOriginalOffset = boxCollider.offset;
        }

        jumpsRemaining = maxJumps;

        currentSpeed = speed;
        currentKeyTimer = keyTimer;
    }

    void Update()
    {
        if (playerDied)
        {
            OnPlayerDied();
            return;
        }

        transform.Translate(new Vector3(1 * currentSpeed * Time.deltaTime, 0, 0));

        if (isDashing) return;
        // --- LÓGICA DE AGACHARSE ---
        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            Slide();
        }
        else if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            wantsToStandUp = true;
        }

        if (wantsToStandUp && isCrouching)
        {
            TryToStandUp();
        }

        // --- SALTO (No se permite si está agachado) --
        if (Input.GetKeyDown(jumpKey) && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpsRemaining--;
            isGrounded = false;
        }

        // --- DASH (No se permite si está agachado) ---
        if (Input.GetKeyDown(dashKey) && !isCrouching && canDash && currentSpeed > 0)
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

        if (currentKeyTimer <= 0.0f)
        {
            SetRandKey();

            currentKeyTimer += keyTimer;
        }

        if (currentSpeed < speed)
            currentSpeed += 0.1f;

        currentKeyTimer -= Time.deltaTime;

        screenRelativePosition = Camera.main.WorldToViewportPoint(transform.position);

        if (screenRelativePosition.y < 0)
            playerDied = true;
    }

    // Método modificado para elegir una tecla aleatoria de un array específico
    private KeyCode GetRandKeyFromRow(KeyCode[] fila)
    {
        int randomIndex = UnityEngine.Random.Range(0, fila.Length);
        return fila[randomIndex];
    }

    private void SetRandKey()
    {
        int randNum = UnityEngine.Random.Range(0, 3);

        switch (randNum)
        {
            case 0: // JUMP -> Fila Superior
                KeyCode prevJump = jumpKey;
                do
                {
                    jumpKey = GetRandKeyFromRow(upperLine);
                }
                // Evitamos que se repita la misma tecla de salto anterior
                while (jumpKey == prevJump);
                break;

            case 1: // DASH -> Fila Media
                KeyCode prevDash = dashKey;
                do
                {
                    dashKey = GetRandKeyFromRow(middleLine);
                }
                while (dashKey == prevDash);
                break;

            case 2: // CROUCH -> Fila Inferior
                KeyCode prevCrouch = crouchKey;
                do
                {
                    crouchKey = GetRandKeyFromRow(bottomLine);
                }
                while (crouchKey == prevCrouch);
                break;
        }
    }

    private void Slide()
    {
        isCrouching = true;
        wantsToStandUp = false;

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(colliderOriginalSize.x, colliderOriginalSize.y * heightColliderMultiplier);
            float reduccion = colliderOriginalSize.y * (1f - heightColliderMultiplier);
            boxCollider.offset = new Vector2(colliderOriginalOffset.x, colliderOriginalOffset.y - (reduccion / 2f));
        }
    }

    private void TryToStandUp()
    {
        if (!IsThereRoofAbove())
        {
            isCrouching = false;
            wantsToStandUp = false;

            if (boxCollider != null)
            {
                boxCollider.size = colliderOriginalSize;
                boxCollider.offset = colliderOriginalOffset;
            }
        }
    }

    private bool IsThereRoofAbove()
    {
        if (boxCollider == null) return false;

        Vector2 footCenter = (Vector2)transform.position + colliderOriginalOffset;
        float direccion = Mathf.Sign(currentSpeed);

        footCenter.x -= (direccion * nextOffset / 2f);
        Vector2 footSize = new Vector2(colliderOriginalSize.x + nextOffset + widthOffset, colliderOriginalSize.y);

        Collider2D solape = Physics2D.OverlapBox(footCenter, footSize, 0f, roofLayer);

        return solape != null;
    }

    private IEnumerator DashRoutine()
    {

        isDashing = true;

        float directionX = Mathf.Sign(currentSpeed);
        Vector2 dasDirection = new Vector2(directionX, 0f);

        float efectiveDistance = dashDistance;

        Vector2 castOrigin = (Vector2)transform.position + boxCollider.offset;
        Vector2 boxSize = new Vector2(0.05f, boxCollider.size.y * 0.9f);

        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, boxSize, 0f, dasDirection, dashDistance, dashObstacleLayer);

        if (hit.collider != null)
        {
            efectiveDistance = hit.distance - wallOffset;
            if (efectiveDistance < 0) efectiveDistance = 0;
        }

        Vector2 finalPosition = (Vector2)transform.position + new Vector2(directionX * efectiveDistance, 0f);
        Debug.Log(finalPosition.x);


        float originalGravity = rb.gravityScale;
        bool hitCollision = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        while (Vector2.SqrMagnitude((Vector2)transform.position - finalPosition) > 0.02f)
        {
            if (currentSpeed <= 0)
            {
                Debug.Log("entro al bucle");
                hitCollision = true;
                break;
            }
            transform.position = Vector2.MoveTowards(transform.position, finalPosition, dashSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("finalizando corrutina");

        if (!hitCollision)
            transform.position = finalPosition;

        rb.gravityScale = originalGravity;
        isDashing = false;
        canDash = false;
        dashTimer = dashCooldown;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {

            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 colliderCenter = (Vector2)transform.position + boxCollider.offset;


            if (contactPoint.y < colliderCenter.y)
            {
                isGrounded = true;
                jumpsRemaining = maxJumps;
            }
        }


        if (collision.gameObject.CompareTag("Obstacle"))
        {
            currentSpeed *= -1.5f;
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            Time.timeScale = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;

            if (isCrouching && !IsThereRoofAbove())
            {
                isCrouching = false;
                wantsToStandUp = false;

                if (boxCollider != null)
                {
                    boxCollider.size = colliderOriginalSize;
                    boxCollider.offset = colliderOriginalOffset;
                }
            }
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;

        // Visualización del techo
        Vector2 footCenter = (Vector2)transform.position + colliderOriginalOffset;
        float direction = Application.isPlaying ? Mathf.Sign(currentSpeed) : 1f;

        footCenter.x -= (direction * nextOffset / 2f);
        Vector2 footSize = new Vector2(colliderOriginalSize.x + nextOffset + widthOffset, colliderOriginalSize.y);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(footCenter, footSize);

        // Visualización del Dash Raycast
        Gizmos.color = Color.cyan;
        Vector2 dashOrigin = (Vector2)transform.position + boxCollider.offset;
        Gizmos.DrawRay(dashOrigin, new Vector2(direction * dashDistance, 0f));
    }
#endif

    private void OnPlayerDied()
    {
        transform.position = startPoint.transform.position;
    }

    public KeyCode GetJumpKey()
    {
        return jumpKey;
    }

    public KeyCode GetDashKey()
    {
        return dashKey;
    }
    public KeyCode GetCrouchKey()
    {
        return crouchKey;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void SetCurrentSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public bool GetPlayerDied()
    {
        return playerDied;
    }

    public void SetPlayerDied(bool died)
    {
        playerDied = died;
    }
}