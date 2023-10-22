using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10f;
    public Vector3 offset;
    public List<Vector3> movePoints;
    private int movePointIndex = 0;

    private bool showingLevel = false;
    private bool shownLevel = false;

    private void FixedUpdate()
    {
        if (!showingLevel)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        } else
        {
            Vector3 desiredPosition = movePoints[movePointIndex] + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        
    }

    public void ShowLevel3()
    {
        if (!shownLevel)
        {
            shownLevel = true;
            StartCoroutine(SmoothShowLevel3());
        }
    }

    IEnumerator SmoothShowLevel3()
    {
        Player player = target.gameObject.GetComponent<Player>();
        player.SetHiding(true);
        movePointIndex = 0;
        yield return new WaitForSeconds(0.5f);
        showingLevel = true;
        for (int i = 0; i < movePoints.Count - 1; i++)
        {
            yield return new WaitForSeconds(2f);
            movePointIndex++;
        }
        yield return new WaitForSeconds(4f);
        player.SetHiding(false);
        showingLevel = false;
    }
}
