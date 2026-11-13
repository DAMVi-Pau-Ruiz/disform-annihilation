using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private int money;

    public void AddMoney(int count)
    {
        money += count;
    }

    public void RemoveMoney(int count)
    {
        money -= count;

        if (money < 0)
        {
            money = 0;
        }
    }

    public int GetMoney()
    {
        return money;
    }
}
