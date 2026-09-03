using TMPro;
using UnityEngine;

public class Administrador_QTE : MonoBehaviour
{
    [Header("Configuración de Tiempos")]
    [SerializeField] private float timeBetweenQTE = 20f;
    [SerializeField] private float timeToComplete = 2.5f;
    [SerializeField] private GameObject startPoint;

    [Header("Referencias UI (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI arrowText;

    private float delayTimer;
    private float timeToCompleteTimer;
    private bool qteActive = false;

    private KeyCode correctKey;
    private Vector2 savePrevPlayerPosition;

    // Guardamos las referencias de los componentes del jugador por fuera
    private PlayerActions playerScript;
    private Rigidbody2D playerRb;

    // Flechas direccionales para el QTE
    private KeyCode[] arrows = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    private string[] arrowsText = { "PRESIONÁ: ↑ (ARRIBA)", "PRESIONÁ: ↓ (ABAJO)", "PRESIONÁ: ← (IZQUIERDA)", "PRESIONÁ: → (DERECHA)" };

    void Start()
    {
        // Buscamos al jugador y sus componentes en la escena automáticamente
        playerScript = FindFirstObjectByType<PlayerActions>();
        if (playerScript != null)
        {
            playerRb = playerScript.GetComponent<Rigidbody2D>();
        }

        delayTimer = timeBetweenQTE;

        if (arrowText != null)
            arrowText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!qteActive)
        {
            ManejarModoNormal();
        }
        else
        {
            // Forzamos el congelamiento absoluto desactivando el script del jugador y frenando su física
            if (playerScript != null) playerScript.enabled = false;
            if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

            ManejarModoQTE();
        }
    }

    void ManejarModoNormal()
    {
        delayTimer -= Time.deltaTime;

        if (delayTimer <= 0)
            ActivarQTE();
    }

    void ActivarQTE()
    {
        if (playerScript == null) return;

        qteActive = true;
        timeToCompleteTimer = timeToComplete;

        // Guardamos la posición exacta antes del QTE para el reinicio
        savePrevPlayerPosition = playerScript.transform.position;

        // Elegir flecha al azar
        int indiceAleatorio = UnityEngine.Random.Range(0, arrows.Length);
        correctKey = arrows[indiceAleatorio];

        // Mostrar texto
        if (arrowText != null)
        {
            arrowText.text = arrowsText[indiceAleatorio];
            arrowText.color = Color.yellow;
            arrowText.gameObject.SetActive(true);
        }
    }

    void ManejarModoQTE()
    {
        timeToCompleteTimer -= Time.deltaTime;

        if (timeToCompleteTimer <= 0)
        {
            TerminarConFallo();
            return;
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(correctKey))
                TerminarConExito();
            else if (PresionoFlechaIncorrecta())
                TerminarConFallo();
        }
    }

    bool PresionoFlechaIncorrecta()
    {
        foreach (KeyCode flecha in arrows)
        {
            if (Input.GetKeyDown(flecha) && flecha != correctKey)
                return true;
        }
        return false;
    }

    void TerminarConExito()
    {
        qteActive = false;
        delayTimer = timeBetweenQTE;

        if (arrowText != null)
            arrowText.gameObject.SetActive(false);

        // Devolvemos el control al script del jugador de forma segura
        if (playerScript != null) playerScript.enabled = true;

        UnityEngine.Debug.Log("¡QTE Correcto! Continuando juego.");
    }

    void TerminarConFallo()
    {
        qteActive = false;
        delayTimer = timeBetweenQTE;

        if (arrowText != null)
            arrowText.gameObject.SetActive(false);

        UnityEngine.Debug.Log("¡QTE Fallado! El jugador muere y reinicia en el lugar.");

        // Devolvemos el control, lo teletransportamos a donde empezó el QTE y frenamos su inercia
        if (playerScript != null)
        {
            playerScript.transform.position = startPoint.transform.position;
            playerScript.enabled = true;
        }
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }
    }
}