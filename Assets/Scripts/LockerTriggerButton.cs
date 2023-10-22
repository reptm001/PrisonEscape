using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerTriggerButton : MonoBehaviour
{
    private Animator lockerAnim;
    public GameObject player;

    private GameObject helpIndicator;

    private bool hiding = false;

    private void Start()
    {
        helpIndicator = transform.parent.Find("HelpIndicator").gameObject;
        lockerAnim = gameObject.GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Player within distance
        if (!hiding)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 1.5f)
            {
                // E pressed
                if (Input.GetKeyDown(KeyCode.E))
                {
                    helpIndicator.SetActive(false);
                    hiding = true;
                    lockerAnim.SetTrigger("Hide");
                    player.GetComponent<Player>().SetHiding(true);
                }
                if (!hiding)
                    helpIndicator.SetActive(true);
            } else
            {
                helpIndicator.SetActive(false);
            }
        } else {
            // E pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                hiding = false;
                lockerAnim.SetTrigger("Hide"); // Same clip for hide/unhide
                player.GetComponent<Player>().SetHiding(false);
            }
        }
        
    }
}
