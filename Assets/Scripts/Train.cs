using System;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] private TrainActivator activator;

    [Header("Speed")]
    [SerializeField] private float speed = 5.0f;
    private Vector3 inittialPos;

    private void Awake()
    {
        activator.OnActivateTrain += OnActivateTrain;

        inittialPos = transform.position;

        enabled = false;
    }

    private void OnActivateTrain(bool obj)
    {
        enabled = obj;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, 0.0f, 0.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position = inittialPos;

        enabled = false;
    }

    private void OnDestroy()
    {
        activator.OnActivateTrain -= OnActivateTrain;
    }
}
