using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float speed = 5;

    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
    }
}
