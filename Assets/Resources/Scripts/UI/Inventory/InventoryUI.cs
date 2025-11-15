using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotUIPrefab;
    
    public PanelHoverDetector panelHoverDetector;

    public ItemData itemData;
    public Text itemName;
    public Text itemDescription;
    public Text itemType;
    public Image itemImage;
    public Text itemPrice;
    public Text totalPrice;
    public Text itemAmountText;

    public int receiptAmount;
    public int receiptPrice;
    public int receiptTotalPrice;
    public Sprite basicIconSprite;

    public GameObject posPanel;
    
    void Update()
    {
        if (panelHoverDetector != null && panelHoverDetector.isMouseOverPanel)
        {
            if (itemData == null)
            {
                return;
            }
            
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0f)
            {
                receiptPrice += 100;
                UpdateReceipt();
            }
            else if (scroll < 0f)
            {
                receiptPrice -= 100;
                if (receiptPrice < 0)
                {
                    receiptPrice = 0;
                }
                UpdateReceipt();
            }
        }
    }
    
    void OnEnable()
    {
        ClearInventorySlots();
        InitInventoryUI();
        InventoryManager.Instance.PrintAllItems();
    }

    private void OnDisable()
    {
        ClearReceipt();
    }

    public void InitInventoryUI()
    {
        if (slotUIPrefab != null)
        {
            foreach (ItemData data in DataManager.Instance.ItemDictionary.Values)
            {
                GameObject slotUI = Instantiate(slotUIPrefab, transform);
                SlotUI slot = slotUI.GetComponent<SlotUI>();
                if (slot != null)
                {
                    slot.InitSlot(data);
                }
            }
        }
    }
    
    public void ClearReceipt()
    {
        itemData = null;
        receiptAmount = 0;
        receiptPrice = 0;
        receiptTotalPrice = 0;
        itemType.text = "";
        itemName.text = "";
        itemDescription.text = "";
        itemPrice.text = "0";
        totalPrice.text = "0";
        itemAmountText.text = "";

        if (itemImage != null)
        {
            itemImage.sprite = basicIconSprite;
        }
    }
    
    public void ClearInventorySlots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void InitReceipt(ItemData receivedItemData)
    {
        this.itemData = receivedItemData;
        if (this.itemData == null) return;
        
        itemName.text = this.itemData.ItemName;
        itemDescription.text = this.itemData.Decription;
        itemImage.sprite = Resources.Load<Sprite>($"Image/Items/{itemData.ItemID}");
        itemPrice.text = GetAdjustedBasePrice(itemData.ItemID).ToString();
        itemType.text = $"계열 : {this.itemData.ItemType}";
        totalPrice.text = GetAdjustedBasePrice(itemData.ItemID).ToString();
        itemAmountText.text = "x1"; 
    
        receiptPrice = GetAdjustedBasePrice(itemData.ItemID);
        receiptTotalPrice = receiptPrice;
        receiptAmount = 1;
    }
    
    // 이벤트 적용된 기준 가격을 가져오는 헬퍼 메서드
    public int GetAdjustedBasePrice(string itemID)
    {
        ItemData itemData = DataManager.Instance.GetItemData(itemID);
        if (itemData == null) return 0;
        
        int basePrice = itemData.BasePrice;
        EventData currentEvent = GameManager.Instance.CurrentEventData;
        
        // 이벤트가 활성화되어 있고, 아이템 유형이 일치하는 경우에만 가격 변동 적용
        if (currentEvent != null && currentEvent.ItemType == itemData.ItemType)
        {
            float multiplier = 1f + (currentEvent.PriceFluctuation / 100f); 
            // NPC에게 팔 때는 이벤트 가격이 적용됨 (BasePrice * (1 + 변동률))
            return Mathf.RoundToInt(basePrice * multiplier); 
        }
        
        return basePrice;
    }

    // NPC Buy (Player Sell) 상황을 위해 아이템을 Receipt에 자동 세팅
    public void PrepareForTrade(string itemID, int amount)
    {
        ItemData requestedItemData = DataManager.Instance.GetItemData(itemID);
        if (requestedItemData == null) return;

        // 이벤트 적용된 가격을 가져옴
        int adjustedBasePrice = GetAdjustedBasePrice(itemID);

        posPanel.SetActive(true);
        itemData = requestedItemData;
        itemName.text = itemData.ItemName;
        itemDescription.text = itemData.Decription;
        itemImage.sprite = Resources.Load<Sprite>($"Image/Items/{itemData.ItemID}");
        itemType.text = $"계열 : {this.itemData.ItemType}";
        receiptAmount = amount;
        receiptPrice = itemData.BasePrice;
        //receiptPrice = adjustedBasePrice; 
        UpdateReceipt();
    }


    public void AddRecepitItem()
    {
        if (itemData == null) return;
        if (receiptAmount + 1 <= InventoryManager.Instance.GetItemCount(int.Parse(itemData.ItemID)))
        {
            receiptAmount++;
            UpdateReceipt();
        }
    }

    public void RemoveRecepitItem()
    {
        if (receiptAmount >= 2)
        {
            receiptAmount--;
            UpdateReceipt();
        }
    }

    public void UpdateReceipt()
    {
        itemAmountText.text = $"x{receiptAmount}";
        receiptTotalPrice = receiptAmount * receiptPrice;
        totalPrice.text = receiptTotalPrice.ToString();
        itemPrice.text = receiptPrice.ToString();
    }

    public void OnClickTradeButton()
    {
        if (itemData == null) return;
        
        var dialogue = DialogueManager.Instance.dialogue;

        // [NPC Buy] -> [Player Sell] 로직
        if (dialogue != null && dialogue.Context.Contains("buy"))
        {
            int requiredItemID = dialogue.ItemID;
            int requiredAmount = DialogueManager.Instance.itemAmount;
            
            // 이벤트 적용된 기준 가격을 가져옴
            int adjustedBasePrice = GetAdjustedBasePrice(requiredItemID.ToString());
            
            bool isItemMatch = requiredItemID.ToString() == itemData.ItemID && requiredAmount == receiptAmount;
            
            // NPC는 이벤트 적용 가격(adjustedBasePrice)의 120%까지 받아들인다고 가정
            float acceptedMaxPrice = adjustedBasePrice * 1.2f; 
            bool isPriceAcceptable = acceptedMaxPrice >= receiptPrice; 
            
            bool hasEnoughItems = InventoryManager.Instance.GetItemCount(requiredItemID) >= receiptAmount;
            
            bool isSaleSuccessful = isItemMatch && isPriceAcceptable && hasEnoughItems;

            if (isSaleSuccessful)
            {
                InventoryManager.Instance.RemoveItem(requiredItemID, receiptAmount);
                GameManager.Instance.AddGold(receiptTotalPrice);
                DialogueManager.Instance.OnAccept();
            }
            else
            {
                DialogueManager.Instance.OnReject();
            }
            
            posPanel.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}