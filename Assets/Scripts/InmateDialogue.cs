using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InmateDialogue : MonoBehaviour
{
    public Player player;
    private GameObject dialogBox;

    private void Awake()
    {
        dialogBox = transform.Find("DialogBox").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Player within distance
        if (Vector3.Distance(player.transform.position, transform.position) < 5f)
        {
            dialogBox.SetActive(true);
        }
        else
        {
            dialogBox.SetActive(false);
        }
    }
}
