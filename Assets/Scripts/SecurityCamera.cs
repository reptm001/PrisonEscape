using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SecurityCamera : MonoBehaviour
{
    private GameObject flashlightObject;
    private Light2D flashlight;

    private GameObject alarmIndicator;

    private Vector3 viewDirection;
    private bool active = true;

    public Player player;
    public GameHandler gameHandler;

    private void Awake()
    {
        flashlightObject = gameObject.transform.Find("Flashlight").gameObject;
        flashlight = flashlightObject.GetComponent<Light2D>();

        alarmIndicator = gameObject.transform.Find("AlarmIndicator").gameObject;

        viewDirection = flashlight.transform.up;
    }

    private void Update()
    {
        if (active)
        {
            FindPlayer();
        } 
    }

    private void FindPlayer()
    {
        float viewDistance = flashlight.pointLightOuterRadius;
        float viewAngle = flashlight.pointLightOuterAngle;
        // Check player if within FOV
        if (Vector3.Distance(transform.position, player.transform.position) < viewDistance)
        {
            // (adjust for top, mid and bottom of player pos)
            Vector3 topOffset = new Vector3(0f, 1.25f);
            Vector3 midOffset = new Vector3(0f, 0.625f);
            Vector3 directionToPlayerBottom = (player.transform.position - transform.position).normalized;
            float angleBetweenGuardAndPlayerBottom = Vector3.Angle(viewDirection, directionToPlayerBottom);
            Vector3 directionToPlayerTop = ((player.transform.position + topOffset) - transform.position).normalized;
            float angleBetweenGuardAndPlayerTop = Vector3.Angle(viewDirection, directionToPlayerTop);
            Vector3 directionToPlayerMid = ((player.transform.position + midOffset) - transform.position).normalized;
            float angleBetweenGuardAndPlayerMid = Vector3.Angle(viewDirection, directionToPlayerMid);
            if ((angleBetweenGuardAndPlayerBottom < viewAngle / 2) || (angleBetweenGuardAndPlayerTop < viewAngle / 2) || (angleBetweenGuardAndPlayerMid < viewAngle / 2))
            {
                // 2 hits for bottom and top of player
                RaycastHit2D hit;
                hit = Physics2D.Linecast(transform.position, player.transform.position, 1 << LayerMask.NameToLayer("Obstacle"));
                if (hit.collider == null)
                {
                    SoundManager.PlaySound(SoundManager.Sound.SecurityAlarm, transform.position);
                    alarmIndicator.SetActive(true);
                    gameHandler.Respawn();
                    alarmIndicator.SetActive(false);
                }
            }
        }
    }

    public void setActive(bool active)
    {
        if (active)
        {
            flashlightObject.SetActive(true);
        }
        else
        {
            flashlightObject.SetActive(false);
        }
        this.active = active;
    }
}
