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
        // 💡 수정: DataManager에 있는 모든 아이템을 1개씩 지급
        if (DataManager.Instance != null && DataManager.Instance.ItemDictionary != null)
        {
            foreach (string itemIDString in DataManager.Instance.ItemDictionary.Keys)
            {
                if (int.TryParse(itemIDString, out int itemID))
                {
                    AddItem(itemID, 1); // 모든 아이템을 1개씩 추가
                }
            }
        }
        else
        {
            Debug.LogError("InventoryManager: DataManager 또는 ItemDictionary를 찾을 수 없어 아이템을 지급할 수 없습니다.");
        }
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
        // DataManager의 GetItemData는 string을 받으므로 변환
        if (DataManager.Instance.GetItemData(itemID.ToString()) == null)
        {
            Debug.LogWarning($"InventoryManager: 존재하지 않는 아이템 ID({itemID})를 추가하려 했습니다.");
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