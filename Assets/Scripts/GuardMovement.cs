using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GuardMovement : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        ChasingPlayer,
        Investigating,
        Idling,
        InvestigateIdling,
    }
    private State state;

    private GuardAI guardAI;
    private Animator animator;
    private Light2D flashlight;
    private Rigidbody2D rb;

    public bool canHear = true;
    public bool isStatic = false;
    public bool movesAfterWhistle = false;
    public bool isScripted = false;
    public float inactivityTime;
    public float idleTime;
    public float investigateIdleTime;
    public float investigateTime;
    public float catchingDistance;

    public List<Vector3> patrolPoints;
    private int patrolPointsIndex = 0;

    private GameObject warningIndicator;
    private GameObject alarmIndicator;

    public Player player;
    public GameHandler gameHandler;

    private Vector3 investigatePosition;

    private float timer;
    private float inactivityTimer;

    private bool playerRespawning = false;

    private void Awake()
    {
        guardAI = GetComponent<GuardAI>();
        animator = GetComponent<Animator>();
        flashlight = gameObject.transform.Find("Flashlight").GetComponent<Light2D>();
        rb = GetComponent<Rigidbody2D>();

        warningIndicator = gameObject.transform.Find("WarningIndicator").gameObject;
        alarmIndicator = gameObject.transform.Find("AlarmIndicator").gameObject;

        inactivityTimer = inactivityTime;
        state = State.Patrolling;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.SqrMagnitude() < 0.2f)
        {
            inactivityTimer -= Time.deltaTime;
            if (inactivityTimer < 0)
            {
                transform.position = patrolPoints[patrolPointsIndex];
                state = State.Patrolling;
            }
        }
        else
        {
            // Reset inactivity timer if moving
            inactivityTimer = inactivityTime;
        }
        
        // Reset inactivity timer if static guard and at patrol point
        if (isStatic)
        {
            if (Vector3.Distance(transform.position, patrolPoints[patrolPointsIndex]) < 1.0f)
            {
                inactivityTimer = inactivityTime;
            }
        }
        if (!isScripted)
            if (Vector3.Distance(transform.position, player.transform.position) < catchingDistance)
                state = State.ChasingPlayer;

        switch (state)
        {
            default:
            case State.Patrolling:
                alarmIndicator.SetActive(false);
                warningIndicator.SetActive(false);
                if (!isStatic) {
                    guardAI.SetSpeed(guardAI.moveSpeed);
                    guardAI.SetStatic(false);
                    guardAI.SetTarget(patrolPoints[patrolPointsIndex]);
                    // When point reached,
                    if (Vector3.Distance(transform.position, patrolPoints[patrolPointsIndex]) < 1.0f)
                    {
                        timer = idleTime;
                        state = State.Idling;
                    }
                } else
                {
                    if (Vector3.Distance(transform.position, patrolPoints[patrolPointsIndex]) > 1.0f) {
                        guardAI.SetSpeed(guardAI.moveSpeed);
                        guardAI.SetStatic(false);
                        guardAI.SetTarget(patrolPoints[patrolPointsIndex]);
                    } else
                    {
                        animator.SetFloat("Speed", 0f);
                        guardAI.SetStatic(true);
                    }
                }
                if (!isScripted)
                    FindPlayer();
                break;
            case State.ChasingPlayer:
                alarmIndicator.SetActive(true);
                warningIndicator.SetActive(false);
                guardAI.SetSpeed(guardAI.chaseSpeed);
                guardAI.SetStatic(false);
                guardAI.SetTarget(player.transform.position);
                // Caught player
                if (Vector3.Distance(transform.position, player.transform.position) < catchingDistance)
                {
                    gameHandler.Respawn();
                    if (!playerRespawning)
                    {
                        playerRespawning = true;
                        StartCoroutine(ResetPosition());
                    }
                    break;
                }
                    
                // Lost player
                if (Vector3.Distance(transform.position, player.transform.position) > (flashlight.pointLightOuterRadius + 3.0f))
                {
                    state = State.Patrolling;
                }

                break;
            case State.Investigating:
                alarmIndicator.SetActive(false);
                warningIndicator.SetActive(true);
                guardAI.SetSpeed(guardAI.investigateSpeed);
                guardAI.SetStatic(false);
                guardAI.SetTarget(investigatePosition);

                timer -= Time.deltaTime;
                // Reached Position
                if (Vector3.Distance(transform.position, investigatePosition) < 1.2f)
                {
                    if (timer < 0)
                    {
                        timer = idleTime;
                        state = State.Idling;
                    }
                }
                break;
            case State.Idling:
                alarmIndicator.SetActive(false);
                warningIndicator.SetActive(false);
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    if (patrolPointsIndex == (patrolPoints.Count - 1))
                    {
                        if (isScripted)
                        {
                            player.FirstGuardDestroyed();
                            Destroy(gameObject);
                        }
                        patrolPointsIndex = 0;
                    }
                    else
                    {
                        patrolPointsIndex++;
                        if (isScripted)
                            if (patrolPointsIndex == 2)
                            {
                                ItemWorld.SpawnItemWorld(new Vector3(-3.36f, -1.34f), new Item { itemType = Item.ItemType.BlueKey, amount = 1 }, false, true);
                                player.ShowDialog("0");
                            }
                    }
                    state = State.Patrolling;
                }
                break;
            case State.InvestigateIdling:
                if (movesAfterWhistle)
                    isStatic = false;
                alarmIndicator.SetActive(false);
                warningIndicator.SetActive(true);
                guardAI.SetStatic(true);
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = investigateTime;
                    state = State.Investigating;
                }
                break;
        }
    }

    // Update called on a fixed timer (default 50fps)
    void FixedUpdate()
    {
        flashlight.transform.eulerAngles = new Vector3(0, 0, GetViewAngle());
    }

    private void FindPlayer()
    {
        float viewDistance = flashlight.pointLightOuterRadius;
        float viewAngle = flashlight.pointLightOuterAngle / 2;
        // Check if player hiding
        if (!player.IsHiding())
        {
            // Chase player if within FOV
            if (Vector3.Distance(transform.position, player.transform.position) < viewDistance)
            {
                // (adjust for top, mid and bottom of player pos)
                Vector3 topOffset = new Vector3(0f, 1.25f);
                Vector3 midOffset = new Vector3(0f, 0.625f);
                Vector3 directionToPlayerBottom = (player.transform.position - transform.position).normalized;
                float angleBetweenGuardAndPlayerBottom = Vector3.Angle(GetViewDirection(), directionToPlayerBottom);
                Vector3 directionToPlayerTop = ((player.transform.position + topOffset) - transform.position).normalized;
                float angleBetweenGuardAndPlayerTop = Vector3.Angle(GetViewDirection(), directionToPlayerTop);
                Vector3 directionToPlayerMid = ((player.transform.position + midOffset) - transform.position).normalized;
                float angleBetweenGuardAndPlayerMid = Vector3.Angle(GetViewDirection(), directionToPlayerMid);
                if ((angleBetweenGuardAndPlayerBottom < viewAngle) || (angleBetweenGuardAndPlayerTop < viewAngle) || (angleBetweenGuardAndPlayerMid < viewAngle))
                {
                    // 2 hits for bottom and top of player
                    RaycastHit2D hit;
                    hit = Physics2D.Linecast(transform.position, player.transform.position, 1 << LayerMask.NameToLayer("Obstacle"));
                    if (hit.collider == null)
                    {
                        state = State.ChasingPlayer;
                    }
                }
            }
        }
    }

    private Vector3 GetViewDirection()
    {
        if (animator.GetFloat("Speed") < 0.01)
        {
            return new Vector3(0, -1);
        }
        else
        {
            return new Vector3(animator.GetFloat("Horizontal"), animator.GetFloat("Vertical"));
        }
    }

    private float GetViewAngle()
    {
        Vector3 viewDirection = GetViewDirection();
        return Mathf.Atan2(-viewDirection.x, viewDirection.y) * Mathf.Rad2Deg;
    }

    public float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public void AlertToPosition(Vector3 position, bool isRock)
    {
        if (isRock)
        {
            investigatePosition = position;
            timer = investigateIdleTime;
            state = State.InvestigateIdling;
        } else
        {
            if (canHear)
            {
                investigatePosition = position;
                timer = investigateIdleTime;
                state = State.InvestigateIdling;
            }
        }
    }

    // Reset guard position after respawn fade animation complete
    IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(1f);
        state = State.Patrolling;
        transform.position = patrolPoints[patrolPointsIndex];
        playerRespawning = false;
    }

    public bool IsChasing()
    {
        return (state == State.ChasingPlayer);
    }
}
