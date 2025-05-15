using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    [SerializeField] private GameObject go_InventoryBase;
    [SerializeField] private GameObject go_SlotsParent;
    private Slot[] slots;
    private int selectedSlotIndex = 0;

    [SerializeField] private float dropDistance = 1f;
    [SerializeField] private float dropHeight = 0.5f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float throwForceForward = 5f;
    [SerializeField] private float throwForceUpward = 3f;

    public Slot[] GetSlots() { return slots; }
    public int SelectedIndex { get { return selectedSlotIndex; } }

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        if (playerTransform == null)
            playerTransform = transform;
        RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlotIndex = 0;
            HighlightSelectedSlot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotIndex = 1;
            HighlightSelectedSlot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotIndex = 2;
            HighlightSelectedSlot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlotIndex = 3;
            HighlightSelectedSlot();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("버리기 상호작용 ");

            if (selectedSlotIndex >= 0 && selectedSlotIndex < slots.Length)
            {
                Slot selectedSlot = slots[selectedSlotIndex];
                if (selectedSlot.item != null)
                {
                    Vector3 dropPos = playerTransform.position + playerTransform.forward * dropDistance;
                    dropPos.y = playerTransform.position.y + dropHeight;
                    if (selectedSlot.item.dropPrefab != null)
                    {
                        GameObject droppedItem = Instantiate(selectedSlot.item.dropPrefab, dropPos, Quaternion.identity);
                        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            Vector3 throwDirection = cameraTransform.forward;
                            Vector3 throwForce = throwDirection * throwForceForward + Vector3.up * throwForceUpward;
                            rb.AddForce(throwForce, ForceMode.Impulse);
                        }
                    }
                    if (selectedSlot.itemCount > 1)
                    {
                        // UI 슬롯 카운트만 차감
                        selectedSlot.SetSlotCount(-1);
                        // 선택한 슬롯에서만 1개 제거
                        InventoryManager.Instance.RemoveItemAt(selectedSlotIndex, 1);
                    }
                    else
                    {
                        // 선택한 슬롯에서 1개 제거 후 슬롯 비우기
                        InventoryManager.Instance.RemoveItemAt(selectedSlotIndex, 1);
                        selectedSlot.ClearSlot();
                    }
                    RefreshUI();
                }
            }
        }
    }

    private void HighlightSelectedSlot()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSelected(i == selectedSlotIndex);
    }

    public void RefreshUI()
    {
        foreach (Slot slot in slots)
            slot.ClearSlot();

        for (int i = 0; i < InventoryManager.Instance.maxCapacity && i < slots.Length; i++)
        {
            var invItem = InventoryManager.Instance.items[i];
            if (invItem != null && invItem.item != null)
                slots[i].AddItem(invItem.item, invItem.count);
        }
    }

    public bool AcquireItem(Item _item, int _count = 1)
    {
        if (_item == null)
        {
            Debug.LogError("AcquireItem: _item is null.");
            return false;
        }
        if (!InventoryManager.Instance.CanAddItem(_item))
        {
            Debug.Log("Inventory is full: " + _item.itemName);
            return false;
        }
        bool success = InventoryManager.Instance.AddItem(_item, _count);
        if (success)
            RefreshUI();
        return success;
    }
}
