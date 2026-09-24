using TMPro;
using UnityEngine;

public class Administrador_QTE : MonoBehaviour
{
    [Header("Configuración de Tiempos")]
    [SerializeField] private float timeBetweenQTE = 20f;
    [SerializeField] private float timeToComplete = 2.5f;

    [Header("Referencias UI (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI arrowText;

    private float delayTimer;
    private float timeToCompleteTimer;
    private bool qteActive = false;

    private KeyCode correctKey;
    private Vector2 savePrevPlayerPosition;

    // Guardamos las referencias de los componentes del jugador por fuera
    private PlayerActions player;
    private Rigidbody2D playerRb;

    // Flechas direccionales para el QTE
    private KeyCode[] arrows = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    private string[] arrowsText = { "PRESIONÁ: ↑ (ARRIBA)", "PRESIONÁ: ↓ (ABAJO)", "PRESIONÁ: ← (IZQUIERDA)", "PRESIONÁ: → (DERECHA)" };

    void Start()
    {
        // Buscamos al jugador y sus componentes en la escena automáticamente
        player = FindFirstObjectByType<PlayerActions>();
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        delayTimer = timeBetweenQTE;

        if (arrowText != null)
            arrowText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!qteActive)
        {
            NormalModeHandler();
        }
        else
        {
            // Forzamos el congelamiento absoluto desactivando el script del jugador y frenando su física
            if (player != null) player.SetCurrentSpeed(0);
            if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

            QTEHandler();
        }
    }

    void NormalModeHandler()
    {
        delayTimer -= Time.deltaTime;

        if (delayTimer <= 0)
            ActivateQTE();
    }

    void ActivateQTE()
    {
        if (player == null) return;

        qteActive = true;
        timeToCompleteTimer = timeToComplete;

        // Guardamos la posición exacta antes del QTE para el reinicio
        savePrevPlayerPosition = player.transform.position;

        // Elegir flecha al azar
        int randomIndex = UnityEngine.Random.Range(0, arrows.Length);
        correctKey = arrows[randomIndex];

        // Mostrar texto
        if (arrowText != null)
        {
            arrowText.text = arrowsText[randomIndex];
            arrowText.color = Color.yellow;
            arrowText.gameObject.SetActive(true);
        }
    }

    void QTEHandler()
    {
        timeToCompleteTimer -= Time.deltaTime;

        if (timeToCompleteTimer <= 0)
        {
            FinishWithFail();
            return;
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(correctKey))
                CompleteQTESuccessfuly();
            else if (PressIncorrectKey())
                FinishWithFail();
        }
    }

    bool PressIncorrectKey()
    {
        foreach (KeyCode arrow in arrows)
        {
            if (Input.GetKeyDown(arrow) && arrow != correctKey)
                return true;
        }
        return false;
    }

    void CompleteQTESuccessfuly()
    {
        qteActive = false;
        delayTimer = timeBetweenQTE;

        if (arrowText != null)
            arrowText.gameObject.SetActive(false);

        UnityEngine.Debug.Log("¡QTE Correcto! Continuando juego.");
    }

    void FinishWithFail()
    {
        qteActive = false;
        delayTimer = timeBetweenQTE;

        if (arrowText != null)
            arrowText.gameObject.SetActive(false);

        UnityEngine.Debug.Log("¡QTE Fallado! El jugador muere y reinicia en el lugar.");

        // Devolvemos el control, lo teletransportamos a donde empezó el QTE y frenamos su inercia
        if (player != null)
        {
            player.SetPlayerDied(true);
        }
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }
    }

    public bool GetIsActive()
    {
        return qteActive;
    }
}