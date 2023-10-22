using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Deactivation_Time : MonoBehaviour
{
    TextMeshProUGUI textUI;

    private void Awake()
    {
        textUI = transform.Find("Background").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetTimerText(string t)
    {
        textUI.text = t;
    }
}
