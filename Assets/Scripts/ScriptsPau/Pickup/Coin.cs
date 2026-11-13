using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private int moneyQuantity = 1;

    public void SetMoneyQuantity(int quantity)
    {
        moneyQuantity = quantity;
    }

    private void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerInventory>().AddMoney(moneyQuantity);
        }
    }
}
