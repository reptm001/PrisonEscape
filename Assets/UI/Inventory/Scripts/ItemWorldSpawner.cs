using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldSpawner : MonoBehaviour
{
    public Item item;
    public bool infiniteSource;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ItemWorld.SpawnItemWorld(transform.position, item, infiniteSource, false);
        spriteRenderer.enabled = false;
    }

    public void SpawnItem()
    {
        ItemWorld.SpawnItemWorld(transform.position, item, infiniteSource, false);
    }
}
