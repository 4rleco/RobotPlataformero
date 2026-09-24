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
        versionNumber.text = Version.GetVersionNumber();

        pauseMenu.SetActive(player.isPaused);
        
        if (lastJump != KeyCode.None && player.GetJumpKey() != lastJump) StartCoroutine(FlashRed(jumpKey));
        if (lastDash != KeyCode.None && player.GetDashKey() != lastDash) StartCoroutine(FlashRed(dashKey));
        if (lastCrouch != KeyCode.None && player.GetCrouchKey() != lastCrouch) StartCoroutine(FlashRed(crouchKey));

       
        lastJump = player.GetJumpKey();
        lastDash = player.GetDashKey();
        lastCrouch = player.GetCrouchKey();
    }

    
    private IEnumerator FlashRed(TextMeshProUGUI textMesh)
    {
        textMesh.color = Color.red;
        yield return new WaitForSeconds(1.0f);
        textMesh.color = Color.white;
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        player.isPaused = false;
        Time.timeScale = 1.0f;
    }
}