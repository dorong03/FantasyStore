using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public ItemData itemData;
    public Text amountText;
    public Image image;
    public Button button;

    void Awake()
    {
        amountText = GetComponentInChildren<Text>();
        if (image == null)
        {
            image = GetComponent<Image>();    
        }
        button = GetComponent<Button>();
        button.onClick.AddListener(onClickSlotUI);
    }

    public void InitSlot(ItemData receivedItemData)
    {
        itemData = receivedItemData;

        if (itemData == null)
        {
            Debug.LogError("SlotUI: Null ItemData를 받았습니다.");
            gameObject.SetActive(false);
            return;
        }

        int itemID;
        if (!int.TryParse(itemData.ItemID, out itemID))
        {
            Debug.LogError($"SlotUI: 잘못된 ItemID 형식입니다: {itemData.ItemID}");
            gameObject.SetActive(false);
            return;
        }
        
        image.sprite = Resources.Load<Sprite>($"Image/Items/{itemData.ItemID}");
        
        int itemCount = InventoryManager.Instance.GetItemCount(itemID);
        
        amountText.text = $"x{itemCount}";
        
        if (itemCount > 0)
        {
            image.color = Color.white;
            button.interactable = true;
        }
        else
        {
            image.color = Color.gray;
            button.interactable = false;
        }
    }

    public void onClickSlotUI()
    {
        if (itemData == null) return; 

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.InitReceipt(itemData);
        }
    }
}