// DataManager.cs
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public Dictionary<int, ItemData> ItemDataBase { get; private set; } = new Dictionary<int, ItemData>();
    public Dictionary<int, DialogueData> DialogueDataBase { get; private set; } = new Dictionary<int, DialogueData>();

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
        LoadItemData();
        LoadDialogueData();
    }

    private void LoadItemData()
    {
        TextAsset jsondata = Resources.Load<TextAsset>("data/ItemData");
        ItemCollection itemCollection = JsonUtility.FromJson<ItemCollection>(jsondata.text);
        foreach (var item in itemCollection.items)
        {
            if (!string.IsNullOrEmpty(item.icon))
            {
                item.iconSprite = Resources.Load<Sprite>(item.icon);
            }
            ItemDataBase.Add(item.id, item);
        }
        Debug.Log($"아이템 {ItemDataBase.Count}개 로드 완료.");
    }

    private void LoadDialogueData()
    {
        TextAsset jsondata = Resources.Load<TextAsset>("data/DialogueData");
        DialogueCollection dialogueCollection = JsonUtility.FromJson<DialogueCollection>(jsondata.text);

        foreach (var dialogue in dialogueCollection.dialogues)
        {
            if (!string.IsNullOrEmpty(dialogue.basicSprite))
            {
                dialogue.basicSpriteLoaded = Resources.Load<Sprite>(dialogue.basicSprite);
            }
            if (!string.IsNullOrEmpty(dialogue.acceptSprite))
            {
                dialogue.acceptSpriteLoaded = Resources.Load<Sprite>(dialogue.acceptSprite);
            }
            if (!string.IsNullOrEmpty(dialogue.rejectSprite))
            {
                dialogue.rejectSpriteLoaded = Resources.Load<Sprite>(dialogue.rejectSprite);
            }
            DialogueDataBase.Add(dialogue.id, dialogue);
        }
        Debug.Log($"대화 {DialogueDataBase.Count}개 로드 완료.");
    }
    
    public ItemData GetItem(int id)
    {
        return ItemDataBase.ContainsKey(id) ? ItemDataBase[id] : null;
    }
    
    public DialogueData GetDialogue(int id)
    {
        return DialogueDataBase.ContainsKey(id) ? DialogueDataBase[id] : null;
    }
}