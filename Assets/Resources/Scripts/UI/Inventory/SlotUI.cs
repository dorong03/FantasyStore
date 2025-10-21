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
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(onClickSlotUI);
    }
    
    public void InitSlot(int index)
    {
        itemData = DataManager.Instance.GetItem(index);
        int itemCount = InventoryManager.Instance.GetItemCount(index);
        if (itemCount > 0)
        {
            image.sprite = itemData.iconSprite;
            amountText.text = $"x{InventoryManager.Instance.GetItemCount(index) }";
        }
        else
        {
            Debug.Log("아이템이 없엉 ㅠㅠ");
        }
    }

    public void onClickSlotUI()
    {
        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
        inventoryUI.InitReceipt(itemData);
    }
}
