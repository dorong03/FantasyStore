using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotUIPrefab;
    
    public PanelHoverDetector panelHoverDetector;

    public ItemData itemData;
    public Text itemName;
    public Text itemDescription;
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
            for (int i = 0; i < DataManager.Instance.ItemDataBase.Count; i++)
            {
                GameObject slotUI = Instantiate(slotUIPrefab, transform);
                SlotUI slot = slotUI.GetComponent<SlotUI>();
                if (slot != null)
                {
                    slot.InitSlot(i);
                }
                else
                {
                    Debug.LogError("slotUI 컴포넌트가 음서");
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
        if (this.itemData == null)
        {
            Debug.LogError("잘못된 아이템 데이터가 전달되었습니다.");
            return;
        }
        
        itemName.text = this.itemData.name;
        itemDescription.text = this.itemData.description;
        itemImage.sprite = this.itemData.iconSprite;
        itemPrice.text = this.itemData.basePrice.ToString();
        totalPrice.text = this.itemData.basePrice.ToString();
        itemAmountText.text = "x1"; 
    
        receiptPrice = this.itemData.basePrice;
        receiptTotalPrice = this.itemData.basePrice;
        receiptAmount = 1;
    }

    public void AddRecepitItem()
    {
        if (receiptAmount + 1 <= InventoryManager.Instance.GetItemCount(itemData.id))
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

        if (itemData == null)
        {
            return;
        }

        if (!DialogueManager.Instance.canBuy)
        {
            return;
        }
        
        var dialogue = DialogueManager.Instance.dialogue;
        if (dialogue != null && dialogue.selling)
        {
            bool isSaleSuccessful = dialogue.itemId == itemData.id &&
                                    dialogue.itemAmount == receiptAmount &&
                                    (dialogue.itemPrice * 1.5f) > receiptTotalPrice;

            if (isSaleSuccessful)
            {
                InventoryManager.Instance.RemoveItem(itemData.id, receiptAmount);
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