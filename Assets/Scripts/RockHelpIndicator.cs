using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHelpIndicator : MonoBehaviour
{
    public GameObject player;
    private GameObject helpIndicator;
    // Start is called before the first frame update
    void Start()
    {
        helpIndicator = transform.Find("HelpIndicator").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Player within distance
        if (Vector3.Distance(player.transform.position, transform.position) < 2.5f)
            helpIndicator.SetActive(true);
        else
            helpIndicator.SetActive(false);
    }
}
