using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Dictionary<int, int> inventory { get; private set; } = new Dictionary<int, int>();
    
    public static event Action OnInventoryChanged;

    void Start()
    {
        AddItem(0, 3);
        AddItem(1, 1);
        AddItem(2, 1);
        AddItem(3, 1);
        AddItem(4, 1);
        AddItem(5, 1);
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddItem(int itemID, int amount = 1)
    {
        if (DataManager.Instance.GetItem(itemID) == null)
        {
            return;
        }

        if (inventory.ContainsKey(itemID))
        {
            inventory[itemID] += amount;
        }
        else
        {
            inventory.Add(itemID, amount);
        }
        
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(int itemID, int amount = 1)
    {
        if (!inventory.ContainsKey(itemID))
        {
            Debug.Log("인벤토리에 없는 아이템");
            return;
        }

        if (inventory[itemID] >= amount)
        {
            inventory[itemID] -= amount;
        }

        if (inventory[itemID] <= 0)
        {
            inventory.Remove(itemID);
        }
        
        OnInventoryChanged?.Invoke();
    }

    public int GetItemCount(int itemID)
    {
        if (inventory.ContainsKey(itemID))
        {
            return inventory[itemID];
        }
        return 0;
    }
    
    public void PrintAllItems()
    {
        Debug.Log("--- 현재 인벤토리 목록 ---");
        if (inventory.Count == 0)
        {
            Debug.Log("인벤토리가 비어있습니다.");
            return;
        }

        foreach (KeyValuePair<int, int> item in inventory)
        {
            Debug.Log($"아이템 ID: {item.Key}, 개수: {item.Value}");
        }
    }
}
