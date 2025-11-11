using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject textPanel;
    [SerializeField] private Text dialogueText;
    
    [SerializeField] private Image visitorImage;
    
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button cancelButton;
    public bool canBuy = false;

    public TextData dialogue;
    public int itemAmount; // NPC 요구 수량
    
    [SerializeField] private NpcController npcController;
    [SerializeField] private InventoryUI inventoryUI;

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

        // 버튼 리스너 연결
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(OnClickTradeOrPurchase); 
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnReject);
        }
    }

    public void StartDialogueCoroutine(int i)
    {
        StartCoroutine(playDialogueWithID(i));
    }
    
    public IEnumerator playDialogueWithID(int dialogueID)
    {
        canBuy = false;
        purchaseButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        
        dialogue = DataManager.Instance.GetTextData(dialogueID.ToString());
        
        // --- NPC 요구 수량 1개로 고정 ---
        itemAmount = 1; 

        // 이미지 로딩
        if (Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Basic") != null)
        {
            visitorImage.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Basic");
        }

        InitChatBubble();
        yield return StartCoroutine(npcController.Apear(dialogue));
        
        canBuy = true;
        
        // NPC 기준 대사 출력 로직
        string npcMessage = "";
        ItemData currentItem = DataManager.Instance.GetItemData(dialogue.ItemID.ToString());

        if (dialogue.Context.Contains("sell")) // NPC 판매 (플레이어 구매)
        {
            int rand = Random.Range(1, 4);
            if (rand == 1) npcMessage = currentItem.SellText1;
            else if (rand == 2) npcMessage = currentItem.SellText2;
            else npcMessage = currentItem.SellText3;
        }
        else // "buy" (NPC 구매, 플레이어 판매)
        {
            int rand = Random.Range(1, 4); 
            if (rand == 1) npcMessage = currentItem.BuyText1;
            else if (rand == 2) npcMessage = currentItem.BuyText2;
            else npcMessage = currentItem.BuyText3;
        }
        
        // 치환: 아이템 이름(%s), 기본 가격(%d), 수량(%a)
        string formattedMessage = npcMessage.Replace("%s", currentItem.ItemName);
        formattedMessage = formattedMessage.Replace("%d", currentItem.BasePrice.ToString());
        formattedMessage = formattedMessage.Replace("%a", itemAmount.ToString());
        
        ChatBubble(formattedMessage);

        SetChoiceButtons(dialogue.Choice);
        purchaseButton.gameObject.SetActive(true);
    }
    
    private void ChatBubble(string msg)
    {
        textPanel.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = msg;
    }

    private void InitChatBubble()
    {
        textPanel.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        dialogueText.text = "";
    }
    
    // 💡 --- 수정된 메서드 --- 💡
    // Choice 텍스트를 버튼에 적용 (줄 바꿈 처리)
    private void SetChoiceButtons(string choiceText)
    {
        string[] choices = choiceText.Split('|');
        if (choices.Length >= 2)
        {
            // 💡 수정: \n (JSON에서 \\n)을 실제 줄바꿈(\n)으로 변환
            string acceptText = choices[0].Trim().Replace("\\n", "\n");
            string rejectText = choices[1].Trim().Replace("\\n", "\n");

            // 첫 번째 텍스트: 수락/거래 버튼
            purchaseButton.GetComponentInChildren<Text>().text = acceptText;
            // 두 번째 텍스트: 거절/취소 버튼
            cancelButton.GetComponentInChildren<Text>().text = rejectText;
        }
        else // 데이터 오류 방지용 기본 설정
        {
            purchaseButton.GetComponentInChildren<Text>().text = "거래"; 
            cancelButton.GetComponentInChildren<Text>().text = "취소"; 
        }
    }

    // NPC 기준 거래/구매 버튼 클릭 시 역할 분기
    public void OnClickTradeOrPurchase()
    {
        if (dialogue == null) return; 
        
        if (dialogue.Context.Contains("sell"))
        {
            // [NPC Sell] -> [Player Buy]: 즉시 거래 로직 실행
            StartCoroutine(AcceptSequence());
        }
        else if (dialogue.Context.Contains("buy"))
        {
            // [NPC Buy] -> [Player Sell]: InventoryUI 열고 아이템 자동 세팅
            if (inventoryUI != null)
            {
                inventoryUI.gameObject.SetActive(true);
                inventoryUI.PrepareForTrade(dialogue.ItemID.ToString(), itemAmount);
            }
        }
    }

    private IEnumerator RejectSequence()
    {
        purchaseButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        
        if (Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Reject") != null)
        {
            visitorImage.sprite = Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Reject");
        }
        ChatBubble(dialogue.RejectText);
        
        yield return new WaitForSeconds(2f);
        InitChatBubble();
        yield return StartCoroutine(npcController.Disapear(dialogue));
        
        dialogue = null;
        GameManager.Instance.OnNpcEncounterFinished();
    }

    private IEnumerator AcceptSequence()
    {
        purchaseButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        
        // [NPC Sell] -> [Player Buy] 로직
        if (dialogue.Context.Contains("sell"))
        {
            // NPC 판매 시 플레이어는 이벤트가 적용되지 않은 BasePrice로 구매
            int price = DataManager.Instance.GetItemData(dialogue.ItemID.ToString()).BasePrice * itemAmount;
            
            if (GameManager.Instance.CheckGold(price))
            {
                // 구매 성공
                GameManager.Instance.RemoveGold(price);
                InventoryManager.Instance.AddItem(dialogue.ItemID, itemAmount);

                if (Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Accept") != null)
                {
                    visitorImage.sprite = Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Accept");
                }
                ChatBubble(dialogue.AcceptText);
            }
            else
            {
                // 돈 부족으로 구매 실패
                StartCoroutine(RejectSequence());
                yield break; 
            }
        }
        // [NPC Buy] -> [Player Sell] 로직 (InventoryUI에서 호출됨)
        else
        {
            if (Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Accept") != null)
            {
                visitorImage.sprite = Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Accept");
            }
            ChatBubble(dialogue.AcceptText); 
        }
        
        // 성공 시 퇴장 로직
        yield return new WaitForSeconds(2f);
        InitChatBubble();
        yield return StartCoroutine(npcController.Disapear(dialogue));
        
        if (dialogue != null) 
        {
            dialogue = null;
            GameManager.Instance.OnNpcEncounterFinished();
        }
    }
    
    public void OnReject()
    {
        if (dialogue != null)
        {
            StartCoroutine(RejectSequence());
        }
    }
    
    public void OnAccept()
    {
        if (dialogue != null)
        {
            StartCoroutine(AcceptSequence());
        }
    }
}