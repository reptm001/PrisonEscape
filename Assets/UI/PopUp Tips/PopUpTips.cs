using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTips : MonoBehaviour
{
    public static bool GamePaused = false;

    public GameObject popUpTipsUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
        }
    }

    public void Resume()
    {
        popUpTipsUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
        Transform popUpTipsUITransform = transform.Find("PopUpTips");
        popUpTipsUITransform.Find("WhistlingText").gameObject.SetActive(false);
        popUpTipsUITransform.Find("WhistlingVideo").gameObject.SetActive(false);
        popUpTipsUITransform.Find("ThrowingText").gameObject.SetActive(false);
        popUpTipsUITransform.Find("ThrowingVideo").gameObject.SetActive(false);
    }

    public void ShowTips(bool whistling)
    {
        if (!popUpTipsUI.activeSelf)
        {
            Transform popUpTipsUITransform;
            if (whistling)
            {
                popUpTipsUITransform = transform.Find("PopUpTips");
                popUpTipsUITransform.Find("WhistlingText").gameObject.SetActive(true);
                popUpTipsUITransform.Find("WhistlingVideo").gameObject.SetActive(true);
            } else
            {
                popUpTipsUITransform = transform.Find("PopUpTips");
                popUpTipsUITransform.Find("ThrowingText").gameObject.SetActive(true);
                popUpTipsUITransform.Find("ThrowingVideo").gameObject.SetActive(true);
            }
            popUpTipsUI.SetActive(true);
            Time.timeScale = 0f;
            GamePaused = true;
        }
    }
}
