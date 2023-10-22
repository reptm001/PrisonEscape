using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DoorAnimated : MonoBehaviour
{
    private Animator animator;
    private ShadowCaster2D shadowCaster;

    public GameHandler gameHandler;
    public bool exitDoor = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        shadowCaster = GetComponent<ShadowCaster2D>();
    }

    public void OpenDoor()
    {
        SoundManager.PlaySound(SoundManager.Sound.CellDoorOpen, transform.position);
        animator.SetBool("Open", true);
        if (shadowCaster != null)
        {
            shadowCaster.enabled = false;
            shadowCaster.Update();
        }
        if (exitDoor)
        {
            gameHandler.EndGame();
        }
    }

    public void CloseDoor()
    {
        SoundManager.PlaySound(SoundManager.Sound.CellDoorClose, transform.position);
        animator.SetBool("Open", false);
        if (shadowCaster != null)
        {
            shadowCaster.enabled = true;
            shadowCaster.Update();
        }
    }
}
