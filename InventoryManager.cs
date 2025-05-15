using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public int maxCapacity = 4;

    [HideInInspector] public InventoryItem[] items;

    [System.Serializable]
    public class InventoryItem
    {
        public Item item;
        public int count;
        public InventoryItem(Item _item, int _count)
        {
            item = _item;
            count = _count;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        items = new InventoryItem[maxCapacity];
        for (int i = 0; i < maxCapacity; i++)
            items[i] = null;
    }

    public bool CanAddItem(Item _item)
    {
        if (_item.itemType != Item.ItemType.Equipment && _item.itemType != Item.ItemType.Blueprint && _item.itemType != Item.ItemType.ResearchLog )
        {
            for (int i = 0; i < maxCapacity; i++)
            {
                if (items[i] != null && items[i].item != null && items[i].item.itemName == _item.itemName)
                    return true;
            }
        }
        
        for (int i = 0; i < maxCapacity; i++)
        {
            if (items[i] == null)
                return true;
        }

        return false;
    }

    public bool AddItem(Item _item, int _count = 1, int slotIndex = -1)
    {
        if (_item == null)
        {
            Debug.LogError("InventoryManager.AddItem: _item is null.");
            return false;
        }

        if (_item.itemType != Item.ItemType.Equipment && _item.itemType != Item.ItemType.Blueprint && _item.itemType != Item.ItemType.ResearchLog)
        {
            for (int i = 0; i < maxCapacity; i++)
            {
                if (items[i] != null && items[i].item != null && items[i].item.itemName == _item.itemName)
                {
                    items[i].count += _count;
                    return true;
                }
            }
        }

        if (slotIndex >= 0 && slotIndex < maxCapacity && items[slotIndex] == null)
        {
            items[slotIndex] = new InventoryItem(_item, _count);
            return true;
        }

        for (int i = 0; i < maxCapacity; i++)
        {
            if (items[i] == null)
            {
                items[i] = new InventoryItem(_item, _count);
                return true;
            }
        }

        Debug.LogWarning("Inventory is full. Cannot add item: " + _item.itemName);
        return false;
    }

    public void InsertItemAt(int slotIndex, Item _item, int _count)
    {
        if (_item == null || slotIndex < 0 || slotIndex >= maxCapacity)
            return;
        items[slotIndex] = new InventoryItem(_item, _count);
    }

    public void RemoveItem(Item _item, int _count = 1)
    {
        for (int i = 0; i < maxCapacity; i++)
        {
            if (items[i] != null && items[i].item != null && items[i].item.itemName == _item.itemName)
            {
                items[i].count -= _count;
                if (items[i].count <= 0)
                    items[i] = null;
                return;
            }
        }
    }

    public InventoryData SaveInventoryData()
    {
        InventoryData data = new InventoryData();
        for (int i = 0; i < maxCapacity; i++)
        {
            if (items[i] != null && items[i].item != null)
            {
                data.slots.Add(new InventorySlotData
                {
                    itemName = items[i].item.itemName,
                    itemCount = items[i].count
                });
            }
        }
        return data;
    }

    public void LoadInventoryData(InventoryData data)
    {
        items = new InventoryItem[maxCapacity];
        for (int i = 0; i < maxCapacity; i++)
            items[i] = null;

        foreach (var slotData in data.slots)
        {
            Item item = ItemDatabase.Instance.GetItemByName(slotData.itemName);
            if (item != null)
                AddItem(item, slotData.itemCount);
        }
    }

    public void RemoveItemAt(int slotIndex, int _count = 1)
    {
        if (slotIndex < 0 || slotIndex >= maxCapacity) return;

        var invItem = items[slotIndex];
        if (invItem != null && invItem.item != null)
        {
            invItem.count -= _count;
            if (invItem.count <= 0)
                items[slotIndex] = null;
        }
    }
}
