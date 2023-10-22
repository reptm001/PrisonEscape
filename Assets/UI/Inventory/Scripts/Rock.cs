using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public const float MAX_FORCE = 100f;
    [SerializeField] private Transform forceTransform;
    private SpriteMask forceSpriteMask;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private Player player;
    private GameObject[] guards;

    private Vector3 offset = new Vector3(0.5f, 0.6f);

    private bool launched = false;

    private void Awake()
    {
        forceSpriteMask = forceTransform.Find("Mask").GetComponent<SpriteMask>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        player = gameObject.GetComponentInParent<Player>();
        guards = GameObject.FindGameObjectsWithTag("Guard");

        HideForce();
    }

    private void Update()
    {
        if (!launched)
            transform.position = player.transform.position + offset;
        forceTransform.position = transform.position;
        Vector2 dir = (GetMouseWorldPosition() - transform.position).normalized;
        forceTransform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    public void Launch(float force, Item rockItem)
    {
        Vector2 dir = (GetMouseWorldPosition() - transform.position).normalized * -1f;
        launched = true;
        rb.velocity = dir * force;
        HideForce();
        StartCoroutine(WaitUntilStopped(rockItem));
    }

    public void ShowForce(float force)
    {
        forceSpriteMask.alphaCutoff = 1 - force / MAX_FORCE;
    }

    private void HideForce()
    {
        forceSpriteMask.alphaCutoff = 1;
    }

    IEnumerator WaitUntilStopped(Item rockItem)
    {
        // Enable collider after leaving player
        while (Vector3.Distance(transform.position, player.transform.position) < 0.8f)
            yield return null;
        circleCollider.enabled = true;
        while (rb.velocity.SqrMagnitude() > 0.01)
        {
            yield return null;
        }
        SoundManager.PlaySound(SoundManager.Sound.RockLand, transform.position);
        ItemWorld.SpawnItemWorld(transform.position, rockItem, false, false);
        AlertClosestGuards();
        Destroy(gameObject);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    private void AlertClosestGuards()
    {
        // TODO: SoundManager play rock hit
        foreach (GameObject g in guards)
        {
            GuardMovement gm = g.GetComponent<GuardMovement>();
            if (gm.GetDistanceFromPlayer() < 10f)
            {
                gm.AlertToPosition(transform.position, true);
            }
        }
        // TODO: Wait
    }
}
