using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEvadeState : IBossState
{
    private Boss _boss;
    private ILaserHandler _laserHandler;


    private float evadeTimer = 6f;
    private bool isDodging = false;
    private bool isDodgingLeft = true; // Toggle to determine dodge direction
    private Vector3 originPosition; // Center of the circle

    public BossEvadeState(Boss boss)
    {
        _boss = boss;
        _laserHandler = _boss.GetEvadeLaserHandler(); // Use Evade Laser Handler
    }

    public void Enter(Boss boss)
    {
        Debug.Log("Entered Evade State");
        isDodging = false;
        originPosition = boss.transform.position; // Set the origin as the boss's starting position
        _laserHandler.FireLaser(boss.transform, boss.playerTransform); // Fire evade lasers upon entry
    }

    public void Execute(Boss boss)
    {            
        evadeTimer -= Time.deltaTime;

        if (evadeTimer > 0)
        {
            if (!isDodging)
            {
                // Idle or wait for laser detection
            }
        }
        else
        {
            // Return to the center and transition to the next state
            ReturnToCenter(boss);
        }
    }

    public void TriggerEvade(Boss boss, Vector3 laserPosition)
    {
        if (isDodging) return;

        // Determine the dodge direction
        Vector3 dodgeDirection = isDodgingLeft
            ? Vector3.left
            : Vector3.right; // Toggle between left and right

        // Add a vertical component to the dodge for randomness
        Vector2 randomVerticalOffset = Random.insideUnitCircle * 3f;
        dodgeDirection = (dodgeDirection + new Vector3(0, randomVerticalOffset.y, 0)).normalized;

        // Scale the dodge direction to fit within a 5f radius
        Vector3 dodgePosition = boss.transform.position + dodgeDirection * 3f;

        // Restrict the dodge position within the circular boundary
        dodgePosition = ConstrainToCircle(dodgePosition, originPosition, 3f);

        // Start smooth movement to the dodge position
        boss.StartCoroutine(SmoothDodge(boss, dodgePosition));

        // Toggle the dodge direction for the next dodge
        isDodgingLeft = !isDodgingLeft;
    }
    private Vector3 ConstrainToCircle(Vector3 position, Vector3 center, float radius)
    {
        // Calculate the offset from the center
        Vector3 offset = position - center;

        // Clamp the offset to the maximum radius
        offset = Vector3.ClampMagnitude(offset, radius);

        // Return the constrained position
        return center + offset;
    }

    private System.Collections.IEnumerator SmoothDodge(Boss boss, Vector3 targetPosition)
    {
        isDodging = true;
        float elapsedTime = 0f;
        float dodgeDuration = 0.15f; // Time to complete the dodge
        Vector3 startingPosition = boss.transform.position;

        while (elapsedTime < dodgeDuration)
        {
            // Smoothly interpolate between starting and target positions
            boss.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / dodgeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exactly the target position
        boss.transform.position = targetPosition;
        isDodging = false;
    }

    private void ReturnToCenter(Boss boss)
    {
        Vector3 targetPosition = new Vector3(boss.transform.position.x, 5f, boss.transform.position.z);
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, boss.speed * Time.deltaTime);

        if (Vector3.Distance(boss.transform.position, targetPosition) < 0.1f)
        {
            boss.SetState(new BossIdleState());
        }
    }

    public void Exit(Boss boss)
    {
        Debug.Log("Exiting Evade State");
    }
}
