using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;

    public Rigidbody2D rb;
    public Animator animator;
    public GameHandler gameHandler;

    Vector2 movement;

    private bool whistling = false;
    private GameObject whistleIndicator;
    private GameObject wasd;
    private BackgroundAudio backgroundAudio;

    private GameObject[] guards;

    private bool throwing = false;

    private void Awake()
    {
        GameObject backgroundAudioObject = GameObject.FindGameObjectWithTag("Audio");
        backgroundAudio = backgroundAudioObject.GetComponent<BackgroundAudio>();
        wasd = transform.Find("WASD").gameObject;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        whistleIndicator = transform.Find("WhistleIndicator").gameObject;

        guards = GameObject.FindGameObjectsWithTag("Guard");
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<Player>().IsHiding())
        {
            // Movement input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }

        if (movement.x != 0 || movement.y != 0)
        {
            wasd.SetActive(false);
            SoundManager.PlaySound(SoundManager.Sound.PlayerWalking);
        }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // X pressed - whistle
        if (gameHandler.MechanicShown('x'))
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!whistling)
                {
                    whistling = true;
                    SoundManager.PlaySound(SoundManager.Sound.Whistle);
                    AlertClosestGuards();
                    whistling = false;
                }
            }
        }
        // Q pressed - throw rock
        if (gameHandler.MechanicShown('q'))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Inventory inventory = GetComponent<Player>().GetInventory();
                if (inventory.HasItemType(Item.ItemType.Rock))
                {
                    if (!throwing)
                    {
                        throwing = true;
                        inventory.UseItem(inventory.GetItem(Item.ItemType.Rock));
                    }
                } 
            } 
        }

        bool closeToGuards = false;
        // SoundManager - Proximity to guards
        foreach(GameObject g in guards)
        {
            if (Vector3.Distance(transform.position, g.transform.position) < 4f)
                closeToGuards = true;
            if (g.GetComponent<GuardMovement>().IsChasing())
                closeToGuards = true;
        }
        if (closeToGuards)
            backgroundAudio.PlayTense();
        else
            backgroundAudio.PlayAmbient();

    }

    // Update called on a fixed timer (default 50fps)
    void FixedUpdate()
    {
        // Movement
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime); // Constant movement speed
    }

    private void AlertClosestGuards()
    {
        // TODO: SoundManager play whistle
        whistleIndicator.SetActive(true);
        foreach (GameObject g in guards)
        {
            GuardMovement gm = g.GetComponent<GuardMovement>();
            if (gm.GetDistanceFromPlayer() < 20f)
            {
                gm.AlertToPosition(transform.position, false);
            }
        }
        StartCoroutine(HideWhistleIndicator(1f));
        
    }

    IEnumerator HideWhistleIndicator(float t)
    {
        yield return new WaitForSeconds(t);
        whistleIndicator.SetActive(false);
    }

    public void PlayerThrown()
    {
        throwing = false;
    }

    public void ResetMovement()
    {
        movement.x = 0f;
        movement.y = 0f;
    }
}
