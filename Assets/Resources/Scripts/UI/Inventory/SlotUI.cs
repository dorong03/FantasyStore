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
    
    // 수정: int index 대신 ItemData 객체를 받도록 변경
    public void InitSlot(ItemData receivedItemData)
    {
        itemData = receivedItemData;

        if (itemData == null)
        {
            Debug.LogError("SlotUI: Null ItemData를 받았습니다.");
            gameObject.SetActive(false); // 슬롯 비활성화
            return;
        }

        int itemID;
        // 수정: string ID를 int로 안전하게 파싱
        if (!int.TryParse(itemData.ItemID, out itemID))
        {
            Debug.LogError($"SlotUI: 잘못된 ItemID 형식입니다: {itemData.ItemID}");
            gameObject.SetActive(false);
            return;
        }

        int itemCount = InventoryManager.Instance.GetItemCount(itemID);
        
        if (itemCount > 0)
        {
            // 아이템이 있으므로 슬롯 활성화
            image.sprite = Resources.Load<Sprite>($"Image/Items/{itemData.ItemID}");
            amountText.text = $"x{itemCount}"; // 수정: inventory에서 개수 가져오기
            image.color = Color.white; // 보이도록 색상 복원
            button.interactable = true; // 버튼 활성화
        }
        else
        {
            // 아이템이 없으므로 슬롯 비활성화
            image.sprite = null;
            image.color = new Color(0, 0, 0, 0); // 투명하게
            amountText.text = "";
            button.interactable = false; // 버튼 비활성화
        }
    }

    public void onClickSlotUI()
    {
        // 아이템이 없거나(interactable=false) 데이터가 없으면 실행 안 됨
        if (itemData == null) return; 

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
        if (inventoryUI != null)
        {
            inventoryUI.InitReceipt(itemData);
        }
    }
}