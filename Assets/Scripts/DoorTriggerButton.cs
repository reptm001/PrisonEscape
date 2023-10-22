using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerButton : MonoBehaviour
{
    private DoorAnimated door;
    public GameObject player;
    [SerializeField] Item requiredKey;
    private bool open = false;

    private GameObject helpIndicator;

    private void Start()
    {
        helpIndicator = transform.parent.Find("HelpIndicator").gameObject;
        door = gameObject.GetComponentInParent<DoorAnimated>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Player within distance
        if (Vector3.Distance(player.transform.position, transform.position) < 1.5f)
        {
            helpIndicator.SetActive(true);
                
            // E pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Player has key
                Inventory inventory = player.GetComponent<Player>().GetInventory();
                if (inventory.HasItemType(requiredKey.itemType))
                {
                    if (open)
                    {
                        door.CloseDoor();
                        open = false;
                    }
                    else
                    {
                        door.OpenDoor();
                        open = true;
                    }
                }
                else
                {
                    SoundManager.PlaySound(SoundManager.Sound.CellDoorLocked);
                }

            }
        } else
            helpIndicator.SetActive(false);
    }
}
