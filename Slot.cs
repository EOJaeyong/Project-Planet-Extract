using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item;
    public int itemCount;
    public Image itemImage;

    [SerializeField]
    private TextMeshProUGUI text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    // 이 슬롯의 인덱스 (InventoryUI 또는 ChestPanelUI에서 할당)
    public int slotIndex;

    void Awake()
    {
        if (itemImage == null)
        {
            itemImage = GetComponent<Image>();
            if (itemImage == null)
                Debug.LogError("Slot: itemImage is not assigned. Please add an Image component.");
        }
        if (text_Count == null)
            Debug.LogWarning("Slot: text_Count is not assigned.");
        if (go_CountImage == null)
            Debug.LogWarning("Slot: go_CountImage is not assigned.");
    }

    private void SetColor(float alpha)
    {
        Color color = itemImage.color;
        color.a = alpha;
        itemImage.color = color;
    }

    public void AddItem(Item _item, int _count = 1)
    {
        if (_item == null)
        {
            Debug.LogError("Slot.AddItem: _item is null.");
            return;
        }
        item = _item;
        itemCount = _count;
        if (itemImage != null)
        {
            if (item.itemImage != null)
                itemImage.sprite = item.itemImage;
            else
                Debug.LogWarning("Slot.AddItem: item.itemImage is null for item: " + item.itemName);
        }
        if (item.itemType != Item.ItemType.Equipment && item.itemType != Item.ItemType.Blueprint && item.itemType != Item.ItemType.ResearchLog)
        {
            if (go_CountImage != null)
                go_CountImage.SetActive(true);
            if (text_Count != null)
                text_Count.text = itemCount.ToString();
        }
        else
        {
            if (go_CountImage != null)
                go_CountImage.SetActive(false);
            if (text_Count != null)
                text_Count.text = "0";
        }
        SetColor(1);
    }

    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        if (text_Count != null)
            text_Count.text = itemCount.ToString();
        if (itemCount <= 0)
            ClearSlot();
    }

    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        if (itemImage != null)
        {
            itemImage.sprite = null;
            SetColor(0);
        }
        if (text_Count != null)
            text_Count.text = "0";
        if (go_CountImage != null)
            go_CountImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            // 우클릭 로직 (예: 장착, 소비 등)을 여기에 구현
        }
    }

    public void SetSelected(bool selected)
    {
        Image bgImage = GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = selected ? Color.white : Color.gray;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
            ChangeSlot();
    }

    // 현재 슬롯이 ChestPanelUI(캐비넷 UI)에 속하는지 판단
    private bool IsCabinetSlot()
    {
        return GetComponentInParent<ChestPanelUI>() != null;
    }

    // 드래그한 슬롯(source)이 ChestPanelUI에 속하는지 판단
    private bool IsSourceCabinetSlot(Slot source)
    {
        return source.GetComponentInParent<ChestPanelUI>() != null;
    }

    /// <summary>
    /// 슬롯 간 아이템 이동 처리
    /// - 인벤토리 → 캐비넷: 지정한 슬롯(slotIndex)에 아이템 배치 (합치지 않음)
    /// - 캐비넷 → 인벤토리: 지정한 슬롯(slotIndex)에 아이템 배치 (스택 가능한 아이템은 합침)
    /// - 같은 컨테이너 내 이동: 단순 이동(스왑)
    /// </summary>
    private void ChangeSlot()
    {
        Slot sourceSlot = DragSlot.instance.dragSlot;
        if (sourceSlot == null || sourceSlot.item == null)
            return;

        bool destinationIsCabinet = IsCabinetSlot();
        bool sourceIsCabinet = IsSourceCabinetSlot(sourceSlot);

        // 인벤토리 → 캐비넷 이동
        if (!sourceIsCabinet && destinationIsCabinet)
        {
            ChestPanelUI chestUI = GetComponentInParent<ChestPanelUI>();
            if (chestUI != null)
            {
                chestUI.AddItemToChest(sourceSlot.item, sourceSlot.itemCount, this.slotIndex);
                InventoryManager.Instance.RemoveItem(sourceSlot.item, sourceSlot.itemCount);
                sourceSlot.ClearSlot();
                chestUI.RefreshUI();
                return;
            }
        }

        // 캐비넷 → 인벤토리 이동
        if (sourceIsCabinet && !destinationIsCabinet)
        {
            if (!InventoryManager.Instance.CanAddItem(sourceSlot.item))
            {
                Debug.Log("Inventory is full. Transfer canceled.");
                return;
            }
            InventoryManager.Instance.InsertItemAt(this.slotIndex, sourceSlot.item, sourceSlot.itemCount);
            ChestManager.Instance.RemoveItem(sourceSlot.item, sourceSlot.itemCount);
            sourceSlot.ClearSlot();
            ChestPanelUI chestUI = sourceSlot.GetComponentInParent<ChestPanelUI>();
            if (chestUI != null)
                chestUI.RefreshUI();
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null)
                inventoryUI.RefreshUI();
            return;
        }

        // 같은 컨테이너 내 이동 (단순 이동: 스왑)
        if (sourceSlot.item != null && item == null)
        {
            AddItem(sourceSlot.item, sourceSlot.itemCount);
            sourceSlot.ClearSlot();
        }
        else if (sourceSlot.item != null && item != null && item.itemName == sourceSlot.item.itemName)
        {
            AddItem(sourceSlot.item, sourceSlot.itemCount);
            sourceSlot.ClearSlot();
        }
        else
        {
            Debug.Log("Different item types cannot be transferred.");
        }
    }
}
