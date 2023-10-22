using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private UI_Inventory uiInventory;
    private Inventory inventory;
    private Animator animator;
    private PlayerMovement playerMovement;

    private Item rockItem = null;
    private Rock rock;

    private float holdDownStartTime;

    private bool hiding = false;

    private bool firstGuardDestroyed = false;

    private List<string> seenDialog = new List<string>();

    private bool showingDialog = false;

    public GameHandler gameHandler;

    public PopUpTips popUpTips;

    public CameraFollow cameraFollow;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rockItem != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Mouse down, start holding
                holdDownStartTime = Time.time;
            }

            if (Input.GetMouseButton(0))
            {
                // Mouse remains down, show force
                float holdDownTime = Time.time - holdDownStartTime;
                rock.ShowForce(CalculateHoldDownForce(holdDownTime));
            }

            if (Input.GetMouseButtonUp(0))
            {
                playerMovement.PlayerThrown();
                // Mouse up, launch rock
                SoundManager.PlaySound(SoundManager.Sound.RockThrow);
                float holdDownTime = Time.time - holdDownStartTime;
                rock.Launch(CalculateHoldDownForce(holdDownTime), rockItem);
                rockItem = null;
            }
        }
    }

    private void Awake()
    {
        inventory = new Inventory(UseItem);
        uiInventory.SetInventory(inventory);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            // Touching Item
            animator.SetTrigger("PickUp");
            Item item = itemWorld.GetItem();
            switch (item.itemType)
            {
                case Item.ItemType.Rock:
                    {
                        ShowDialog("2-3");
                        // Only allow pickup if player does not already have one
                        if (!inventory.HasItemType(Item.ItemType.Rock))
                        {
                            inventory.AddItem(item);
                            if (!itemWorld.IsInfinite())
                                itemWorld.DestroySelf(); // Destroy if non-infinite
                        }
                        break;
                    }
                case Item.ItemType.BlueKey:
                    {
                        StartCoroutine(WaitForGuardDestroy(itemWorld, item));
                        break;
                    }
                case Item.ItemType.RedKey:
                    {
                        inventory.AddItem(item);
                        if (!itemWorld.IsInfinite())
                            itemWorld.DestroySelf(); // Destroy if non-infinite
                        break;
                    }
            }
        }

        if (collider.gameObject.tag == "Dialog")
        {
            string name = collider.gameObject.name;
            if (name == "3")
            {
                cameraFollow.ShowLevel3();
            } else
            {
                if (name.Contains(","))
                {
                    string[] names = name.Split(',');
                    ShowDialog(names[0], names[1]);
                }
                else
                    ShowDialog(name);
            }
        }    
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public void ClearInventory()
    {
        inventory.ClearInventory();
        inventory.AddItem(new Item { itemType = Item.ItemType.NullKey, amount = 1 });
    }

    private void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Rock:
                inventory.RemoveItem(item);
                Vector3 offset = new Vector3(0.5f, 0.6f);
                Transform rockTransform = Instantiate(GameAssets.i.pfRock, transform.position + offset, Quaternion.identity, gameObject.transform);
                rock = rockTransform.GetComponent<Rock>();
                rockItem = item;
                break;
        }
    }

    public bool IsHiding()
    {
        return hiding;
    }

    public void SetHiding(bool hiding)
    {
        this.hiding = hiding;
        if (hiding)
        {
            playerMovement.ResetMovement();
            GetComponent<SpriteRenderer>().enabled = false;
        } else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private float CalculateHoldDownForce(float holdTime)
    {
        float maxForceHoldDownTime = 2f;
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        return holdTimeNormalized * Rock.MAX_FORCE;
    }

    public void ShowDialog(string s)
    {
        if (!seenDialog.Contains(s))
        {
            seenDialog.Add(s);
            GameObject go = transform.Find(s).gameObject;
            StartCoroutine(ShowHideDialog(go, s));
        }
    }

    public void ShowDialog(string s, string s2)
    {
        if (!seenDialog.Contains(s))
        {
            seenDialog.Add(s);
            GameObject go = transform.Find(s).gameObject;
            GameObject go2 = transform.Find(s2).gameObject;
            StartCoroutine(ShowHideDialog(go, go2, s2));
        }
    }

    IEnumerator ShowHideDialog(GameObject go, string s)
    {
        while (showingDialog) 
            yield return null;
        showingDialog = true;
        yield return new WaitForSeconds(1f);
        go.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        go.gameObject.SetActive(false);
        if (s == "2-3")
            StartCoroutine(ShowTip(false));
        showingDialog = false;
    }

    IEnumerator ShowHideDialog(GameObject go, GameObject go2, string s)
    {
        while (showingDialog)
            yield return null;
        showingDialog = true;
        yield return new WaitForSeconds(1f);
        go.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        go.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        go2.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        go2.gameObject.SetActive(false);
        if (s == "1-2")
            StartCoroutine(ShowTip(true));
        showingDialog = false;
    }

    IEnumerator ShowTip(bool whistling)
    {
        yield return new WaitForSeconds(1f);
        popUpTips.ShowTips(whistling);
        if (whistling)
            gameHandler.AddMechanic('x');
        else
            gameHandler.AddMechanic('q');
    }

    public void FirstGuardDestroyed()
    {
        firstGuardDestroyed = true;
    }

    IEnumerator WaitForGuardDestroy(ItemWorld itemWorld, Item item)
    {
        while (!firstGuardDestroyed)
            yield return null;
        inventory.AddItem(item);
        itemWorld.DestroySelf(); // Destroy if non-infinite
    }
}
