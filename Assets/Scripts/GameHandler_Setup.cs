using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler_Setup : MonoBehaviour
{
    private void Awake()
    {
        SoundManager.Initialize();
    }
}
