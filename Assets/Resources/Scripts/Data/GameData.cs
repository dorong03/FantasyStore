using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public string name;
    public string type;
    public string icon;
    public int basePrice;
    public string description;
}

[Serializable]
public class ItemDataWrapper
{
    public ItemData[] items;
}

[Serializable]
public class DialogueData
{
    public int id;
    public string customer;
    public bool selling;
    public int itemId;
    public int itemAmount;
    public int itemPrice;
    public string explanation;
    public string choice1;
    public string choice2;
    public string acceptDialogue;
    public string rejectDialogue;

    public string basicSprite;
    public string acceptSprite;
    public string rejectSprite;

    // [NonSerialized] public Sprite basicSpriteLoaded;
    // [NonSerialized] public Sprite acceptSpriteLoaded;
    // [NonSerialized] public Sprite rejectSpriteLoaded;
}

[Serializable]
public class DialogueDataWrapper
{
    public DialogueData[] dialogues;
}

[Serializable]
public class EventData
{
    public int id;
    public string type;
    public string text;
    public float priceChangePercent;
}

[Serializable]
public class EventDataWrapper
{
    public EventData[] events;
}