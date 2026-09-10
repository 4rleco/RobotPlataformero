using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private PlayerActions player;

    private float initialPosX;

    private void Start()
    {
        initialPosX = transform.position.x;
    }

    private void Update()
    {
        if (player == null) return;

            transform.position = new Vector3(transform.position.x + (player.GetCurrentSpeed() * Time.deltaTime), transform.position.y, transform.position.z);

        if (player.GetPlayerDied())
            transform.position = new Vector3(initialPosX, transform.position.y, transform.position.z);
    }
}
