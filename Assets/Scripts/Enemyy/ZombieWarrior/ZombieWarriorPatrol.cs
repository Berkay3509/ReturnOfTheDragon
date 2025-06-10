using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWarriorPatrol : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float idleDuration = 1f;
    [SerializeField] private bool startImmediately = true;

    [Header("Player Detection")]
    [SerializeField] private Vector2 detectionSize = new Vector2(5f, 2f);
    [SerializeField] private Vector2 detectionOffset = new Vector2(1f, 0f);
    [SerializeField] private LayerMask playerLayer;

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform player;

    private int currentWaypointIndex = 0;
    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool isFacingRight;
    private bool isPlayerDetected = false;

    private void Start()
    {
        if (waypoints.Length > 0)
        {
            DetermineInitialFacingDirection();
        }

        if (anim != null)
        {
            anim.SetBool("MoveTrigger", !isIdle && startImmediately);
        }
    }

    private void Update()
    {
        CheckPlayerDetection();

        if (isPlayerDetected)
        {
            if (anim != null && anim.GetBool("MoveTrigger"))
            {
                anim.SetBool("MoveTrigger", false);
            }
            return;
        }

        if (waypoints.Length == 0) return;

        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                if (anim != null) anim.SetBool("MoveTrigger", true);
            }
        }
        else
        {
            MoveToWaypoint();
        }
    }

    private void CheckPlayerDetection()
    {
        Vector2 detectionCenter = (Vector2)transform.position +
                                new Vector2(
                                    isFacingRight ? detectionOffset.x : -detectionOffset.x,
                                    detectionOffset.y
                                );

        bool detected = Physics2D.OverlapBox(
            detectionCenter,
            detectionSize,
            0f,
            playerLayer);

        if (detected && !isPlayerDetected)
        {
            isPlayerDetected = true;
            if (anim != null) anim.SetBool("MoveTrigger", false);
        }
        else if (!detected && isPlayerDetected)
        {
            isPlayerDetected = false;
            if (anim != null) anim.SetBool("MoveTrigger", true);
        }
    }

    private void DetermineInitialFacingDirection()
    {
        float directionToWaypoint = waypoints[currentWaypointIndex].position.x - transform.position.x;
        isFacingRight = directionToWaypoint > 0;
        UpdateSpriteDirection();
    }

    private void MoveToWaypoint()
    {
        // Get target position but keep current Y position (prevent vertical movement)
        Vector2 targetPosition = new Vector2(
            waypoints[currentWaypointIndex].position.x,
            transform.position.y // Maintain original Y position
        );

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        float moveDirection = targetPosition.x - transform.position.x;
        bool shouldFaceRight = moveDirection > 0;

        if ((shouldFaceRight && !isFacingRight) || (!shouldFaceRight && isFacingRight))
        {
            isFacingRight = shouldFaceRight;
            UpdateSpriteDirection();
        }

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            isIdle = true;
            idleTimer = 0f;
            if (anim != null) anim.SetBool("MoveTrigger", false);
        }
    }

    private void UpdateSpriteDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? -1 : 1);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        // Detection area (red/yellow)
        Gizmos.color = isPlayerDetected ? Color.red : Color.yellow;
        Vector2 detectionCenter = (Vector2)transform.position +
                                new Vector2(
                                    isFacingRight ? detectionOffset.x : -detectionOffset.x,
                                    detectionOffset.y
                                );
        Gizmos.DrawWireCube(detectionCenter, detectionSize);

        // Waypoint path (blue)
        Gizmos.color = Color.blue;
        if (waypoints != null && waypoints.Length > 1)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    // Draw all waypoints at the same Y level as the object
                    Vector2 wpPos = new Vector2(waypoints[i].position.x, transform.position.y);
                    Gizmos.DrawSphere(wpPos, 0.2f);
                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        Vector2 nextWpPos = new Vector2(waypoints[i + 1].position.x, transform.position.y);
                        Gizmos.DrawLine(wpPos, nextWpPos);
                    }
                }
            }
        }
    }
}
