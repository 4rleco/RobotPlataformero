using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private PlayerActions player;

    private void Update()
    {
       if (player == null) return;

        // Mantiene el seguimiento directo en X
        transform.position = new Vector3(player.transform.position.x , transform.position.y, -10);
    }
}
