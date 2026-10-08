using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] int tutorialSteps = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerActions player = other.GetComponentInParent<PlayerActions>();

        if (player == null) return;
       

        switch (tutorialSteps)
        {
            case 1:
                PlayerEvents.current.OnSlideTutorialTriggerEnter(player);
                break;
            case 2:
                PlayerEvents.current.OnDashTutorialTriggerEnter(player);
                break;
            case 3:
                PlayerEvents.current.OnJumpTutorialTriggerEnter(player);
                break;

            default:

            break;
        }

        gameObject.SetActive(false);
    }
}