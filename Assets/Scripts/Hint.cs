using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{

    [SerializeField] private GameObject JumpTutorial;
    [SerializeField] private GameObject SlideTutorial;
    [SerializeField] private GameObject DashTutorial;
    [SerializeField] private GameObject dashTrigger;
    [SerializeField] private Image SlideCanva;
    [SerializeField] private Image DashCanva;
    [SerializeField] private Image JumpCanva;
    [SerializeField] private PlayerActions owner;

    void Start()
    {
        PlayerEvents.current.onJumpTutorialTriggerEnter += OnJumpOpen;

        PlayerEvents.current.onDashTutorialTriggerEnter += OnDashOpen;

        PlayerEvents.current.onSlideTutorialTriggerEnter += OnSlideOpen;

        PlayerEvents.current.onPlayerDashTriggerClose += OnDashClose;

        PlayerEvents.current.onPlayerSlideTriggerClose += OnSlideClose;

        PlayerEvents.current.onPlayerJumpTriggerClose += OnJumpClose;

        PlayerEvents.current.onPlayerDeath += OnPlayerDeath;
    }
    private void OnJumpOpen(PlayerActions player)
    {
        if (player != owner) return;

        JumpCanva.gameObject.SetActive(true);
        DashCanva.gameObject.SetActive(false);

    }

    private void OnJumpClose(PlayerActions player)
    {
        if (player != owner) return;

        JumpCanva.gameObject.SetActive(false);
        

    }
    private void OnDashOpen(PlayerActions player)
    {
        if (player != owner) return;

        DashCanva.gameObject.SetActive(true);

    }

    private void OnDashClose(PlayerActions player)
    {
        if (player != owner) return;
        DashCanva.gameObject.SetActive(false);
      

    }
    private void OnSlideOpen(PlayerActions player)
    {
      
        if (player != owner) return;

        SlideCanva.gameObject.SetActive(true);
        

    }

    private void OnSlideClose(PlayerActions player)
    {
        Debug.Log("cierra");
        if (player != owner) return;

        SlideCanva.gameObject.SetActive(false);
       

    }

    private void OnPlayerDeath(PlayerActions player)
    {
        if (player != owner) return;

        if (DashCanva.gameObject.activeSelf)
            dashTrigger.SetActive(true);

        JumpCanva.gameObject.SetActive(false);
        DashCanva.gameObject.SetActive(false);
        SlideCanva.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (PlayerEvents.current == null) return;
        PlayerEvents.current.onJumpTutorialTriggerEnter -= OnJumpOpen;
        PlayerEvents.current.onDashTutorialTriggerEnter -= OnDashClose;
        PlayerEvents.current.onSlideTutorialTriggerEnter -= OnSlideClose;
        PlayerEvents.current.onPlayerDeath-=OnPlayerDeath;
    }

}
