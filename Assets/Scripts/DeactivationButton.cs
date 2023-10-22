using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivationButton : MonoBehaviour
{
    [SerializeField] private UI_Deactivation_Time uiDeactivationTime;
    private SecurityCamera securityCamera;
    public GameObject player;
    

    private GameObject helpIndicator;

    private bool countingDown = false;
    private float currentTime = 0f;
    [SerializeField] private float duration;

    void Start()
    {
        helpIndicator = transform.Find("HelpIndicator").gameObject;
        securityCamera = gameObject.GetComponentInParent<SecurityCamera>();
        currentTime = duration;
    }

    // Update is called once per frame
    void Update()
    {
        // Begin counting down once button pressed
        if (countingDown)
        {
            currentTime -= 1 * Time.deltaTime;
            uiDeactivationTime.SetTimerText(currentTime.ToString("0.0"));

            if (currentTime <= 0f)
            {
                countingDown = false;
                currentTime = duration;
                uiDeactivationTime.SetTimerText("");
                securityCamera.setActive(true);
            }
        }
        // Player within distance
        if (Vector3.Distance(player.transform.position, transform.position) < 1.5f)
        {
            helpIndicator.SetActive(true);  
            // E pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                SoundManager.PlaySound(SoundManager.Sound.Deactivation, transform.position);
                securityCamera.setActive(false);
                countingDown = true;
            }
        } else
            helpIndicator.SetActive(false);
    }

    public void ResetCamera()
    {
        countingDown = false;
        currentTime = duration;
        uiDeactivationTime.SetTimerText("");
        securityCamera.setActive(true);
    }
}
