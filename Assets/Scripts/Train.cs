using System;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] private TrainActivator activator;

    [Header("Speed")]
    [SerializeField] private float speed = 5.0f;

    private SpriteRenderer body;

    private void Awake()
    {
        activator.OnActivateTrain += OnActivateTrain;

        body = GetComponentInChildren<SpriteRenderer>();
        enabled = false;
    }

    private void OnActivateTrain(bool obj)
    {
        enabled = obj;
    }

    private void Update()
    {
        body.transform.position = new Vector3(body.transform.position.x - speed * Time.deltaTime, 0.0f, 0.0f);
    }

    private void OnDestroy()
    {
        activator.OnActivateTrain -= OnActivateTrain;
    }
}
