using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public Dictionary<string, ItemData> ItemDictionary { get; private set; } = new Dictionary<string, ItemData>();
    public Dictionary<string, EventData> EventDictionary { get; private set; } = new Dictionary<string, EventData>();
    public Dictionary<string, TextData> TextDictionary { get; private set; } = new Dictionary<string, TextData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllGameData()
    {
        Debug.Log("DataManager: 게임 데이터 로드를 시작합니다.");
        
        LoadItemData("Data/Items");
        LoadEventData("Data/Events");
        LoadTextData("Data/Texts");

        Debug.Log($"DataManager: 로드 완료. 아이템:{ItemDictionary.Count}, 이벤트:{EventDictionary.Count}, 텍스트:{TextDictionary.Count}");
    }

    private void LoadItemData(string path)
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(path);
        if (jsonTextAsset == null) { Debug.LogError($"DataManager Error: 경로 '{path}'에서 JSON 파일을 찾을 수 없습니다."); return; }

        ItemList listWrapper = JsonUtility.FromJson<ItemList>(jsonTextAsset.text);
        if (listWrapper == null || listWrapper.items == null) { Debug.LogError($"DataManager Error: '{path}' 파일의 JSON 파싱에 실패했습니다."); return; }

        ItemDictionary.Clear();
        foreach (var item in listWrapper.items)
        {
            if (!string.IsNullOrEmpty(item.ItemID) && !ItemDictionary.ContainsKey(item.ItemID))
            {
                ItemDictionary.Add(item.ItemID, item);
            }
        }
    }

    private void LoadEventData(string path)
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(path);
        if (jsonTextAsset == null) { Debug.LogError($"DataManager Error: 경로 '{path}'에서 JSON 파일을 찾을 수 없습니다."); return; }

        EventList listWrapper = JsonUtility.FromJson<EventList>(jsonTextAsset.text);
        if (listWrapper == null || listWrapper.events == null) { Debug.LogError($"DataManager Error: '{path}' 파일의 JSON 파싱에 실패했습니다."); return; }

        EventDictionary.Clear();
        foreach (var eventData in listWrapper.events)
        {
            if (!string.IsNullOrEmpty(eventData.EventID) && !EventDictionary.ContainsKey(eventData.EventID))
            {
                EventDictionary.Add(eventData.EventID, eventData);
            }
        }
    }

    private void LoadTextData(string path)
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(path);
        if (jsonTextAsset == null) { Debug.LogError($"DataManager Error: 경로 '{path}'에서 JSON 파일을 찾을 수 없습니다."); return; }

        TextList listWrapper = JsonUtility.FromJson<TextList>(jsonTextAsset.text);
        if (listWrapper == null || listWrapper.texts == null) { Debug.LogError($"DataManager Error: '{path}' 파일의 JSON 파싱에 실패했습니다."); return; }

        TextDictionary.Clear();
        foreach (var textData in listWrapper.texts)
        {
            if (!string.IsNullOrEmpty(textData.TextID) && !TextDictionary.ContainsKey(textData.TextID))
            {
                TextDictionary.Add(textData.TextID, textData);
            }
        }
    }
    
    public ItemData GetItemData(string itemID)
    {
        if (ItemDictionary.TryGetValue(itemID, out ItemData data))
        {
            return data;
        }
        Debug.LogWarning($"ItemData: ItemID '{itemID}'를 찾을 수 없습니다.");
        return null;
    }
    
    public TextData GetTextData(string textID)
    {
        if (TextDictionary.TryGetValue(textID, out TextData data))
        {
            return data;
        }
        Debug.LogWarning($"TextData: TextID '{textID}'를 찾을 수 없습니다.");
        return null;
    }
}