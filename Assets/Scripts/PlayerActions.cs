using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerActions : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Speed")]
    [SerializeField] private int speed = 10;
    private float currentSpeed;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        currentSpeed = speed;
    }

    void Update()
    {
        transform.Translate(new Vector3(1 * currentSpeed * Time.deltaTime, 0, 0));
    }
}