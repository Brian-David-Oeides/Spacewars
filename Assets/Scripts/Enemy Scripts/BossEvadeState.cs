using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEvadeState : IBossState
{
    private float evadeTimer = 6f;
    private bool dodgeLeft = true;
    private bool isDodging = false;

    public void Enter(Boss boss)
    {
        Debug.Log("Entered Evade State");
        isDodging = false;
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

        // Determine dodge direction based on the laser's position
        float dodgeDirection = dodgeLeft ? -1 : 1;
        dodgeLeft = !dodgeLeft; // Alternate dodge direction

        // Move boss outside the range of the laser
        Vector3 dodgePosition = boss.transform.position + new Vector3(dodgeDirection * boss.dodgeDistance, 0, 0);

        // Start smooth movement to dodge position
        boss.StartCoroutine(SmoothDodge(boss, dodgePosition));
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
