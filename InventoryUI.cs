
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private GameObject go_SlotsParent; // 슬롯들이 포함된 부모 오브젝트

    private Slot[] slots;
    private int selectedSlotIndex = -1;

    void OnEnable()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();

        // 덮어쓰기 방지: 기존 데이터가 없거나 크기가 다를 때만 초기화
        if (InventoryManager.Instance.items == null || InventoryManager.Instance.items.Length != slots.Length)
        {
            InventoryManager.Instance.maxCapacity = slots.Length;
            InventoryManager.Instance.items = new InventoryManager.InventoryItem[slots.Length];
            for (int i = 0; i < slots.Length; i++)
                InventoryManager.Instance.items[i] = null;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotIndex = i;
        }

        RefreshUI();
    }

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedSlotIndex = i;
                HighlightSelectedSlot();
            }
        }
    }

    private void HighlightSelectedSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSelected(i == selectedSlotIndex);
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
        for (int i = 0; i < InventoryManager.Instance.maxCapacity && i < slots.Length; i++)
        {
            var invItem = InventoryManager.Instance.items[i];
            if (invItem != null && invItem.item != null)
            {
                slots[i].AddItem(invItem.item, invItem.count);
            }
        }
    }
}
