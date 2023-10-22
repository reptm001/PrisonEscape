using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GuardAI : MonoBehaviour
{
    public Vector3 target;

    public float moveSpeed = 200.0f;
    public float chaseSpeed = 1200.0f;
    public float investigateSpeed = 70.0f;
    private float speed;
    public float nextWaypointDistance = 0.45f;

    private Path path;
    private bool isStatic;
    int currentWaypoint = 0;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        speed = moveSpeed;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            if (target != null)
            {
                if (!isStatic)
                    seeker.StartPath(rb.position, target, OnPathCompute);
            }
        }
    }

    void OnPathCompute(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        } 

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Update animator if direction is chosen (not zero)
        if (direction.sqrMagnitude > 0f)
        {
            if (speed == moveSpeed)
                SoundManager.PlaySound(SoundManager.Sound.GuardWalking, transform.position);
            else
                SoundManager.PlaySound(SoundManager.Sound.GuardRunning, transform.position);
            animator.SetFloat("Horizontal", direction.x, 1f, Time.deltaTime * 10f);
            animator.SetFloat("Vertical", direction.y, 1f, Time.deltaTime * 10f);
            animator.SetFloat("Speed", 1.0f);
        }
    }

    public void SetTarget(Vector3 target)
    {
        if (this.target != target)
        {
            this.target = target;
        }
    }

    public void SetSpeed(float speed)
    {
        if (this.speed != speed)
        {
            this.speed = speed;
        }
    }

    public void SetStatic(bool isStatic)
    {
        this.isStatic = isStatic;
    }
}
