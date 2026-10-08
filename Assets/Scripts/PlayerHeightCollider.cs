using UnityEngine;

public class PlayerHeightCollider : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float fixedY = -15f;

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(target.position.x, fixedY, 0f);
    }
}