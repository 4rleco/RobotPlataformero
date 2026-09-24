using System;
using System.Collections; 
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI jumpKey;
    [SerializeField] private TextMeshProUGUI dashKey;
    [SerializeField] private TextMeshProUGUI crouchKey;
    [SerializeField] private TextMeshProUGUI versionNumber;

    [SerializeField] private PlayerActions player;
    [SerializeField] private Version version;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject loseMenu;

    private KeyCode lastJump;
    private KeyCode lastDash;
    private KeyCode lastCrouch;

    private void Start()
    {
        pauseMenu.SetActive(false);
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
    }

    private void Update()
    {
        
        jumpKey.text = "Jump Key: " + player.GetJumpKey();
        dashKey.text = "Dash Key: " + player.GetDashKey();
        crouchKey.text = "Crouch Key: " + player.GetCrouchKey();
        versionNumber.text = version.GetVersionNumber();

        pauseMenu.SetActive(player.isPaused);

        float cooldownRestante = player.GetDashCooldownTimer();
        if (cooldownRestante > 0)
        {
         
            dashKey.text = "Dash Key: " + player.GetDashKey() + " [ " + cooldownRestante.ToString("F1") + "s]";
        }
        else
        {
            
            dashKey.text = "Dash Key: " + player.GetDashKey() + " [READY]";
        }

        if (lastJump != KeyCode.None && player.GetJumpKey() != lastJump) StartCoroutine(FlashRed(jumpKey));
        if (lastDash != KeyCode.None && player.GetDashKey() != lastDash) StartCoroutine(FlashRed(dashKey));
        if (lastCrouch != KeyCode.None && player.GetCrouchKey() != lastCrouch) StartCoroutine(FlashRed(crouchKey));

       
        lastJump = player.GetJumpKey();
        lastDash = player.GetDashKey();
        lastCrouch = player.GetCrouchKey();

        if(player.GetPlayerWin())
        {
            Time.timeScale = 0.0f;
            winMenu.SetActive(true);
        }
    }

    
    private IEnumerator FlashRed(TextMeshProUGUI textMesh)
    {
        textMesh.color = Color.red;
        yield return new WaitForSeconds(1.0f);
        textMesh.color = Color.white;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
        winMenu.SetActive(false);
        player.SetPlayerWin(false);
    }

    public void ResetGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        player.isPaused = false;
        Time.timeScale = 1.0f;
    }
}