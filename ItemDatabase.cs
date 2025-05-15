using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [SerializeField]
    private List<Item> itemList;

    private Dictionary<string, Item> itemDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            itemDict = new Dictionary<string, Item>();
            foreach (var item in itemList)
            {
                itemDict[item.itemName] = item;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Item GetItemByName(string name)
    {
        if (itemDict.TryGetValue(name, out var item))
            return item;

        Debug.LogWarning("Item not found in database: " + name);
        return null;
    }
}
