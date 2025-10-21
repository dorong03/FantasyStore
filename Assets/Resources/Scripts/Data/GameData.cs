using System;
using UnityEngine;

[Serializable]
public class ItemCollection
{
    public ItemData[] items;
}

[Serializable]
public class ItemData
{
    public int id;
    public string name;
    public string icon;
    public int basePrice;
    public string description;

    [NonSerialized]
    public Sprite iconSprite;
}

[Serializable]
public class DialogueCollection
{
    public DialogueData[] dialogues;
}

[Serializable]
public class DialogueData
{
    public int id;
    public string dialogue;
    public bool selling;
    public int itemId;
    public int itemAmount;
    public int itemPrice;
    public string rejectDialogue;
    public string acceptDialogue;
    
    public string basicSprite;
    public string acceptSprite;
    public string rejectSprite;
    
    [NonSerialized] public Sprite basicSpriteLoaded;
    [NonSerialized] public Sprite acceptSpriteLoaded;
    [NonSerialized] public Sprite rejectSpriteLoaded;
}