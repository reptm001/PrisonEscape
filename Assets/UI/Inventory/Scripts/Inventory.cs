using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemListChanged;

    private List<Item> itemList;
    private Action<Item> useItemAction;

    public Inventory(Action<Item> useItemAction)
    {
        this.useItemAction = useItemAction;
        itemList = new List<Item>();
        itemList.Add(new Item { itemType = Item.ItemType.NullKey, amount = 1 });
    }

    public void AddItem(Item item)
    {
        // Only allow player to hold one of everything (rocks)
        if (!HasItemType(item.itemType))
        {
            itemList.Add(item);
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UseItem(Item item)
    {
        useItemAction(item);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public Item GetItem(Item.ItemType itemType)
    {
        return itemList.Find(x => x.itemType == itemType);
    }

    public bool HasItemType(Item.ItemType itemType)
    {
        bool result = false;
        foreach(Item i in itemList)
        {
            if (i.itemType == itemType) result = true;
        }
        return result;
    }

    public void ClearInventory()
    {
        itemList.Clear();
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }
}
