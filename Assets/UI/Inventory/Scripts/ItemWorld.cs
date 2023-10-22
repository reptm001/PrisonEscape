using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item, bool infiniteSource, bool blueKey)
    {
        Transform transform;
        if (blueKey)
            transform = Instantiate(GameAssets.i.pfItemWorldBlueKey, position, Quaternion.identity);
        else
            transform = Instantiate(GameAssets.i.pfItemWorld, position, Quaternion.identity);

        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);
        itemWorld.SetInfiniteSource(infiniteSource);

        return itemWorld;
    }

    private Item item;
    private SpriteRenderer spriteRenderer;
    private bool infiniteSource;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = GetSprite(item);
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private Sprite GetSprite(Item item)
    {
        foreach (GameAssets.ItemSprite itemSprite in GameAssets.i.itemSpriteArray)
        {
            if (itemSprite.itemType == item.itemType)
            {
                return itemSprite.sprite;
            }
        }
        Debug.LogError("Item " + item.itemType + " not found.");
        return null;
    }

    public bool IsInfinite()
    {
        return infiniteSource;
    }

    public void SetInfiniteSource(bool infiniteSource)
    {
        this.infiniteSource = infiniteSource;
    }
}
