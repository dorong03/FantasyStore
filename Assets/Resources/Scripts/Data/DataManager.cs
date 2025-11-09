using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public Dictionary<int, ItemData> ItemDataBase { get; private set; } = new Dictionary<int, ItemData>();
    public Dictionary<int, DialogueData> DialogueDataBase { get; private set; } = new Dictionary<int, DialogueData>();
    public Dictionary<int, EventData> EventDataBase { get; private set; } = new Dictionary<int, EventData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAll();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAll()
    {
        LoadItems();
        LoadDialogues();
        LoadEvents();
    }

    private void LoadItems()
    {
        TextAsset json = Resources.Load<TextAsset>("data/items");
        if (json == null)
        {
            Debug.LogWarning("data/items.json 을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<ItemDataWrapper>(json.text);
        ItemDataBase.Clear();
        foreach (var item in wrapper.items)
        {
            ItemDataBase[item.id] = item;
        }
    }

    private void LoadDialogues()
    {
        TextAsset json = Resources.Load<TextAsset>("data/dialogues");
        if (json == null)
        {
            Debug.LogWarning("data/dialogues.json 을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<DialogueDataWrapper>(json.text);
        DialogueDataBase.Clear();
        foreach (var d in wrapper.dialogues)
        {
            // 가격이 0인데 아이템이 존재하면 아이템 기본가로 채워주기
            if (d.itemPrice == 0 && ItemDataBase.TryGetValue(d.itemId, out var item))
            {
                d.itemPrice = item.basePrice;
            }

            DialogueDataBase[d.id] = d;
        }
    }

    private void LoadEvents()
    {
        TextAsset json = Resources.Load<TextAsset>("data/events");
        if (json == null)
        {
            Debug.LogWarning("data/events.json 을 찾을 수 없습니다.");
            return;
        }

        var wrapper = JsonUtility.FromJson<EventDataWrapper>(json.text);
        EventDataBase.Clear();
        foreach (var e in wrapper.events)
        {
            EventDataBase[e.id] = e;
        }
    }

    // 편의 메서드
    public ItemData GetItem(int id)
    {
        ItemDataBase.TryGetValue(id, out var item);
        return item;
    }

    public DialogueData GetDialogue(int id)
    {
        DialogueDataBase.TryGetValue(id, out var d);
        return d;
    }

    public EventData GetEvent(int id)
    {
        EventDataBase.TryGetValue(id, out var e);
        return e;
    }
}
