using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string ItemID;

    public string ItemName;
    public string ItemType;
    public int BasePrice;
    public string Decription; 
    
    public string SellText1;
    public string SellText2;
    public string SellText3;
    public string BuyText1;
    public string BuyText2;
    public string BuyText3;
}
[Serializable]
public class ItemList
{
    public ItemData[] items;
}


[Serializable]
public class EventData
{
    public string EventID;
    public string ItemType;
    public string Text;
    public int PriceFluctuation;
}

[Serializable]
public class EventList
{
    public EventData[] events;
}

[Serializable]
public class TextData
{
    public string TextID;
    
    public string Customer;
    public int ItemID;
    public string Context;
    public string Explanation;
    public string Choice;
    public string AcceptText;
    public string RejectText;
}

[Serializable]
public class TextList
{
    public TextData[] texts;
}